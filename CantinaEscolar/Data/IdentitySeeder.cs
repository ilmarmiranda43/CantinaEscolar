using CantinaEscolar.Models;
using Microsoft.AspNetCore.Identity;

namespace CantinaEscolar.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "admin@meusistema.com";
            const string adminPassword = "SenhaForte123!";

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

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

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
