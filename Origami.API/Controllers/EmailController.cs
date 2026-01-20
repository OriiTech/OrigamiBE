using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.Email;

namespace Origami.API.Controllers
{
    [Route(ApiEndPointConstant.ApiEndpoint)]
    [ApiController]
    public class EmailController : BaseController<EmailController>
    {
        private readonly IEmailService _emailService;

        public EmailController(
            ILogger<EmailController> logger, 
            IEmailService emailService) 
            : base(logger)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Test gửi email với TemplateTest
        /// </summary>
        /// <param name="request">Thông tin email test</param>
        /// <returns>Kết quả gửi email</returns>
        [HttpPost("/api/v1/emails/test")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            var response = await _emailService.SendTestEmailAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Gửi feedback email từ mobile app
        /// </summary>
        /// <param name="request">Thông tin feedback</param>
        /// <returns>Kết quả gửi email</returns>
        [HttpPost("/api/v1/emails/feedback")]
        [Authorize] // Yêu cầu user đã đăng nhập
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendFeedback([FromBody] FeedbackEmailRequest request)
        {
            var response = await _emailService.SendFeedbackEmailAsync(request);
            return Ok(response);
        }
    }
}

