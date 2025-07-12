using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    /// <summary>
    /// Classe responsável pela inicialização da base de dados com dados padrão,
    /// como roles e utilizadores administrativos.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Método assíncrono para garantir que a role "Administrador" e um utilizador admin
        /// existem na base de dados. Se não existirem, cria-os.
        /// </summary>
        /// <param name="serviceProvider">Provedor de serviços para obter os gerenciadores necessários.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            // Obtém RoleManager para gerir roles (funções) de utilizadores
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Obtém UserManager para gerir utilizadores
            var userManager = serviceProvider.GetRequiredService<UserManager<Utilizadores>>();

            // Obtém contexto da base de dados (não usado diretamente aqui, mas pode ser necessário para outras sementes)
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Verifica se a role "Administrador" existe e cria se não existir
            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "Administrador", NormalizedName = "ADMINISTRADOR" });
            }

            // Verifica se o utilizador administrador já existe pelo email
            var adminUser = await userManager.FindByEmailAsync("admin@mail.pt");

            // Se não existir, cria o utilizador admin com dados padrão
            if (adminUser == null)
            {
                adminUser = new Utilizadores
                {
                    UserName = "admin@mail.pt",
                    Email = "admin@mail.pt",
                    EmailConfirmed = true,
                    Nome = "Administrador",
                    DataRegistro = DateTime.Now,
                    TipoUsuario = "Administrador"
                };

                // Cria o utilizador com password inicial segura
                var result = await userManager.CreateAsync(adminUser, "Aa0_aa@123!");

                // Lança exceção em caso de erro na criação do utilizador admin
                if (!result.Succeeded)
                {
                    throw new Exception("Erro ao criar utilizador admin: " + string.Join(", ", result.Errors));
                }

                // Adiciona o utilizador criado à role "Administrador"
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }
        }
    }
}
