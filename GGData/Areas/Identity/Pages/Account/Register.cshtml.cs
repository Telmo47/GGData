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
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome � obrigat�rio.")]
            [StringLength(150)]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O email � obrigat�rio.")]
            [EmailAddress(ErrorMessage = "Tem de inserir um email v�lido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A password � obrigat�ria.")]
            [StringLength(20, MinimumLength = 6, ErrorMessage = "A password tem de ter entre 6 e 20 caracteres.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "As passwords n�o coincidem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O tipo de utilizador � obrigat�rio.")]
            [Display(Name = "Tipo de utilizador")]
            public string TipoUsuario { get; set; } // "Normal" ou "Critico"

            // Campos exclusivos para cr�ticos (opcionais para utilizadores normais)
            [Display(Name = "Institui��o")]
            public string Instituicao { get; set; }

            [Display(Name = "Website Profissional")]
            public string WebsiteProfissional { get; set; }

            [Display(Name = "Descri��o Profissional")]
            public string DescricaoProfissional { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Valida��o extra para campos de cr�ticos
            if (Input.TipoUsuario == "Critico")
            {
                if (string.IsNullOrWhiteSpace(Input.Instituicao))
                    ModelState.AddModelError("Input.Instituicao", "A institui��o � obrigat�ria para cr�ticos.");
                if (string.IsNullOrWhiteSpace(Input.WebsiteProfissional))
                    ModelState.AddModelError("Input.WebsiteProfissional", "O website profissional � obrigat�rio para cr�ticos.");
                if (string.IsNullOrWhiteSpace(Input.DescricaoProfissional))
                    ModelState.AddModelError("Input.DescricaoProfissional", "A descri��o profissional � obrigat�ria para cr�ticos.");
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Conta criada com sucesso.");

                    var novoUsuario = new Usuarios
                    {
                        Nome = Input.Nome,
                        Email = Input.Email,
                        DataRegistro = DateTime.Now,
                        TipoUsuario = Input.TipoUsuario,
                        UserName = user.UserName
                    };

                    if (Input.TipoUsuario == "Critico")
                    {
                        novoUsuario.Instituicao = Input.Instituicao;
                        novoUsuario.WebsiteProfissional = Input.WebsiteProfissional;
                        novoUsuario.DescricaoProfissional = Input.DescricaoProfissional;
                        // Aqui podes adicionar flags para indicar que o cr�tico est� pendente de verifica��o
                    }

                    bool haErro = false;
                    try
                    {
                        _context.Usuarios.Add(novoUsuario);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        haErro = true;

                        // Apagar o utilizador Identity criado pois a BD falhou
                        await _userManager.DeleteAsync(user);

                        // Registar o erro (log)
                        _logger.LogError(ex, "Erro ao guardar dados do utilizador na BD");

                        ModelState.AddModelError(string.Empty, "Ocorreu um erro interno. Por favor, tenta novamente mais tarde.");
                    }

                    if (!haErro)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page("/Account/ConfirmEmail", null,
                            new { area = "Identity", userId, code, returnUrl },
                            Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirma��o de Conta",
                            $"Confirme a sua conta clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se chegar aqui, houve erro, mostrar a p�gina com mensagens de erro
            return Page();
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("A store de utilizadores tem de suportar email.");

            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
