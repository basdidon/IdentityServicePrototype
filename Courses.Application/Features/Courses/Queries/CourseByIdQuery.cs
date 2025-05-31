using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CourseByIdQuery(Guid CourseId) : IRequest<CourseQueryDto?>;

    internal class CourseByIdQueryHandler(ICourseRepository courseRepo) : IRequestHandler<CourseByIdQuery, CourseQueryDto?>
    {
        public async Task<CourseQueryDto?> Handle(CourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await courseRepo.GetCourseById(request.CourseId, cancellationToken);
            return course?.ToDto();
        }
    }
}
