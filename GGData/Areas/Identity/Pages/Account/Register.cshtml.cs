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
    [AllowAnonymous] // Permite acesso à página sem autenticação
    public class RegisterModel : PageModel
    {
        // Serviços necessários para gerir utilizadores, login, envio de emails e logging
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IUserStore<Utilizadores> _userStore;
        private readonly IUserEmailStore<Utilizadores> _emailStore;
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        // Construtor que injeta os serviços
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

        // Propriedade ligada ao formulário para os dados de input
        [BindProperty]
        public InputModel Input { get; set; }

        // URL de retorno após registo bem-sucedido
        public string ReturnUrl { get; set; }

        // Lista de esquemas de autenticação externos disponíveis (ex: Google, Facebook)
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Modelo interno que representa os dados do formulário de registo
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
            public string TipoUsuario { get; set; } // Pode ser "Utilizador" ou "Critico"

            // Campos opcionais que apenas aplicam para críticos
            [Display(Name = "Instituição")]
            public string Instituicao { get; set; }

            [Display(Name = "Website Profissional")]
            public string WebsiteProfissional { get; set; }

            [Display(Name = "Descrição Profissional")]
            public string DescricaoProfissional { get; set; }
        }

        // Método executado quando a página é acedida via GET
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/"); // Define URL de retorno
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Carrega esquemas externos
        }

        // Método executado quando o formulário é submetido via POST
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // Define URL de retorno padrão
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Validação adicional para campos obrigatórios do tipo "Crítico"
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
                // Remove erros dos campos não aplicáveis para utilizadores normais
                ModelState.Remove("Input.Instituicao");
                ModelState.Remove("Input.WebsiteProfissional");
                ModelState.Remove("Input.DescricaoProfissional");
            }

            if (ModelState.IsValid)
            {
                // Cria um novo utilizador com os dados do formulário
                var user = new Utilizadores
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    DataRegistro = DateTime.UtcNow,
                    TipoUsuario = Input.TipoUsuario,
                    Instituicao = Input.TipoUsuario == "Critico" ? Input.Instituicao : null,
                    WebsiteProfissional = Input.TipoUsuario == "Critico" ? Input.WebsiteProfissional : null,
                    DescricaoProfissional = Input.TipoUsuario == "Critico" ? Input.DescricaoProfissional : null,
                };

                // Tenta criar o utilizador na base de dados com a password indicada
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Conta criada com sucesso.");

                    // Gera token para confirmação do email
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // Cria URL para confirmação do email
                    var callbackUrl = Url.Page("/Account/ConfirmEmail", null,
                        new { area = "Identity", userId, code, returnUrl },
                        Request.Scheme);

                    // Envia email para o utilizador com link de confirmação
                    await _emailSender.SendEmailAsync(Input.Email, "Confirmação de Conta",
                        $"Confirme a sua conta clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                    // Se o sistema exigir confirmação de conta antes do login, redireciona para página de confirmação
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });

                    // Caso contrário, faz login automático e redireciona para a URL de retorno
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // Se ocorreram erros na criação da conta, adiciona ao modelo para mostrar na view
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se houver erros, recarrega a página para mostrar mensagens
            return Page();
        }

        // Método para obter a store de email do utilizador e garantir suporte a email
        private IUserEmailStore<Utilizadores> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("A store de utilizadores tem de suportar email.");

            return (IUserEmailStore<Utilizadores>)_userStore;
        }
    }
}
