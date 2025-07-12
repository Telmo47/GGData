using GGData.Models; // para a classe Utilizadores
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GGData.Services
{
    /// <summary>
    /// Serviço para gerar tokens JWT para autenticação.
    /// </summary>
    public class TokenService
    {
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa uma nova instância do serviço com as dependências necessárias.
        /// </summary>
        /// <param name="userManager">Gerenciador de utilizadores do Identity</param>
        /// <param name="configuration">Configuração da aplicação para obter as definições do JWT</param>
        public TokenService(UserManager<Utilizadores> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um token JWT assinado para o utilizador especificado.
        /// O token inclui claims do utilizador, como Id, Email, Nome de Utilizador e Roles.
        /// </summary>
        /// <param name="user">Instância do utilizador para o qual gerar o token</param>
        /// <returns>String contendo o token JWT gerado</returns>
        public async Task<string> GenerateTokenAsync(Utilizadores user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

            // Adiciona um claim para cada role do utilizador
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
