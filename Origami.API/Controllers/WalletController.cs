using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.Wallet;
using System.Linq;

namespace Origami.API.Controllers;

[ApiController]
public class WalletController : BaseController<WalletController>
{
    private readonly IWalletService _walletService;
    private readonly IVnPayService _vnPayService;

    public WalletController(
        ILogger<WalletController> logger,
        IWalletService walletService,
        IVnPayService vnPayService)
        : base(logger)
    {
        _walletService = walletService;
        _vnPayService = vnPayService;
    }

    [Authorize]
    [HttpGet(ApiEndPointConstant.Wallet.MyWalletEndPoint)]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyWallet()
    {
        var response = await _walletService.GetMyWalletAsync();
        return Ok(response);
    }

    [Authorize]
    [HttpGet(ApiEndPointConstant.Wallet.MyTransactionsEndPoint)]
    [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTransactions()
    {
        var response = await _walletService.GetMyTransactionsAsync();
        return Ok(response);
    }

    [Authorize]
    [HttpPost(ApiEndPointConstant.Wallet.TopUpEndPoint)]
    [ProducesResponseType(typeof(TopUpResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request)
    {
        if (request == null || request.Amount <= 0)
        {
            return BadRequest(new { message = "Số tiền nạp không hợp lệ" });
        }

        var response = await _walletService.CreateTopUpTransactionAsync(request);
        return Ok(response);
    }

    [HttpPost(ApiEndPointConstant.Wallet.VnPayCallbackEndPoint)]
    [HttpGet(ApiEndPointConstant.Wallet.VnPayCallbackEndPoint)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VnPayCallback()
    {
        try
        {
            // VNPay có thể gửi callback qua GET hoặc POST
            var queryString = Request.QueryString.ToString().TrimStart('?');
            if (string.IsNullOrEmpty(queryString) && Request.HasFormContentType)
            {
                // Nếu là POST với form data
                var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                queryString = string.Join("&", formData.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            }

            var vnpayData = _vnPayService.ParseCallbackData(queryString);

            var success = await _walletService.ProcessVnPayCallbackAsync(vnpayData);

            if (success)
            {
                // Trả về HTML redirect cho mobile app hoặc web
                return Content(
                    "<html><head><meta http-equiv='refresh' content='0;url=origamitech://wallet/top-up-success'></head><body>Payment successful. Redirecting...</body></html>",
                    "text/html"
                );
            }
            else
            {
                return Content(
                    "<html><head><meta http-equiv='refresh' content='0;url=origamitech://wallet/top-up-failed'></head><body>Payment failed. Redirecting...</body></html>",
                    "text/html"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay callback");
            return Content(
                "<html><head><meta http-equiv='refresh' content='0;url=origamitech://wallet/top-up-failed'></head><body>Payment error. Redirecting...</body></html>",
                "text/html"
            );
        }
    }
}
