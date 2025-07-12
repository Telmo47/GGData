using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace GGData.Areas.Identity.Pages.Account
{
    // Modelo da Razor Page para confirmar o email do utilizador
    public class ConfirmEmailModel : PageModel
    {
        // Serviço para gerir utilizadores (UserManager) com a classe Utilizadores personalizada
        private readonly UserManager<Utilizadores> _userManager;

        // Construtor que injeta o UserManager
        public ConfirmEmailModel(UserManager<Utilizadores> userManager)
        {
            _userManager = userManager;
        }

        // Propriedade para armazenar uma mensagem temporária (TempData) a mostrar na página
        [TempData]
        public string StatusMessage { get; set; }

        // Método assíncrono chamado quando a página é acedida via GET
        // Recebe o userId e o código de confirmação de email (code)
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            // Se userId ou code não forem fornecidos, redireciona para a página inicial
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            // Procura o utilizador na base de dados pelo ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Se o utilizador não existir, retorna erro 404 com mensagem personalizada
                return NotFound($"Não foi possível carregar utilizador com ID '{userId}'.");
            }

            // Decodifica o código recebido da URL (base64)
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Tenta confirmar o email do utilizador com o código decodificado
            var result = await _userManager.ConfirmEmailAsync(user, code);

            // Define a mensagem de status consoante o resultado da confirmação
            StatusMessage = result.Succeeded ? "Obrigado por confirmares o teu email." : "Erro ao confirmar email.";

            // Retorna a página para mostrar a mensagem
            return Page();
        }
    }
}
