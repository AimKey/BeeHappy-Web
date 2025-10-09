using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml);
        // Template emails
        Task SendWelcomeEmailAsync(string toEmail, string username);
        Task SendPremiumConfirmationAsync(string toEmail, string username, DateTime expireAt);
    }
}
