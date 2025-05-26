using Identity.Application.Interfaces;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared;

namespace Identity.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastucture(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider,options) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                options.UseNpgsql(settings.DefaultConnection);
            });



            return services;
        }

        public static async void UseInfrastructure(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                await app.EnsureDbCreated<ApplicationDbContext>(reset:true);
                await app.UseApplicationRoles();
                await app.SeedUsers();
            }
        }
    }
}
