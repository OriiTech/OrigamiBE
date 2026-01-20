using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Email;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Threading;

namespace Origami.API.Services.Implement
{
    public class EmailService : BaseService<EmailService>, IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _templatesPath;

        public EmailService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<EmailService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
            _templatesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Templates",
                "Email"
            );

            // Tạo folder nếu chưa tồn tại
            if (!Directory.Exists(_templatesPath))
            {
                Directory.CreateDirectory(_templatesPath);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            await SendEmailAsync(toEmail, null, subject, htmlBody);
        }

        public async Task SendEmailAsync(string toEmail, string? toName, string subject, string htmlBody)
        {
            // Ưu tiên dùng SendGrid nếu có API key trong cấu hình / environment
            var sendGridApiKey = _configuration["SendGrid:ApiKey"] 
                ?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (!string.IsNullOrWhiteSpace(sendGridApiKey))
            {
                await SendEmailWithSendGridAsync(sendGridApiKey, toEmail, toName, subject, htmlBody);
                return;
            }

            // Fallback: dùng SMTP cũ (hữu ích cho môi trường local)
            await SendEmailWithSmtpAsync(toEmail, toName, subject, htmlBody);
        }

        private async Task SendEmailWithSmtpAsync(string toEmail, string? toName, string subject, string htmlBody)
        {
            try
            {
                var emailSettings = _configuration.GetSection("Authentication:EmailSettings");
                var smtpServer = emailSettings["SmtpServer"] ?? throw new InvalidOperationException("EmailSettings:SmtpServer is not configured");
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
                var fromEmail = emailSettings["FromEmail"] ?? throw new InvalidOperationException("EmailSettings:FromEmail is not configured");
                var smtpPassword = emailSettings["SmtpPassword"] ?? throw new InvalidOperationException("EmailSettings:SmtpPassword is not configured");
                var fromName = emailSettings["FromName"] ?? "Origami Tech Sharing";
                var enableSsl = emailSettings.GetValue<bool>("EnableSsl", true);

                // Gmail yêu cầu SmtpUsername phải là email đầy đủ, không phải tên
                var smtpUsername = fromEmail;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress(toName ?? toEmail, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.Timeout = 60000; // 60s

                await client.ConnectAsync(smtpServer, smtpPort, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"[SMTP] Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[SMTP] Failed to send email to {toEmail}");
                throw;
            }
        }

        private async Task SendEmailWithSendGridAsync(
            string apiKey,
            string toEmail,
            string? toName,
            string subject,
            string htmlBody)
        {
            var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL")
                ?? _configuration["SendGrid:FromEmail"]
                ?? _configuration["Authentication:EmailSettings:FromEmail"]
                ?? throw new InvalidOperationException("SENDGRID_FROM_EMAIL is not configured");

            var fromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME")
                ?? _configuration["SendGrid:FromName"]
                ?? _configuration["Authentication:EmailSettings:FromName"]
                ?? "Origami Tech Sharing";

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail, toName ?? toEmail);

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent: null,
                htmlContent: htmlBody);

            var response = await client.SendEmailAsync(msg);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                _logger.LogInformation($"[SendGrid] Email sent successfully to {toEmail} with status {response.StatusCode}");
                return;
            }

            var body = await response.Body.ReadAsStringAsync();
            _logger.LogError("[SendGrid] Failed to send email. Status: {StatusCode}, Body: {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"SendGrid gửi email thất bại với mã trạng thái {(int)response.StatusCode}.");
        }

        public async Task<string> RenderEmailTemplateAsync(string templateName, Dictionary<string, object> model)
        {
            var templatePath = Path.Combine(_templatesPath, $"{templateName}.html");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found: {templateName}");
            }

            var templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

            // Simple template replacement - thay thế {{Key}} bằng giá trị
            foreach (var item in model)
            {
                templateContent = templateContent.Replace($"{{{{{item.Key}}}}}", item.Value?.ToString() ?? "");
            }

            return templateContent;
        }

        public async Task<string> SendTestEmailAsync(TestEmailRequest request)
        {
            // Lấy User từ DB thông qua Email
            var user = await _unitOfWork.GetRepository<User>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Email.ToLower() == request.Email.ToLower(),
                    include: q => q.Include(u => u.UserProfile),
                    asNoTracking: true
                );

            // Lấy UserName: ưu tiên DisplayName từ UserProfile, nếu không có thì dùng Username
            var userName = user?.UserProfile?.DisplayName 
                ?? user?.Username 
                ?? request.Email.Split('@')[0]; // Fallback nếu không tìm thấy user
            
            // Tạo model cho template
            var model = new Dictionary<string, object>
            {
                { "UserName", userName },
                { "UserEmail", request.Email },
                { "CurrentTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
            };

            // Render template
            var htmlBody = await RenderEmailTemplateAsync("TemplateTest", model);

            // Gửi email
            var subject = "🎌 Test Email - Origami Tech Sharing";
            await SendEmailAsync(
                request.Email,
                userName,
                subject,
                htmlBody
            );

            return "Email sent successfully";
        }

        public async Task<string> SendFeedbackEmailAsync(FeedbackEmailRequest request)
        {
            // Lấy email từ JWT token của user đang đăng nhập
            var userEmail = GetEmailFromJwt();
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new UnauthorizedAccessException("User is not logged in. Please login first.");

            // Lấy UserId từ JWT token
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User ID not found in token.");

            // Lấy User từ DB thông qua UserId
            var user = await _unitOfWork.GetRepository<User>()
                .GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId.Value,
                    include: q => q.Include(u => u.UserProfile),
                    asNoTracking: true
                ) ?? throw new InvalidOperationException("User not found in database.");

            // Đảm bảo email từ JWT khớp với email trong DB
            if (!string.Equals(user.Email, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Email mismatch: JWT has {userEmail}, DB has {user.Email}");
                userEmail = user.Email ?? userEmail; // Ưu tiên email từ DB
            }

            // Lấy UserName: ưu tiên DisplayName từ UserProfile, nếu không có thì dùng Username
            var userName = user.UserProfile?.DisplayName 
                ?? user.Username 
                ?? userEmail.Split('@')[0];

            // Tạo HTML body cho feedback email
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1d9bf0;'>📧 Feedback từ Origami Mobile App</h2>
                    <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Người gửi:</strong> {userName} ({userEmail})</p>
                        <p><strong>Thời gian:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                    </div>
                    <div style='margin: 20px 0;'>
                        <h3 style='color: #333;'>Tiêu đề:</h3>
                        <p style='background-color: #fff; padding: 10px; border-left: 3px solid #1d9bf0;'>{request.Subject}</p>
                    </div>
                    <div style='margin: 20px 0;'>
                        <h3 style='color: #333;'>Nội dung:</h3>
                        <div style='background-color: #fff; padding: 15px; border-left: 3px solid #1d9bf0; white-space: pre-wrap;'>{request.Content}</div>
                    </div>
                    <hr style='border: none; border-top: 1px solid #ddd; margin: 30px 0;' />
                    <p style='color: #666; font-size: 12px;'>Email này được gửi tự động từ Origami Mobile App.</p>
                </body>
                </html>";

            // Gửi email đến địa chỉ feedback
            var feedbackEmail = _configuration["Authentication:EmailSettings:FromEmail"] ?? "origamihub678@gmail.com";
            var subject = $"📧 Feedback: {request.Subject}";
            
            await SendEmailAsync(
                feedbackEmail,
                "Origami Support",
                subject,
                htmlBody
            );

            return "Feedback sent successfully";
        }
    }
}

