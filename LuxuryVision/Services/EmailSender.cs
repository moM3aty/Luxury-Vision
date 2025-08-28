using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LuxuryVision.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Debug.WriteLine("=====================================");
            Debug.WriteLine($"To: {email}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine("Message (HTML):");
            Debug.WriteLine(htmlMessage);
            Debug.WriteLine("=====================================");

            return Task.CompletedTask;
        }
    }
}