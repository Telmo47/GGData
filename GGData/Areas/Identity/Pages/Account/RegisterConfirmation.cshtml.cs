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
    /// Modelo para a página de confirmação de registo.
    /// Mostra informação sobre a confirmação do email e, em ambiente de desenvolvimento,
    /// disponibiliza o link direto para confirmar a conta.
    /// </summary>
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;

        /// <summary>
        /// Construtor que injeta o UserManager para gerir utilizadores.
        /// </summary>
        /// <param name="userManager">Gestor de utilizadores</param>
        public RegisterConfirmationModel(UserManager<Utilizadores> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Email do utilizador a confirmar.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Indica se o link direto de confirmação deve ser mostrado (apenas em ambiente de desenvolvimento).
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        /// URL do link de confirmação de email.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        /// <summary>
        /// Método GET executado ao aceder à página.
        /// Prepara dados para mostrar a confirmação.
        /// </summary>
        /// <param name="email">Email do utilizador</param>
        /// <param name="returnUrl">URL para redirecionamento após confirmação (opcional)</param>
        /// <returns>Página a renderizar</returns>
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                // Redireciona para a página inicial se o email não for fornecido
                return RedirectToPage("/Index");
            }

            // Procura o utilizador pelo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Erro se não encontrar o utilizador
                return NotFound($"Não foi possível carregar utilizador com o email '{email}'.");
            }

            Email = email;

            // Ativa a visualização do link direto (usar true apenas para desenvolvimento)
            DisplayConfirmAccountLink = true;

            if (DisplayConfirmAccountLink)
            {
                // Gera o token para confirmação de email e cria a URL para confirmação
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}
