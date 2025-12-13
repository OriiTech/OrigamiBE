using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Npgsql;

namespace Origami.API.Services.Implement;

public class WalletService : BaseService<WalletService>, IWalletService
{
    private readonly IConfiguration _configuration;

    public WalletService(
        IUnitOfWork<OrigamiDbContext> unitOfWork,
        ILogger<WalletService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    ) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<GetWalletResponse> GetMyWallet()
    {
        var userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
        
        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            asNoTracking: true
        );

        // Tạo wallet nếu chưa có
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
                // Duplicate key - có thể do race condition hoặc duplicate user_id
                // Thử load lại wallet
                _logger.LogWarning(ex, "Duplicate key error when creating wallet for user {UserId}, attempting to reload", userId);
                
                wallet = await walletRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId,
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException("FailedToCreateWallet");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating wallet for user {UserId}", userId);
                throw new BadHttpRequestException("FailedToCreateWallet");
            }
        }

        return _mapper.Map<GetWalletResponse>(wallet);
    }

    public async Task<GetWalletResponse> GetWalletById(int id)
    {
        var wallet = await _unitOfWork.GetRepository<Wallet>().GetFirstOrDefaultAsync(
            predicate: x => x.WalletId == id,
            asNoTracking: true
        ) ?? throw new BadHttpRequestException("WalletNotFound");

        return _mapper.Map<GetWalletResponse>(wallet);
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
                // Duplicate key - có thể do race condition hoặc duplicate user_id
                // Thử load lại wallet
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
            Amount = request.Amount, // Số tiền VND
            TransactionType = "deposit", // Sử dụng "deposit" thay vì "TOP_UP" để phù hợp với database constraint
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
        // Verify signature từ VNPAY
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

            // Nạp tiền: 1 VND = 1 xu
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

    public async Task<IPaginate<TransactionResponse>> GetMyTransactions(TransactionFilter filter, PagingModel pagingModel)
    {
        var userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");
        
        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            asNoTracking: true
        );

        if (wallet == null)
            throw new BadHttpRequestException("WalletNotFound");

        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        
        var response = await transactionRepo.GetPagingListAsync(
            selector: x => _mapper.Map<TransactionResponse>(x),
            predicate: x => (x.SenderWalletId == wallet.WalletId || x.ReceiverWalletId == wallet.WalletId) &&
                           (filter.TransactionType == null || x.TransactionType == filter.TransactionType) &&
                           (filter.Status == null || x.Status == filter.Status) &&
                           (filter.FromDate == null || x.CreatedAt >= filter.FromDate) &&
                           (filter.ToDate == null || x.CreatedAt <= filter.ToDate),
            orderBy: x => x.OrderByDescending(t => t.CreatedAt),
            include: q => q.Include(t => t.SenderWallet).Include(t => t.ReceiverWallet),
            page: pagingModel.page,
            size: pagingModel.size
        );

        return response;
    }

    public async Task<IPaginate<TransactionResponse>> GetAllTransactions(TransactionFilter filter, PagingModel pagingModel)
    {
        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        
        var response = await transactionRepo.GetPagingListAsync(
            selector: x => _mapper.Map<TransactionResponse>(x),
            predicate: x => (filter.WalletId == null || x.SenderWalletId == filter.WalletId || x.ReceiverWalletId == filter.WalletId) &&
                           (filter.TransactionType == null || x.TransactionType == filter.TransactionType) &&
                           (filter.Status == null || x.Status == filter.Status) &&
                           (filter.FromDate == null || x.CreatedAt >= filter.FromDate) &&
                           (filter.ToDate == null || x.CreatedAt <= filter.ToDate),
            orderBy: x => x.OrderByDescending(t => t.CreatedAt),
            include: q => q.Include(t => t.SenderWallet).Include(t => t.ReceiverWallet),
            page: pagingModel.page,
            size: pagingModel.size
        );

        return response;
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
            ["vnp_Amount"] = ((long)(amount * 100)).ToString(), // VNPAY yêu cầu số tiền * 100
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = transactionId.ToString(),
            ["vnp_OrderInfo"] = $"Nạp tiền vào ví - Transaction {transactionId}",
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl ?? returnUrlConfig ?? "",
            ["vnp_IpAddr"] = GetIpAddress(),
            ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss")
        };

        // Sắp xếp và tạo query string
        var queryString = string.Join("&", vnp_Params
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        // Tạo hash
        var signData = queryString;
        if (!string.IsNullOrEmpty(hashSecret))
        {
            signData += $"&{hashSecret}";
        }
        var vnp_SecureHash = HmacSHA512(hashSecret ?? "", signData);
        queryString += $"&vnp_SecureHash={vnp_SecureHash}";

        return $"{url}?{queryString}";
    }

    private bool VerifyVnpaySignature(Dictionary<string, string> vnpayData)
    {
        var vnpayConfig = _configuration.GetSection("VNPay");
        var hashSecret = vnpayConfig["HashSecret"];

        if (!vnpayData.ContainsKey("vnp_SecureHash"))
            return false;

        var vnp_SecureHash = vnpayData["vnp_SecureHash"];
        vnpayData.Remove("vnp_SecureHash");
        vnpayData.Remove("vnp_SecureHashType");

        var signData = string.Join("&", vnpayData
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        if (!string.IsNullOrEmpty(hashSecret))
        {
            signData += $"&{hashSecret}";
        }

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

