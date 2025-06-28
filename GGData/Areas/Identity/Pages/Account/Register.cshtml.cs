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
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly IUserStore<Usuarios> _userStore;
        private readonly IUserEmailStore<Usuarios> _emailStore;
        private readonly SignInManager<Usuarios> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Usuarios> userManager,
            IUserStore<Usuarios> userStore,
            SignInManager<Usuarios> signInManager,
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

            // Campos opcionais para cr�ticos
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

            if (Input.TipoUsuario == "Critico")
            {
                if (string.IsNullOrWhiteSpace(Input.Instituicao))
                    ModelState.AddModelError("Input.Instituicao", "A institui��o � obrigat�ria para cr�ticos.");
                if (string.IsNullOrWhiteSpace(Input.WebsiteProfissional))
                    ModelState.AddModelError("Input.WebsiteProfissional", "O website profissional � obrigat�rio para cr�ticos.");
                if (string.IsNullOrWhiteSpace(Input.DescricaoProfissional))
                    ModelState.AddModelError("Input.DescricaoProfissional", "A descri��o profissional � obrigat�ria para cr�ticos.");
            }
            else
            {
                // Remove os erros associados aos campos de cr�ticos
                ModelState.Remove("Input.Instituicao");
                ModelState.Remove("Input.WebsiteProfissional");
                ModelState.Remove("Input.DescricaoProfissional");
            }

            if (ModelState.IsValid)
            {
                var user = new Usuarios
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
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IUserEmailStore<Usuarios> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("A store de utilizadores tem de suportar email.");

            return (IUserEmailStore<Usuarios>)_userStore;
        }
    }
}
