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
                await client.ConnectAsync(smtpServer, smtpPort, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

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
    }
}

