using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    // Modelo para a página que confirma o registo de um novo utilizador
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;

        // Injeção do UserManager para gerir utilizadores
        public RegisterConfirmationModel(UserManager<Utilizadores> userManager)
        {
            _userManager = userManager;
        }

        // Propriedades usadas na view
        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; } // Controla se o link direto aparece
        public string EmailConfirmationUrl { get; set; }   // URL para confirmar email

        // Método chamado via GET, recebe o email e URL de retorno
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                // Redireciona para página inicial se o email não for fornecido
                return RedirectToPage("/Index");
            }

            // Procura o utilizador pelo email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Se não encontrado, retorna erro 404
                return NotFound($"Não foi possível carregar utilizador com o email '{email}'.");
            }

            Email = email;

            // Em ambiente de desenvolvimento, permite mostrar o link direto para confirmação
            DisplayConfirmAccountLink = true;

            if (DisplayConfirmAccountLink)
            {
                // Gera token de confirmação e cria URL para confirmação de email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId, code, returnUrl },
                    protocol: Request.Scheme);
            }

            // Renderiza a página com as informações definidas
            return Page();
        }
    }
}
