using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.Wallet;

namespace Origami.API.Controllers;

[ApiController]
public class VnPayController : BaseController<VnPayController>
{
    private readonly IVnPayService _vnPayService;

    public VnPayController(ILogger<VnPayController> logger, IVnPayService vnPayService) : base(logger)
    {
        _vnPayService = vnPayService;
    }

    [HttpPost(ApiEndPointConstant.VnPay.TopUpEndPoint)]
    [Authorize]
    [ProducesResponseType(typeof(TopUpResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request)
    {
        var response = await _vnPayService.TopUpWallet(request);
        return Ok(response);
    }

    [HttpGet(ApiEndPointConstant.VnPay.CallbackEndPoint)]
    [AllowAnonymous]
    public async Task<IActionResult> Callback()
    {
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var result = await _vnPayService.ProcessVnpayCallback(queryParams);

        if (result)
            return Ok(new { success = true, message = "Payment successful" });
        else
            return Ok(new { success = false, message = "Payment failed" });
    }
}

