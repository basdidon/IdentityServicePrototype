using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using Courses.Core.Enums;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesByInstructorIdQuery(Guid InstructorId, CourseStatus CourseStatus, string OrderBy, bool IsAscending, int Page, int PageSize) :IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesByInstructorIdQueryHandler(ICourseRepository courseRepo) : IRequestHandler<CoursesByInstructorIdQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesByInstructorIdQuery request, CancellationToken cancellationToken)
        {
            var courses = await courseRepo.GetCoursesByInstructorAsync(
                request.InstructorId,
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
