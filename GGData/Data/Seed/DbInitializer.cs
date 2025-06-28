using GGData.Data;
using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuarios>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Cria role se não existir
            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = "Administrador", NormalizedName = "ADMINISTRADOR" });
            }

            // Cria utilizador admin se não existir
            var adminUser = await userManager.FindByEmailAsync("admin@mail.pt");
            if (adminUser == null)
            {
                adminUser = new Usuarios
                {
                    UserName = "admin@mail.pt",
                    Email = "admin@mail.pt",
                    EmailConfirmed = true,
                    Nome = "Administrador",
                    DataRegistro = DateTime.Now,
                    TipoUsuario = "Administrador"
                };
                var result = await userManager.CreateAsync(adminUser, "Aa0_aa@123!");
                if (!result.Succeeded)
                {
                    throw new Exception("Erro ao criar utilizador admin: " + string.Join(", ", result.Errors));
                }
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }
        }
    }
}
