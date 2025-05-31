using Courses.Application.Abstracts;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public record UnEnrollStudentCommand(Guid CourseId,Guid StudentId):IRequest;

    internal class UnEnrollStudentCommandHandler(ICourseRepository courseRepo) : IRequestHandler<UnEnrollStudentCommand>
    {
        public async Task Handle(UnEnrollStudentCommand request, CancellationToken cancellationToken)
        {
            await courseRepo.UnEnrollStudentAsync(request.CourseId, request.StudentId, cancellationToken);
        }
    }
}
