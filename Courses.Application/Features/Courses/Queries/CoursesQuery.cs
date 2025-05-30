using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesQuery(CourseQueryFilters CourseQueryFilters):IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesQueryHandler(ICourseRepository courseRepo) : IRequestHandler<CoursesQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await courseRepo.GetCoursesAsync(request.CourseQueryFilters,cancellationToken);
            return [.. courses.Select(x => x.ToDto())];
        }
    }
}
