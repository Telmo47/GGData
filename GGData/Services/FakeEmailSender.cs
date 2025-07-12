using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace GGData.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Aqui podes só fazer log para testes
            Console.WriteLine($"[FakeEmailSender] Email para: {email}, Assunto: {subject}");
            return Task.CompletedTask;
        }
    }
}
