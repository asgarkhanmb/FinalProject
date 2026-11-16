using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Service.Helpers.Enums;

namespace FinalProject.Helpers
{
    /// <summary>
    /// Proqram başlayanda rolları (Admin, Member) yaradır və appsettings-dəki
    /// "AdminSeed" bölməsinə əsasən admin istifadəçini təmin edir.
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            try
            {
                foreach (var role in Enum.GetNames(typeof(Roles)))
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var section = config.GetSection("AdminSeed");
                var userName = section["UserName"];
                var email = section["Email"];
                var password = section["Password"];
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return;

                var admin = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(email);
                if (admin == null)
                {
                    admin = new AppUser
                    {
                        UserName = userName,
                        Email = email,
                        FullName = section["FullName"] ?? "Administrator",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(admin, password);
                    if (!result.Succeeded)
                    {
                        logger.LogWarning("Admin user could not be created: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                        return;
                    }
                    logger.LogInformation("Admin user '{UserName}' created.", userName);
                }

                foreach (var role in new[] { Roles.Admin.ToString(), Roles.Member.ToString() })
                {
                    if (!await userManager.IsInRoleAsync(admin, role))
                        await userManager.AddToRoleAsync(admin, role);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seeding failed");
            }
        }
    }
}
