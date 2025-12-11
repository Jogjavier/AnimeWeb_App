using AnimeWeb_App.Services;
using AnimeWeb_App.Models;
using AnimeWeb_App.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using DinkToPdf;
using DinkToPdf.Contracts;

var builder = WebApplication.CreateBuilder(args);

// DB - Configuración para Render
string connectionString;
var databaseUrl = Environment.GetEnvironmentVariable("postgresql://postgress:Wf1pHmmwrSXOFf98i75zQ58Aa8RAYFUV@dpg-d4t8ilmuk2gs73elafk0-a/animeweb_tzon");

if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        // Convertir DATABASE_URL de Render (postgres://...)
        var uri = new Uri(databaseUrl);
        
        // Extraer credenciales
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        
        // Extraer base de datos (remover el '/' inicial)
        var database = uri.AbsolutePath.TrimStart('/');
        
        // Construir connection string
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        
        Console.WriteLine($"✓ Usando DATABASE_URL - Host: {uri.Host}, Port: {uri.Port}, DB: {database}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error parseando DATABASE_URL: {ex.Message}");
        throw new Exception($"Error al parsear DATABASE_URL. Verifica que esté configurada correctamente en Render. Error: {ex.Message}");
    }
}
else
{
    // Desarrollo local: usar appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("✓ Usando ConnectionString local de appsettings.json");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);
// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "AnimeWebAuth";
});

// Filtros globales
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");
});

// Servicios HTTP
builder.Services.AddHttpClient<JikanService>(client =>
{
    client.BaseAddress = new Uri("https://api.jikan.moe/v4/");
});

// Razor -> HTML
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

// PDF
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IHtmlPdfService, HtmlPdfService>();


var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Anime}/{action=Index}/{id?}"
);

app.MapRazorPages();

// Aplicar migraciones automáticamente al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Seed Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Usuario" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();
