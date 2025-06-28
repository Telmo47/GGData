using GGData.Models;
using GGData.Models.ViewModels;
using GGData.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GGData.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<Usuarios> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return Unauthorized("Utilizador ou palavra-passe inválidos");

            var token = await _tokenService.GenerateTokenAsync(user);

            return Ok(new { Token = token });
        }
    }
}
