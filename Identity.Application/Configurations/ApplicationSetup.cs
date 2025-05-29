using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Configurations
{
    public static class ApplicationSetup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();

            return services;
        }
    }
}
