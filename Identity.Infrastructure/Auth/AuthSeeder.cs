using Identity.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shared.Constants;

namespace Identity.Infrastructure.Auth
{
    public static class AuthSeeder
    {
        public static async Task UseApplicationRoles(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in ApplicationRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedUsers(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ADD Users below. (password should match with the password rules.)
            await CreateUserAsync(userManager, "admin", "admin123", ApplicationRoles.Admin);
            await CreateUserAsync(userManager, "teacher", "teacher123", ApplicationRoles.Teacher);
            await CreateUserAsync(userManager, "student01", "student123", ApplicationRoles.Student);
        }


        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string username, string password, string role)
        {
            ApplicationUser user = new() { UserName = username };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
            Console.WriteLine($"UserCreated:{user.UserName} [{user.Id}]");
        }
    }
}
