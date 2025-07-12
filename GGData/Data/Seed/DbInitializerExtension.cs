using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    /// <summary>
    /// Extensão para a aplicação que permite popular a base de dados automaticamente na inicialização.
    /// </summary>
    public static class DbInitializerExtension
    {
        /// <summary>
        /// Método de extensão para executar a inicialização (seed) da base de dados SQL Server ao arrancar a aplicação.
        /// </summary>
        /// <param name="app">Aplicação ASP.NET Core onde a extensão será aplicada.</param>
        /// <returns>Retorna o objeto IApplicationBuilder para encadeamento de chamadas.</returns>
        public static async Task<IApplicationBuilder> UseItToSeedSqlServerAsync(this IApplicationBuilder app)
        {
            // Cria um escopo para obter serviços injetados, garantindo que sejam corretamente descartados
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            // Obtem o contexto da base de dados e garante que a base está criada (cria se não existir)
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            // Executa o método SeedAsync para popular a base de dados com dados iniciais
            await DbInitializer.SeedAsync(services);

            // Retorna a aplicação para possibilitar encadeamento de chamadas
            return app;
        }
    }
}
