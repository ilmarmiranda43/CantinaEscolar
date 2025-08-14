using CantinaEscolar.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CantinaEscolar.Infrastructure
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 1) Garante todos os perfis (roles) necessários
            var roles = new[] { "Admin", "Aluno", "Prop", "Responsavel" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2) Garante o usuário Admin
            const string adminEmail = "admin@meusistema.com";
            const string adminPassword = "SenhaForte123!";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nome = "Administrador",
                    RA = "000000"
                };

                var create = await userManager.CreateAsync(user, adminPassword);
                if (!create.Succeeded)
                {
                    var errors = string.Join("; ", create.Errors);
                    throw new System.Exception($"Falha ao criar usuário admin: {errors}");
                }
            }

            // 3) Garante que o admin está no role Admin
            if (!await userManager.IsInRoleAsync(user, "Admin"))
                await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
