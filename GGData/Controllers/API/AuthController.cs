using GGData.Models;
using GGData.Models.ViewModels;
using GGData.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GGData.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController] // Indica que é um controlador API, com comportamentos próprios como validação automática do modelo
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Utilizadores> _userManager; // Serviço para gerir utilizadores
        private readonly TokenService _tokenService; // Serviço personalizado para gerar tokens JWT

        // Construtor com injeção de dependências
        public AuthController(UserManager<Utilizadores> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // Endpoint POST api/auth/login para autenticar utilizador
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Procura o utilizador pelo email recebido no corpo da requisição
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            // Verifica se a password está correta
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            // Gera um token JWT para o utilizador autenticado
            var token = await _tokenService.GenerateTokenAsync(user);

            // Retorna o token no corpo da resposta com status 200 OK
            return Ok(new { Token = token });
        }
    }
}
