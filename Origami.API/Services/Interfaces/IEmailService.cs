using Origami.BusinessTier.Payload.Email;

namespace Origami.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendEmailAsync(string toEmail, string? toName, string subject, string htmlBody);
        Task<string> RenderEmailTemplateAsync(string templateName, Dictionary<string, object> model);
        Task<string> SendTestEmailAsync(TestEmailRequest request);
        Task<string> SendFeedbackEmailAsync(FeedbackEmailRequest request);
    }
}

