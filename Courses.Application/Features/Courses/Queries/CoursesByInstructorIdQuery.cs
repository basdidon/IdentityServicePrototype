using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesByInstructorIdQuery(Guid InstructorId, CourseQueryFilters CourseQueryFilters):IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesByInstructorIdQueryHandler(ICourseRepository courseRepo) : IRequestHandler<CoursesByInstructorIdQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesByInstructorIdQuery request, CancellationToken cancellationToken)
        {
            var courses = await courseRepo.GetCoursesByInstructorAsync(request.InstructorId, request.CourseQueryFilters,cancellationToken);
            return [.. courses.Select(x => x.ToDto())];
        }
    }
}
