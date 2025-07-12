using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    [AllowAnonymous] // Permite que utilizadores anónimos acedam à página de registo
    public class RegisterModel : PageModel
    {
        // Serviços essenciais para gerir utilizadores, autenticação, logging e envio de emails
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IUserStore<Utilizadores> _userStore;
        private readonly IUserEmailStore<Utilizadores> _emailStore;
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        // Construtor que injeta as dependências necessárias
        public RegisterModel(
            UserManager<Utilizadores> userManager,
            IUserStore<Utilizadores> userStore,
            SignInManager<Utilizadores> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // Propriedade ligada ao formulário, representa os dados de entrada
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; } // URL para onde redirecionar após registo
        public IList<AuthenticationScheme> ExternalLogins { get; set; } // Logins externos (ex: Google, Facebook)

        /// <summary>
        /// Modelo para capturar os dados do formulário de registo.
        /// Inclui validações, mensagens de erro personalizadas e campos opcionais para críticos.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(150)]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Tem de inserir um email válido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A password é obrigatória.")]
            [StringLength(20, MinimumLength = 6, ErrorMessage = "A password tem de ter entre 6 e 20 caracteres.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O tipo de utilizador é obrigatório.")]
            [Display(Name = "Tipo de utilizador")]
            public string TipoUsuario { get; set; } // Valores possíveis: "Utilizador" ou "Critico"

            // Campos opcionais para utilizadores do tipo "Crítico"
            [Display(Name = "Instituição")]
            public string Instituicao { get; set; }

            [Display(Name = "Website Profissional")]
            public string WebsiteProfissional { get; set; }

            [Display(Name = "Descrição Profissional")]
            public string DescricaoProfissional { get; set; }
        }

        /// <summary>
        /// Método GET executado ao carregar a página.
        /// Inicializa a URL de retorno e lista de logins externos.
        /// </summary>
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /// <summary>
        /// Método POST que processa o registo do utilizador.
        /// Valida os dados, cria o utilizador, envia email de confirmação e faz login automático (se aplicável).
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Valida campos obrigatórios extras para tipo "Crítico"
            if (Input.TipoUsuario == "Critico")
            {
                if (string.IsNullOrWhiteSpace(Input.Instituicao))
                    ModelState.AddModelError("Input.Instituicao", "A instituição é obrigatória para críticos.");
                if (string.IsNullOrWhiteSpace(Input.WebsiteProfissional))
                    ModelState.AddModelError("Input.WebsiteProfissional", "O website profissional é obrigatório para críticos.");
                if (string.IsNullOrWhiteSpace(Input.DescricaoProfissional))
                    ModelState.AddModelError("Input.DescricaoProfissional", "A descrição profissional é obrigatória para críticos.");
            }
            else
            {
                // Remove erros relacionados aos campos de críticos se o utilizador não for crítico
                ModelState.Remove("Input.Instituicao");
                ModelState.Remove("Input.WebsiteProfissional");
                ModelState.Remove("Input.DescricaoProfissional");
            }

            // Se os dados forem válidos, cria o utilizador
            if (ModelState.IsValid)
            {
                var user = new Utilizadores
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    DataRegistro = DateTime.Now,
                    TipoUsuario = Input.TipoUsuario,
                    Instituicao = Input.TipoUsuario == "Critico" ? Input.Instituicao : null,
                    WebsiteProfissional = Input.TipoUsuario == "Critico" ? Input.WebsiteProfissional : null,
                    DescricaoProfissional = Input.TipoUsuario == "Critico" ? Input.DescricaoProfissional : null,
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Conta criada com sucesso.");

                    // Gera token para confirmação do email
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ConfirmEmail", null,
                        new { area = "Identity", userId, code, returnUrl },
                        Request.Scheme);

                    // Envia email de confirmação ao utilizador
                    await _emailSender.SendEmailAsync(Input.Email, "Confirmação de Conta",
                        $"Confirme a sua conta clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                    // Se for obrigatório confirmar a conta antes de login, redireciona para página de confirmação
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });

                    // Caso contrário, faz login automático do utilizador
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // Se houver erros na criação, adiciona-os ao modelo para mostrar ao utilizador
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se houver erros de validação ou criação, volta a mostrar a página com mensagens de erro
            return Page();
        }

        /// <summary>
        /// Retorna a store de email para o UserManager,
        /// garantindo que o sistema suporta emails.
        /// </summary>
        private IUserEmailStore<Utilizadores> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("A store de utilizadores tem de suportar email.");

            return (IUserEmailStore<Utilizadores>)_userStore;
        }
    }
}
