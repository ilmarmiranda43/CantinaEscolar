using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CantinaEscolar.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // 1) Tenta pegar do ambiente (Render ou terminal local)
            var raw = Environment.GetEnvironmentVariable("DATABASE_URL");

            // 2) Se não houver, tenta appsettings.* (para facilitar local)
            if (string.IsNullOrWhiteSpace(raw))
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                raw = config.GetConnectionString("DefaultConnection");
            }

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Não foi possível obter a connection string. Defina DATABASE_URL ou ConnectionStrings:DefaultConnection.");

            var conn = ToNpgsql(raw);

            // (Opcional) compatibilidade de timestamp
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            optionsBuilder.UseNpgsql(conn);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static string ToNpgsql(string url)
        {
            // Se já for "Host=...;Port=..." retorna como está
            if (url.Contains("Host=", StringComparison.OrdinalIgnoreCase))
                return url;

            var uri = new Uri(url); // aceita postgres:// ou postgresql://
            var userInfo = uri.UserInfo.Split(':', 2);
            var user = Uri.UnescapeDataString(userInfo[0]);
            var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var db = uri.AbsolutePath.Trim('/');

            return $"Host={uri.Host};Port={uri.Port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}
