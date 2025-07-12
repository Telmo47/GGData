using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GGData.Models; // Usa a tua classe personalizada de utilizadores

namespace GGData.Areas.Identity.Pages.Account
{
    // Modelo da Razor Page para gerir o logout (terminar sessão) do utilizador
    public class LogoutModel : PageModel
    {
        // Serviços injetados: gestor de sessões e logger
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        // Construtor que recebe as dependências necessárias
        public LogoutModel(SignInManager<Utilizadores> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // Método chamado quando o formulário de logout é submetido via POST
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Termina a sessão do utilizador
            await _signInManager.SignOutAsync();

            // Regista no log que o utilizador efetuou logout
            _logger.LogInformation("Utilizador terminou sessão.");

            // Se houver URL de retorno, redireciona para essa página
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Caso contrário, recarrega a página atual
                return RedirectToPage();
            }
        }
    }
}
