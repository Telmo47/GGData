using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace GGData.Services
{
    /// <summary>
    /// Serviço de envio de email falso para testes.
    /// Apenas regista no console em vez de enviar emails reais.
    /// </summary>
    public class FakeEmailSender : IEmailSender
    {
        /// <summary>
        /// Simula o envio de email registando no console.
        /// </summary>
        /// <param name="email">Email do destinatário</param>
        /// <param name="subject">Assunto do email</param>
        /// <param name="htmlMessage">Conteúdo HTML da mensagem</param>
        /// <returns>Tarefa concluída (Task.CompletedTask)</returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"[FakeEmailSender] Email para: {email}, Assunto: {subject}");
            return Task.CompletedTask;
        }
    }
}
