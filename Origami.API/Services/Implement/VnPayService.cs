using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Origami.API.Services.Implement;

public class VnPayService : BaseService<VnPayService>, IVnPayService
{
    private readonly IConfiguration _configuration;

    public VnPayService(
        IUnitOfWork<OrigamiDbContext> unitOfWork,
        ILogger<VnPayService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    ) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<TopUpResponse> TopUpWallet(TopUpRequest request)
    {
        if (request.Amount <= 0)
            throw new BadHttpRequestException("InvalidAmount");

        var userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

        // Lấy hoặc tạo wallet
        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            asNoTracking: false
        );

        if (wallet == null)
        {
            try
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await walletRepo.InsertAsync(wallet);
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                _logger.LogWarning(ex, "Duplicate key error when creating wallet for user {UserId}, attempting to reload", userId);

                wallet = await walletRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId,
                    asNoTracking: false
                ) ?? throw new BadHttpRequestException("FailedToCreateWallet");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating wallet for user {UserId}", userId);
                throw new BadHttpRequestException("FailedToCreateWallet");
            }
        }

        // Tạo transaction với status PENDING
        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        var transaction = new Transaction
        {
            ReceiverWalletId = wallet.WalletId,
            Amount = request.Amount,
            TransactionType = "deposit",
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow
        };
        await transactionRepo.InsertAsync(transaction);
        await _unitOfWork.CommitAsync();

        // Tạo payment URL với VNPAY
        var paymentUrl = CreateVnpayPaymentUrl(transaction.TransactionId, request.Amount, request.ReturnUrl);

        return new TopUpResponse
        {
            PaymentUrl = paymentUrl,
            TransactionCode = transaction.TransactionId.ToString()
        };
    }

    public async Task<bool> ProcessVnpayCallback(Dictionary<string, string> vnpayData)
    {
        if (!VerifyVnpaySignature(vnpayData))
            throw new BadHttpRequestException("InvalidSignature");

        var transactionId = int.Parse(vnpayData["vnp_TxnRef"]);
        var responseCode = vnpayData["vnp_ResponseCode"];
        var amount = decimal.Parse(vnpayData["vnp_Amount"]) / 100; // VNPAY trả về số tiền * 100

        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        var transaction = await transactionRepo.GetFirstOrDefaultAsync(
            predicate: x => x.TransactionId == transactionId,
            asNoTracking: false
        ) ?? throw new BadHttpRequestException("TransactionNotFound");

        // Nếu đã xử lý rồi thì không xử lý lại
        if (transaction.Status == "SUCCESS")
            return true;

        if (responseCode == "00") // Thành công
        {
            // Cập nhật transaction
            transaction.Status = "SUCCESS";

            // Cập nhật balance của wallet
            var walletRepo = _unitOfWork.GetRepository<Wallet>();
            var wallet = await walletRepo.GetFirstOrDefaultAsync(
                predicate: x => x.WalletId == transaction.ReceiverWalletId,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("WalletNotFound");

            wallet.Balance = (wallet.Balance ?? 0) + amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
            return true;
        }
        else
        {
            transaction.Status = "FAILED";
            await _unitOfWork.CommitAsync();
            return false;
        }
    }

    private string CreateVnpayPaymentUrl(int transactionId, decimal amount, string? returnUrl)
    {
        var vnpayConfig = _configuration.GetSection("VNPay");
        var tmnCode = vnpayConfig["TmnCode"];
        var hashSecret = vnpayConfig["HashSecret"];
        var url = vnpayConfig["Url"];
        var returnUrlConfig = vnpayConfig["ReturnUrl"];

        var vnp_Params = new Dictionary<string, string>
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode ?? "",
            ["vnp_Amount"] = ((long)(amount * 100)).ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = transactionId.ToString(),
            ["vnp_OrderInfo"] = $"Nạp tiền vào ví - Transaction {transactionId}",
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl ?? returnUrlConfig ?? "",
            ["vnp_IpAddr"] = GetIpAddress(),
            ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss")
        };

        var filteredParams = vnp_Params
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        var signData = string.Join("&", filteredParams
            .Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

        var vnp_SecureHash = HmacSHA512(hashSecret ?? "", signData);

        filteredParams["vnp_SecureHashType"] = "HMACSHA512";
        filteredParams["vnp_SecureHash"] = vnp_SecureHash;

        var queryString = string.Join("&", filteredParams
            .Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

        return $"{url}?{queryString}";
    }

    private bool VerifyVnpaySignature(Dictionary<string, string> vnpayData)
    {
        var vnpayConfig = _configuration.GetSection("VNPay");
        var hashSecret = vnpayConfig["HashSecret"];

        if (!vnpayData.ContainsKey("vnp_SecureHash"))
            return false;

        var vnp_SecureHash = vnpayData["vnp_SecureHash"];

        var paramsForSign = new Dictionary<string, string>(vnpayData);
        paramsForSign.Remove("vnp_SecureHash");
        paramsForSign.Remove("vnp_SecureHashType");

        var filteredParams = paramsForSign
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        var signData = string.Join("&", filteredParams
            .Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

        var checkSum = HmacSHA512(hashSecret ?? "", signData);
        return checkSum.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (byte theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }
        return hash.ToString();
    }

    private string GetIpAddress()
    {
        return _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}

