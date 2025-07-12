using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GGData.Models; // Classe personalizada de utilizadores

namespace GGData.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Modelo para gerir o logout (terminar sessão) dos utilizadores.
    /// </summary>
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        /// <summary>
        /// Construtor que injeta os serviços necessários para logout e logging.
        /// </summary>
        /// <param name="signInManager">Serviço para gerir sessões de utilizador.</param>
        /// <param name="logger">Serviço de logging para registar eventos.</param>
        public LogoutModel(SignInManager<Utilizadores> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Método que processa o pedido POST para terminar a sessão do utilizador.
        /// </summary>
        /// <param name="returnUrl">URL para redirecionar após logout (opcional).</param>
        /// <returns>Redireciona para a página indicada ou para a página atual.</returns>
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Termina a sessão do utilizador
            await _signInManager.SignOutAsync();

            // Regista no log que o utilizador terminou sessão
            _logger.LogInformation("Utilizador terminou sessão.");

            // Redireciona para a URL de retorno se especificada, senão recarrega a página
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
