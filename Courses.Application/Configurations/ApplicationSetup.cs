using Courses.Application.Features.Courses.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Configurations
{
    public static class ApplicationSetup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(typeof(CreateCourseCommand).Assembly));

            return services;
        }
    }
}
