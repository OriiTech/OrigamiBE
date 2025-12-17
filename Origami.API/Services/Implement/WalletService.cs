using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using Npgsql;
using System.Text;

namespace Origami.API.Services.Implement;

public class WalletService : BaseService<WalletService>, IWalletService
{
    public WalletService(
        IUnitOfWork<OrigamiDbContext> unitOfWork,
        ILogger<WalletService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    ) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
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

    public async Task<IPaginate<TransactionResponse>> GetMyTransactions(MyTransactionFilter filter, PagingModel pagingModel)
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

}

