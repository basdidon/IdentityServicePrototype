using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;
using MediatR;

namespace Courses.Application.Features.Courses.Queries
{
    public record EnrolledStudentsQuery(Guid CourseId):IRequest<IEnumerable<CourseStudentQueryDto>>;

    internal class EnrolledStudentsQueryHandler(ICourseRepository courseRepo) : IRequestHandler<EnrolledStudentsQuery, IEnumerable<CourseStudentQueryDto>>
    {
        public async Task<IEnumerable<CourseStudentQueryDto>> Handle(EnrolledStudentsQuery request, CancellationToken cancellationToken)
        {
            var courseStudents = await courseRepo.GetEnrolledStudentsAsync(request.CourseId, cancellationToken);
            return [.. courseStudents.Select(x => x.ToDto())];
        }
    }
}
