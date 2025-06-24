using Microsoft.AspNetCore.Builder;
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
                DbInitializer.SeedAsync(services).Wait();
            }
            catch (Exception ex)
            {
                // Aqui podes fazer log do erro, se quiseres
                throw;
            }

            return app;
        }
    }
}
