using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GGData.Data.Seed
{
    /// <summary>
    /// Classe estática que fornece método de extensão para a inicialização da base de dados.
    /// </summary>
    public static class DbInitializerExtension
    {
        /// <summary>
        /// Método de extensão assíncrono para IApplicationBuilder que garante a criação
        /// da base de dados e executa a semente inicial (seed) de dados.
        /// </summary>
        /// <param name="app">Aplicação ASP.NET Core para extensão.</param>
        /// <returns>Retorna o mesmo IApplicationBuilder para encadeamento de chamadas.</returns>
        public static async Task<IApplicationBuilder> UseItToSeedSqlServerAsync(this IApplicationBuilder app)
        {
            // Cria um escopo para obter serviços do DI container
            using var scope = app.ApplicationServices.CreateScope();

            // Obtém o provedor de serviços dentro do escopo
            var services = scope.ServiceProvider;

            // Obtém o contexto da base de dados e assegura que a BD está criada
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            // Executa o método de semente para inserir dados iniciais
            await DbInitializer.SeedAsync(services);

            // Retorna o app para permitir chamadas encadeadas
            return app;
        }
    }
}
