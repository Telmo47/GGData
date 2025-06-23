using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GGData.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//L� do ficheiro do 'appsettings.json' as bases de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//Define o tipo de BD e a sua 'liga��o'
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identidade (login/register)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// ----------------------
// Configura��o de cookies (sess�o)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ou TimeSpan.FromSeconds(60) se quiseres testar r�pido
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// ----------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseAuthentication(); // Garantir login funciona corretamente
app.UseAuthorization();

// ----------------------
// Ativar cookies de sess�o
app.UseSession();
// ----------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
