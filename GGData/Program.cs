using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GGData.Data;

var builder = WebApplication.CreateBuilder(args);

// L� do ficheiro 'appsettings.json' a connection string para a base de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar Entity Framework para usar SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adiciona p�gina de erro detalhada para desenvolvimento em BD
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar autentica��o e autoriza��o com Identity e suporte a Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Essencial para suportar roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Configura��o de cache em mem�ria e sess�o para cookies
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de inatividade antes da sess�o expirar
    options.Cookie.HttpOnly = true;                 // Cookie inacess�vel via JavaScript
    options.Cookie.IsEssential = true;              // Cookie essencial para a aplica��o
});

var app = builder.Build();

// Configura��o do pipeline HTTP
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

app.UseAuthentication(); // Necess�rio para ativar autentica��o
app.UseAuthorization();

// Ativar cookies de sess�o
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
