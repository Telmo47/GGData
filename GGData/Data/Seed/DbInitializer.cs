using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    /// <summary>
    /// Classe responsável por inicializar e popular a base de dados com dados essenciais.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Método que realiza a inicialização dos dados, criando roles e utilizadores iniciais se não existirem.
        /// </summary>
        /// <param name="serviceProvider">Provedor de serviços para obter dependências necessárias.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            // Obter o RoleManager para gerir roles (perfis/permissões)
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Obter o UserManager para gerir utilizadores
            var userManager = serviceProvider.GetRequiredService<UserManager<Utilizadores>>();

            // Obter o contexto da base de dados
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Verifica se a role "Administrador" já existe; se não, cria-a
            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR"
                });
            }

            // Verifica se o utilizador admin já existe com o email indicado
            var adminUser = await userManager.FindByEmailAsync("admin@mail.pt");
            if (adminUser == null)
            {
                // Cria o utilizador administrador com dados iniciais
                adminUser = new Utilizadores
                {
                    UserName = "admin@mail.pt",
                    Email = "admin@mail.pt",
                    EmailConfirmed = true, // Confirma o email automaticamente
                    Nome = "Administrador",
                    DataRegistro = DateTime.Now,
                    TipoUsuario = "Administrador"
                };

                // Cria o utilizador com a password definida
                var result = await userManager.CreateAsync(adminUser, "Aa0_aa@123!");

                if (!result.Succeeded)
                {
                    // Se falhar, lança exceção com a descrição dos erros
                    throw new Exception("Erro ao criar utilizador admin: " + string.Join(", ", result.Errors));
                }

                // Associa o utilizador criado à role "Administrador"
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }
        }
    }
}
