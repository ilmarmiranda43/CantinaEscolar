using System;
using CantinaEscolar.Data;
using CantinaEscolar.Models;
using CantinaEscolar.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// (Opcional) comportamento de timestamp legado do Npgsql (evita warnings com DateTime)
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
    // Se já for formato Npgsql (Host=...;Port=...), retorna direto
    if (url.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        return url;

    var uri = new Uri(url); // aceita "postgres://" e "postgresql://"
    var userInfo = uri.UserInfo.Split(':', 2);
    var user = Uri.UnescapeDataString(userInfo[0]);
    var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var db = uri.AbsolutePath.Trim('/');

    // Observação: se a URL tiver querystring (ex.: ?sslmode=require), você pode ler via uri.Query
    // mas no Render exigimos TLS sempre:
    return $"Host={uri.Host};Port={uri.Port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
}

var npgsqlConn = ToNpgsql(rawUrl);

// DbContext com Npgsql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(npgsqlConn));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

// Services
builder.Services.AddScoped<IVendaService, VendaService>();

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

// Aplica migrations no start (útil no Render)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
