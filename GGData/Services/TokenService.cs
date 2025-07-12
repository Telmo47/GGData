using GGData.Models; // Para a classe Utilizadores
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
    /// Serviço responsável por gerar tokens JWT para autenticação.
    /// </summary>
    public class TokenService
    {
        private readonly UserManager<Utilizadores> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor que recebe dependências via injeção.
        /// </summary>
        /// <param name="userManager">Gerenciador de utilizadores do Identity.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        public TokenService(UserManager<Utilizadores> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um token JWT para o utilizador especificado.
        /// </summary>
        /// <param name="user">Utilizador para quem o token será gerado.</param>
        /// <returns>Token JWT como string.</returns>
        public async Task<string> GenerateTokenAsync(Utilizadores user)
        {
            // Obtém a configuração JWT do appsettings.json
            var jwtSettings = _configuration.GetSection("Jwt");

            // Cria a chave de segurança a partir da key definida na configuração
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            // Define as credenciais de assinatura usando HMAC-SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Obtém os roles (funções) do utilizador
            var roles = await _userManager.GetRolesAsync(user);

            // Cria lista de claims para o token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Identificador do utilizador
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),  // Email do utilizador
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único do token
            };

            // Adiciona as roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Cria o token JWT
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],         // Emissor do token
                audience: jwtSettings["Audience"],     // Destinatário do token
                claims: claims,                        // Claims incluídos no token
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"])), // Expiração
                signingCredentials: creds              // Credenciais de assinatura
            );

            // Retorna o token como string compacta
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
