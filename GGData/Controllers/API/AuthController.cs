using GGData.Models;
using GGData.Models.ViewModels;
using GGData.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GGData.Controllers.API
{
    /// <summary>
    /// Controlador API responsável pela autenticação dos utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Utilizadores> _userManager;
        private readonly TokenService _tokenService;

        /// <summary>
        /// Construtor que injeta os serviços necessários: UserManager para gerir utilizadores e TokenService para gerar tokens JWT.
        /// </summary>
        /// <param name="userManager">Gestor de utilizadores</param>
        /// <param name="tokenService">Serviço de geração de tokens</param>
        public AuthController(UserManager<Utilizadores> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Endpoint POST para login de utilizadores.
        /// Recebe as credenciais, valida e retorna um token JWT se autenticado com sucesso.
        /// </summary>
        /// <param name="model">Modelo com email e palavra-passe</param>
        /// <returns>Token JWT se sucesso, caso contrário Unauthorized</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Procura utilizador pelo email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            // Verifica se a palavra-passe está correta
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            // Gera token JWT para o utilizador autenticado
            var token = await _tokenService.GenerateTokenAsync(user);

            // Retorna token na resposta
            return Ok(new { Token = token });
        }
    }
}
