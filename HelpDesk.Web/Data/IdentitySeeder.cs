using HelpDesk.DAL.Identity;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Web.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            // Create required roles
            foreach (var role in new[] { "Admin", "Technician" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!roleResult.Succeeded)
                    {
                        throw new Exception(string.Join(
                            "; ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            var adminEmail = configuration["InitialAdmin:Email"];
            var adminPassword = configuration["InitialAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Initial Admin credentials are missing from configuration.");
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
                    Department = "IT Administration",
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(
                        "; ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var result = await userManager.AddToRoleAsync(
                    adminUser, "Admin");

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(
                        "; ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}