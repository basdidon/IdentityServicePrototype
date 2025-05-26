using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations<T>(this IApplicationBuilder app, bool reset = false) where T : DbContext
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<T>();

            if (await context.Database.CanConnectAsync())
            {
                if (reset) await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }
        }

        public static async Task EnsureDbCreated<T>(this IApplicationBuilder app, bool reset = false) where T : DbContext
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<T>();

            if (await context.Database.CanConnectAsync())
            {
                if (reset) await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
        }
    }
}
