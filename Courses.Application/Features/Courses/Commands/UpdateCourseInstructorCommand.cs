using Courses.Application.Abstracts;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public  sealed record UpdateCourseInstructorCommand(
        Guid CourseId,
        Guid InstructorId) : IRequest;

    internal class UpdateCourseInstructorCommandHandle(ICourseRepository courseRepo) : IRequestHandler<UpdateCourseInstructorCommand>
    {
        public async Task Handle(UpdateCourseInstructorCommand request, CancellationToken cancellationToken)
        {
            await courseRepo.UpdateCourseInstructorAsync(request.CourseId, request.InstructorId, cancellationToken);
        }
    }
}
