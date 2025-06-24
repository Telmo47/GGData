using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GGData.Data;

var builder = WebApplication.CreateBuilder(args);

// Lê do ficheiro 'appsettings.json' a connection string para a base de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar Entity Framework para usar SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona página de erro detalhada para desenvolvimento em BD
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar autenticação e autorização com Identity e suporte a Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Essencial para suportar roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Configuração de cache em memória e sessão para cookies
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de inatividade antes da sessão expirar
    options.Cookie.HttpOnly = true;                 // Cookie inacessível via JavaScript
    options.Cookie.IsEssential = true;              // Cookie essencial para a aplicação
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Necessário para ativar autenticação
app.UseAuthorization();

// Ativar cookies de sessão
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
