using Courses.Application.Abstracts;
using Courses.Infrastructure.Data;
using Courses.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

namespace Courses.Infrastructure.Configurations
{
    public static class InfrastructureSetup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();

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
            }
        }
    }
}
