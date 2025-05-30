using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record CoursesByStudentIdQuery(Guid StudentId, CourseQueryFilters CourseQueryFilters):IRequest<IEnumerable<CourseQueryDto>>;

    internal class CoursesByStudentIdQueryHandler(ICourseRepository CourseRepo) : IRequestHandler<CoursesByStudentIdQuery, IEnumerable<CourseQueryDto>>
    {
        public async Task<IEnumerable<CourseQueryDto>> Handle(CoursesByStudentIdQuery request, CancellationToken cancellationToken)
        {
            var courses = await CourseRepo.GetCoursesByStudentAsync(request.StudentId,request.CourseQueryFilters,cancellationToken);
            return [.. courses.Select(x => x.ToDto())];
        }
    }
}
