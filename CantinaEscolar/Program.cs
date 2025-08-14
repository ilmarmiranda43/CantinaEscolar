using System;
using CantinaEscolar.Data;
using CantinaEscolar.Models;
using CantinaEscolar.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


// + para chamar o seeder
using CantinaEscolar.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// (Opcional) comportamento de timestamp legado do Npgsql
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// --- SOMENTE DATABASE_URL (sem fallback a appsettings) ---
var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrWhiteSpace(rawUrl))
{
    throw new InvalidOperationException(
        "DATABASE_URL não definida. Defina a URL do Postgres do Render em Environment Variables."
    );
}

// Converte postgres(ql)://user:pass@host:port/db -> Npgsql format
static string ToNpgsql(string url)
{
    if (url.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        return url;

    var uri = new Uri(url); // aceita postgres:// e postgresql://
    var userInfo = uri.UserInfo.Split(':', 2);
    var user = Uri.UnescapeDataString(userInfo[0]);
    var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var db = uri.AbsolutePath.Trim('/');

    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;

    // TLS exigido no Render
    return $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
}

var npgsqlConn = ToNpgsql(rawUrl);

// DbContext com Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(npgsqlConn));

// Identity (+ tokens por padrão)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

// Services
builder.Services.AddScoped<IVendaService, VendaService>();

// Porta do Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // habilite se publicar atrás de HTTPS
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- MIGRATIONS + SEED (ASSÍNCRONO) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync(); // cria/atualiza schema

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeeder.SeedAdminAsync(userManager, roleManager);
}
// --- FIM SEED ---

app.Run();
