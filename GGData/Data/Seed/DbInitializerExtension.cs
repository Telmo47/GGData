using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    public static class DbInitializerExtension
    {
        public static async Task<IApplicationBuilder> UseItToSeedSqlServerAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            await DbInitializer.SeedAsync(services);

            return app;
        }
    }
}
