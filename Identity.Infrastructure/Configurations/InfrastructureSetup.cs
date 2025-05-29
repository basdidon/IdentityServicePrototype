using Identity.Application.Interfaces;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Options;

namespace Identity.Infrastructure.Configurations;

public static class InfrastructureSetup
{
    public static IServiceCollection AddInfrastucture(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddDbContext<ApplicationDbContext>((options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

    public static async void UseInfrastructure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            await app.EnsureDbCreated<ApplicationDbContext>(reset: true);
            await app.UseApplicationRoles();
            await app.SeedUsers();
        }
    }
}
