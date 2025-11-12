using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Wallet;

namespace Origami.API.Controllers
{
    [ApiController]
    public class WalletController : BaseController<WalletController>
    {

        private readonly IWalletService _walletService;

        public WalletController(ILogger<WalletController> logger, IWalletService walletService) : base(logger)
        {
            _walletService = walletService;
        }

        [HttpGet(ApiEndPointConstant.Wallet.WalletEndPoint)]
        [ProducesResponseType(typeof(GetWalletResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWallet(int id)
        {
            var response = await _walletService.GetWalletByUserId(id);
            return Ok(response);
        }
    }
}
