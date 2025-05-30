using Courses.Application.Abstracts;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public sealed record RemoveCourseCommand(Guid CourseId) : IRequest;

    internal class RemoveCourseCommandHandler(ICourseRepository courseRepo) : IRequestHandler<RemoveCourseCommand>
    {
        public async Task Handle(RemoveCourseCommand request, CancellationToken cancellationToken)
        {
            await courseRepo.DeleteCourseAsync(request.CourseId, cancellationToken);
        }
    }
}
