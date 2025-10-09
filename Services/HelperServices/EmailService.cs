
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Services.HelperServices
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _senderEmail = configuration["EmailSettings:SenderEmail"] ?? "noreply@yourapp.com";
            _senderName = configuration["EmailSettings:SenderName"] ?? "BeeHappy App";

            _smtpClient = new SmtpClient
            {
                Host = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com",
                Port = int.Parse(configuration["EmailSettings:Port"] ?? "587"),
                EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"] ?? "true"),
                Credentials = new NetworkCredential(
                    configuration["EmailSettings:Username"],
                    configuration["EmailSettings:Password"]
                )
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            mail.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mail);
        }
        // ================= TEMPLATE EMAIL =================

        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            string subject = "🎉 Chào mừng bạn đến với BeeHappy App!";
            string bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; padding:20px;'>
                    <h2>Xin chào {username},</h2>
                    <p>Cảm ơn bạn đã đăng ký BeeHappy App 🎉</p>
                    <p>Hãy bắt đầu trải nghiệm ngay bây giờ!</p>
                    <br/>
                    <footer style='font-size:12px;color:gray'>BeeHappy Team</footer>
                </div>
            ";

            await SendEmailAsync(toEmail, subject, bodyHtml);
        }

        public async Task SendPremiumConfirmationAsync(string toEmail, string username, DateTime expireAt)
        {
            string subject = "✨ BeeHappy Premium Activated!";
            string bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; padding:20px;'>
                    <h2>Xin chào {username},</h2>
                    <p>Bạn đã nâng cấp thành công <b>BeeHappy Premium</b>! 🎉</p>
                    <p>Thời hạn premium của bạn sẽ kết thúc vào: <b>{expireAt:dd/MM/yyyy}</b>.</p>
                    <p>Cảm ơn bạn đã tin tưởng BeeHappy ❤️</p>
                    <br/>
                    <footer style='font-size:12px;color:gray'>BeeHappy Team</footer>
                </div>
            ";

            await SendEmailAsync(toEmail, subject, bodyHtml);
        }
    }
}
