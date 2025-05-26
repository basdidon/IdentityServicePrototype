using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<TokenService>();
            services.AddTransient<IUserService, UserService>();

            return services;
        }
    }
}
