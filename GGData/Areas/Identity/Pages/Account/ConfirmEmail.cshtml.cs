using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Modelo da página de confirmação de email.
    /// </summary>
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;

        /// <summary>
        /// Construtor que injeta o UserManager para gerir utilizadores.
        /// </summary>
        /// <param name="userManager">Serviço UserManager para Utilizadores</param>
        public ConfirmEmailModel(UserManager<Utilizadores> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Mensagem temporária para mostrar o estado da confirmação (sucesso/erro).
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Método assíncrono chamado na requisição GET para confirmar o email.
        /// </summary>
        /// <param name="userId">ID do utilizador a confirmar</param>
        /// <param name="code">Código de confirmação codificado em base64url</param>
        /// <returns>Retorna a página com mensagem de sucesso ou erro</returns>
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            // Verifica se os parâmetros foram fornecidos, caso contrário redireciona para a página principal
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            // Procura o utilizador pelo ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Retorna erro caso não encontre o utilizador
                return NotFound($"Não foi possível carregar utilizador com ID '{userId}'.");
            }

            // Decodifica o código recebido de base64url para UTF8
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Tenta confirmar o email com o código decodificado
            var result = await _userManager.ConfirmEmailAsync(user, code);

            // Define a mensagem de estado consoante o resultado da confirmação
            StatusMessage = result.Succeeded ? "Obrigado por confirmares o teu email." : "Erro ao confirmar email.";

            // Mostra a página com a mensagem definida
            return Page();
        }
    }
}
