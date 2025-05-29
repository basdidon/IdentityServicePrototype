using Courses.Application.Abstracts;
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
                
            return services;
        }
    }
}
