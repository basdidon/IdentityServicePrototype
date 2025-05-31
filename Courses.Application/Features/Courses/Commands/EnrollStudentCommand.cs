using Courses.Application.Abstracts;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public record EnrollStudentCommand(Guid CourseId,Guid StudentId):IRequest;

    internal class EnrollStudentCommandHandler(ICourseRepository courseRepo) : IRequestHandler<EnrollStudentCommand>
    {
        public async Task Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {
            await courseRepo.EnrollStudentAsync(request.CourseId, request.StudentId, cancellationToken);
        }
    }
}
