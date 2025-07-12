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
    // Modelo da Razor Page para gerir o login de utilizadores
    public class LoginModel : PageModel
    {
        // Serviços injetados: gestor de login e logger
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        // Construtor que recebe as dependências
        public LoginModel(SignInManager<Utilizadores> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // Propriedade ligada ao formulário para captar dados de input
        [BindProperty]
        public InputModel Input { get; set; }

        // Lista de esquemas de autenticação externos (ex: Google, Facebook)
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // URL para redirecionar após login bem-sucedido
        public string ReturnUrl { get; set; }

        // Mensagem temporária para erros que pode ser passada entre requests
        [TempData]
        public string ErrorMessage { get; set; }

        // Classe interna para os dados do formulário de login
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")] // A tradução deste texto está na view (login.cshtml)
            public bool RememberMe { get; set; }
        }

        // Método chamado quando a página é acedida via GET
        public async Task OnGetAsync(string returnUrl = null)
        {
            // Se existir mensagem de erro, adiciona ao modelo para mostrar na página
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Define URL de retorno para raiz caso não seja especificada
            returnUrl ??= Url.Content("~/");

            // Limpa qualquer autenticação externa pendente
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Obtém os esquemas de login externos disponíveis
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Guarda a URL de retorno para uso na view
            ReturnUrl = returnUrl;
        }

        // Método chamado quando o formulário é submetido via POST
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Atualiza lista de esquemas externos (para reexibir a página em caso de erro)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Verifica se os dados do formulário são válidos
            if (ModelState.IsValid)
            {
                // Tenta efetuar login com email e password fornecidos
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilizador autenticado com sucesso.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    // Se o login exigir autenticação de dois fatores, redireciona para essa página
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    // Se a conta estiver bloqueada, regista no log e redireciona para página de lockout
                    _logger.LogWarning("Conta do utilizador bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Em caso de falha no login, adiciona erro ao modelo para mostrar na página
                    ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
                    return Page();
                }
            }

            // Se os dados do formulário forem inválidos, simplesmente recarrega a página com erros
            return Page();
        }
    }
}
