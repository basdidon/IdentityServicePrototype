using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using Courses.Core.Enums;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesByStudentIdQuery(Guid StudentId, CourseStatus CourseStatus, string OrderBy, bool IsAscending, int Page, int PageSize) :IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesByStudentIdQueryHandler(ICourseRepository CourseRepo) : IRequestHandler<CoursesByStudentIdQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesByStudentIdQuery request, CancellationToken cancellationToken)
        {
            var courses = await CourseRepo.GetCoursesByStudentAsync(
                request.StudentId,
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
