using Courses.Application.Abstracts;
using Courses.Core.Entities;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public sealed record CreateCourseCommand(
        string Code,
        string Title,
        string Description,
        Guid InstructorId,
        int DurationInHours,
        DateTime StartDate,
        DateTime EndDate) : IRequest<Guid>;

    internal class CreateCourseCommandHandler(ICourseRepository courseRepo) : IRequestHandler<CreateCourseCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            Course course = new()
            {
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                InstructorId = request.InstructorId,
                DurationInHours = request.DurationInHours,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };
            return await courseRepo.CreateCourseAsync(course, cancellationToken);
        }
    }
}
