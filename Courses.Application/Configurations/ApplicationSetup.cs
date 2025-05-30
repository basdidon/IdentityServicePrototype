using Courses.Application.Abstracts;
using Courses.Application.Features.Courses.Commands;
using Courses.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application.Configurations
{
    public static class ApplicationSetup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IEnrollmentService, EnrollmentService>();

            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(typeof(CreateCourseCommand).Assembly));

            return services;
        }
    }
}
