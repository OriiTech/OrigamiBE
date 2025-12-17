using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;

namespace Origami.API.Controllers;

[ApiController]
[Authorize]
public class WalletController : BaseController<WalletController>
{
    private readonly IWalletService _walletService;

    public WalletController(ILogger<WalletController> logger, IWalletService walletService) : base(logger)
    {
        _walletService = walletService;
    }

    [HttpGet(ApiEndPointConstant.Wallet.MyWalletEndPoint)]
    [ProducesResponseType(typeof(GetWalletResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyWallet()
    {
        var response = await _walletService.GetMyWallet();
        return Ok(response);
    }

    [HttpGet(ApiEndPointConstant.Wallet.WalletEndPoint)]
    [ProducesResponseType(typeof(GetWalletResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWalletById(int id)
    {
        var response = await _walletService.GetWalletById(id);
        return Ok(response);
    }

    [HttpGet(ApiEndPointConstant.Wallet.TransactionsEndPoint)]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTransactions([FromQuery] MyTransactionFilter filter, [FromQuery] PagingModel pagingModel)
    {
        var response = await _walletService.GetMyTransactions(filter, pagingModel);
        return Ok(response);
    }

    [HttpGet(ApiEndPointConstant.Wallet.TransactionsEndPoint + "/all")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionFilter filter, [FromQuery] PagingModel pagingModel)
    {
        var response = await _walletService.GetAllTransactions(filter, pagingModel);
        return Ok(response);
    }
}

