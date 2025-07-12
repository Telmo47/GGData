using GGData.Data;
using GGData.Data.Seed;
using GGData.Models;
using GGData.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ====== Configuração da Base de Dados ======
// Obtem a connection string do ficheiro appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Adiciona o contexto do Entity Framework para SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Páginas de erro específicas para desenvolvimento em BD
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ====== Configuração do Identity e Roles ======
// Configura a autenticação com Identity, usando o tipo Utilizadores (classe personalizada)
builder.Services.AddIdentity<Utilizadores, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // Requer confirmação de conta por email
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Usa o contexto EF para armazenar dados do Identity
.AddDefaultTokenProviders(); // Adiciona suporte a tokens padrão (ex: confirmação email, reset password)

// Configuração dos cookies do Identity (paths personalizados para login e acesso negado)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";          // Página de login
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Página de acesso negado
});

// ====== Configuração JWT ======
// Lê a configuração JWT do appsettings
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured."));

// ====== Configuração da autenticação ======
// Configura os esquemas de autenticação: cookie do Identity e JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;  // Default cookie do Identity
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false; // Para desenvolvimento, pode estar a false
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,               // Valida emissor
        ValidateAudience = true,             // Valida audiência
        ValidateLifetime = true,             // Valida validade do token
        ValidateIssuerSigningKey = true,    // Valida chave de assinatura

        ValidIssuer = jwtSettings["Issuer"],     // Emissor válido
        ValidAudience = jwtSettings["Audience"], // Audiência válida
        IssuerSigningKey = new SymmetricSecurityKey(key)  // Chave simétrica usada para validar token
    };
});

// ====== Serviços auxiliares ======
// Serviço para envio de emails falso (mock) usado para desenvolvimento
builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();

// Serviço para gestão de tokens JWT personalizado
builder.Services.AddScoped<TokenService>();

// ====== Configuração MVC + Razor Pages ======
// Adiciona suporte para controllers com views e Razor Pages
builder.Services.AddControllersWithViews()
    // Evita erros de referência cíclica ao serializar JSON (ex: entidades com relações circulares)
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddRazorPages();

// ====== Configuração de sessão ======
builder.Services.AddDistributedMemoryCache(); // Cache na memória para sessões
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);   // Tempo de expiração da sessão (30 minutos)
    options.Cookie.HttpOnly = true;                    // Cookie acessível só via HTTP, não por JS
    options.Cookie.IsEssential = true;                  // Cookie essencial para funcionamento
});

// ====== Configuração do Swagger para documentação da API ======
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GGData API",
        Version = "v1",
        Description = "API para gestão de videojogos, avaliações e utilizadores"
    });

    // Inclui comentários XML para documentação (extras do código)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Define esquema de segurança JWT para o Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no campo abaixo. Exemplo: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ====== Configuração do pipeline HTTP ======
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();   // Endpoint para migrações de BD em desenvolvimento

    await app.UseItToSeedSqlServerAsync();  // Método custom para popular a BD com dados iniciais

    app.UseSwagger();               // Ativa Swagger na dev
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GGData API v1");
        c.RoutePrefix = "swagger";  // URL: /swagger
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");  // Página de erro custom em produção
    app.UseHsts();                           // HTTP Strict Transport Security
}

app.UseHttpsRedirection();  // Redireciona HTTP para HTTPS
app.UseStaticFiles();       // Serve ficheiros estáticos (css, js, imagens)

app.UseRouting();           // Roteamento das requisições

app.UseAuthentication();    // Ativa autenticação (cookies + JWT)
app.UseAuthorization();     // Ativa autorização (roles, policies)

app.UseSession();           // Ativa sessões

// Configura rotas MVC padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();        // Ativa Razor Pages

app.Run();                  // Executa a aplicação
