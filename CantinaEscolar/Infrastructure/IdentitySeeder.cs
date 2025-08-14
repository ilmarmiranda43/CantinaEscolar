using CantinaEscolar.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CantinaEscolar.Infrastructure;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        const string roleName = "Admin";
        const string adminEmail = "admin@meusistema.com";
        const string adminPassword = "SenhaForte123!";

        // 1) Garante o role
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));

        // 2) Garante o usuário
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
                // Logue os erros para facilitar debug
                var errors = string.Join("; ", create.Errors);
                throw new System.Exception($"Falha ao criar usuário admin: {errors}");
            }
        }

        // 3) Garante que está no role Admin
        if (!await userManager.IsInRoleAsync(user, roleName))
            await userManager.AddToRoleAsync(user, roleName);
    }
}
