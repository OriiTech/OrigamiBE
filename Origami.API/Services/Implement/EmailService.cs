using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.Email;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
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
                
                // Set timeout to 60 seconds for SMTP operations (longer for Render network)
                client.Timeout = 60000; // 60 seconds in milliseconds
                
                // Connect with timeout and retry logic
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                
                try
                {
                    await client.ConnectAsync(smtpServer, smtpPort, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cts.Token);
                }
                catch (Exception connectEx)
                {
                    _logger.LogError(connectEx, $"Failed to connect to SMTP server {smtpServer}:{smtpPort}");
                    throw new InvalidOperationException($"Không thể kết nối đến SMTP server. Vui lòng thử lại sau.", connectEx);
                }
                
                try
                {
                    await client.AuthenticateAsync(smtpUsername, smtpPassword, cts.Token);
                }
                catch (Exception authEx)
                {
                    _logger.LogError(authEx, $"Failed to authenticate with SMTP server");
                    await client.DisconnectAsync(true, CancellationToken.None);
                    throw new InvalidOperationException($"Xác thực SMTP thất bại. Vui lòng kiểm tra lại thông tin đăng nhập.", authEx);
                }
                
                try
                {
                    await client.SendAsync(message, cts.Token);
                }
                catch (Exception sendEx)
                {
                    _logger.LogError(sendEx, $"Failed to send email");
                    await client.DisconnectAsync(true, CancellationToken.None);
                    throw new InvalidOperationException($"Không thể gửi email. Vui lòng thử lại sau.", sendEx);
                }
                
                await client.DisconnectAsync(true, CancellationToken.None);

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
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
            // Lấy User từ DB thông qua Email (nếu có)
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

            // Tạo HTML body cho feedback email
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1d9bf0;'>📧 Feedback từ Origami Mobile App</h2>
                    <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Người gửi:</strong> {userName} ({request.Email})</p>
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

