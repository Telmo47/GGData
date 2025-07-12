using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System;

namespace GGData.Services
{
    /// <summary>
    /// Implementação simples de um serviço de envio de email para testes.
    /// Não envia emails reais, apenas regista a tentativa no console.
    /// </summary>
    public class FakeEmailSender : IEmailSender
    {
        /// <summary>
        /// Simula o envio de email, escrevendo os detalhes no console.
        /// </summary>
        /// <param name="email">Email do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="htmlMessage">Conteúdo HTML do email.</param>
        /// <returns>Tarefa concluída imediatamente.</returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Apenas faz log da tentativa de envio para testes
            Console.WriteLine($"[FakeEmailSender] Email para: {email}, Assunto: {subject}");
            return Task.CompletedTask;
        }
    }
}
