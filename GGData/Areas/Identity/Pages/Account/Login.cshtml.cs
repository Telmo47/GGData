using GGData.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Página e modelo para iniciar sessão (login).
    /// Gere o login local e external providers.
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<Utilizadores> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// Propriedade para ligação de dados dos inputs do formulário.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// Lista dos esquemas de autenticação externos disponíveis.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// URL para redirecionamento após login.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Mensagem temporária para erros.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Modelo interno para representar os dados do formulário.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Por favor insira um email válido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Lembrar-me")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// Método chamado na requisição GET para preparar a página de login.
        /// </summary>
        /// <param name="returnUrl">URL para redirecionar depois do login</param>
        /// <returns></returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            // Se houver mensagem de erro, adiciona ao ModelState para mostrar no formulário
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Se não houver returnUrl, define a homepage como padrão
            returnUrl ??= Url.Content("~/");

            // Limpa cookies de autenticação externa
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Carrega os esquemas externos (ex: Google, Facebook)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Método chamado no POST ao submeter o formulário de login.
        /// Valida as credenciais e processa o login.
        /// </summary>
        /// <param name="returnUrl">URL para redirecionar depois do login</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Atualiza lista de provedores externos (caso necessário)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Tenta autenticar o utilizador com email e palavra-passe
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilizador autenticado com sucesso.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Conta de utilizador bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Tentativa de login inválida, adiciona erro para feedback
                    ModelState.AddModelError(string.Empty, "Tentativa de autenticação inválida.");
                    return Page();
                }
            }

            // Se o ModelState for inválido, volta a mostrar o formulário
            return Page();
        }
    }
}
