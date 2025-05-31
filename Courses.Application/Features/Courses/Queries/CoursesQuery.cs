using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using Courses.Core.Enums;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesQuery(CourseStatus CourseStatus, string OrderBy, bool IsAscending, int Page, int PageSize):IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesQueryHandler(ICourseRepository courseRepo) : IRequestHandler<CoursesQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await courseRepo.GetCoursesAsync(
                request.CourseStatus,
                request.OrderBy,
                request.IsAscending,
                request.Page,
                request.PageSize,
                cancellationToken);
            return [.. courses.Select(x => x.ToDto())];
        }
    }
}
