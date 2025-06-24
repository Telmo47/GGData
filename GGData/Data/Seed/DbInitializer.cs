using GGData.Data;  // Para ApplicationDbContext e Usuario
using GGData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Criar role Administrador se não existir
            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            // Criar utilizador admin se não existir
            var adminUser = await userManager.FindByEmailAsync("admin@mail.pt");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@mail.pt", Email = "admin@mail.pt", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Aa0_aa"); // senha de exemplo
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }

            // Popular tabela Usuarios (a tua tabela personalizada)
            if (!await context.Usuarios.AnyAsync())
            {
                context.Usuarios.Add(new Usuarios
                {
                    Nome = "Administrador",
                    Email = "admin@mail.pt",
                    Senha = "Aa0_aa", // Só para exemplo, cuidado em produção
                    DataRegistro = DateTime.Now,
                    TipoUsuario = "Administrador",
                    UserName = "admin@mail.pt"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
