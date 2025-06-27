using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GGData.Data.Seed
{
    public static class DbInitializerExtension
    {
        public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated(); // Garante que a BD existe
                DbInitializer.SeedAsync(services).Wait();
            }
            catch (Exception ex)
            {
                throw;
            }

            return app;
        }
    }
}
