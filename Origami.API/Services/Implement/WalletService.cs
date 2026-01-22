using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement;

public class WalletService : BaseService<WalletService>, IWalletService
{
    private readonly IVnPayService _vnPayService;

    public WalletService(
        IUnitOfWork<OrigamiDbContext> unitOfWork,
        ILogger<WalletService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IVnPayService vnPayService)
        : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _vnPayService = vnPayService;
    }

    public async Task<WalletResponse> GetMyWalletAsync()
    {
        int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            asNoTracking: true
        );

        if (wallet == null)
        {
            // Tạo wallet mới nếu chưa có
            var newWallet = new Wallet
            {
                UserId = userId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await walletRepo.InsertAsync(newWallet);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<WalletResponse>(newWallet);
        }

        return _mapper.Map<WalletResponse>(wallet);
    }

    public async Task<List<TransactionResponse>> GetMyTransactionsAsync()
    {
        int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId,
            asNoTracking: true
        );

        if (wallet == null)
        {
            return new List<TransactionResponse>();
        }

        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        var transactions = await transactionRepo.GetListAsync(
            predicate: x => x.SenderWalletId == wallet.WalletId || x.ReceiverWalletId == wallet.WalletId,
            orderBy: q => q.OrderByDescending(x => x.CreatedAt),
            asNoTracking: true
        );

        return transactions.Select(x => _mapper.Map<TransactionResponse>(x)).ToList();
    }

    public async Task<TopUpResponse> CreateTopUpTransactionAsync(TopUpRequest request)
    {
        int userId = GetCurrentUserId() ?? throw new BadHttpRequestException("Unauthorized");

        var walletRepo = _unitOfWork.GetRepository<Wallet>();
        var wallet = await walletRepo.GetFirstOrDefaultAsync(
            predicate: x => x.UserId == userId
        );

        if (wallet == null)
        {
            // Tạo wallet mới nếu chưa có
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

        // Tạo transaction với status "Pending"
        var transaction = new Transaction
        {
            ReceiverWalletId = wallet.WalletId,
            Amount = request.Amount,
            TransactionType = "TopUp",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        await transactionRepo.InsertAsync(transaction);
        await _unitOfWork.CommitAsync();

        // Lấy IP address từ request (hỗ trợ X-Forwarded-For từ proxy)
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = "127.0.0.1";
        
        if (httpContext != null)
        {
            // Kiểm tra X-Forwarded-For header (từ proxy như Render)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                ipAddress = forwardedFor.Split(',')[0].Trim();
            }
            else
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            }
        }

        // Tạo payment URL từ VNPay
        var paymentUrl = _vnPayService.CreatePaymentUrl(transaction.TransactionId, request.Amount, ipAddress);

        return new TopUpResponse
        {
            PaymentUrl = paymentUrl,
            TransactionId = transaction.TransactionId
        };
    }

    public async Task<bool> ProcessVnPayCallbackAsync(Dictionary<string, string> vnpayData)
    {
        if (!vnpayData.ContainsKey("vnp_SecureHash") || !vnpayData.ContainsKey("vnp_TxnRef"))
        {
            return false;
        }

        var vnp_SecureHash = vnpayData["vnp_SecureHash"];
        var vnp_ResponseCode = vnpayData.GetValueOrDefault("vnp_ResponseCode", "");

        // Validate signature
        if (!_vnPayService.ValidateSignature(vnpayData, vnp_SecureHash))
        {
            _logger.LogWarning("VNPay callback signature validation failed");
            return false;
        }

        // Check response code (00 = success)
        if (vnp_ResponseCode != "00")
        {
            _logger.LogWarning($"VNPay payment failed with response code: {vnp_ResponseCode}");
            return false;
        }

        // Get transaction ID
        if (!int.TryParse(vnpayData["vnp_TxnRef"], out int transactionId))
        {
            _logger.LogWarning("Invalid transaction ID in VNPay callback");
            return false;
        }

        // Get amount (VNPay returns in cents)
        if (!long.TryParse(vnpayData.GetValueOrDefault("vnp_Amount", "0"), out long amountInCents))
        {
            _logger.LogWarning("Invalid amount in VNPay callback");
            return false;
        }

        var amount = amountInCents / 100m;

        // Update transaction status
        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        var transaction = await transactionRepo.GetFirstOrDefaultAsync(
            predicate: x => x.TransactionId == transactionId
        );

        if (transaction == null)
        {
            _logger.LogWarning($"Transaction {transactionId} not found");
            return false;
        }

        // Nếu đã xử lý rồi thì không xử lý lại
        if (transaction.Status == "Success")
        {
            return true;
        }

        // Update transaction status
        transaction.Status = "Success";
        transaction.CreatedAt = DateTime.UtcNow;

        // Update wallet balance
        if (transaction.ReceiverWalletId.HasValue)
        {
            var walletRepo = _unitOfWork.GetRepository<Wallet>();
            var wallet = await walletRepo.GetFirstOrDefaultAsync(
                predicate: x => x.WalletId == transaction.ReceiverWalletId.Value
            );

            if (wallet != null)
            {
                wallet.Balance = (wallet.Balance ?? 0) + amount;
                wallet.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.CommitAsync();

        _logger.LogInformation($"Successfully processed VNPay callback for transaction {transactionId}");
        return true;
    }
}
