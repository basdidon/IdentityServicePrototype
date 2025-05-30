using Courses.Application.Abstracts;
using Courses.Core.Entities;
using MediatR;

namespace Courses.Application.Features.Courses.Commands
{
    public sealed record UpdateCourseCommand(
        Guid CourseId,
        string Code,
        string Title,
        string Description,
        int DurationInHours,
        DateTime StartDate,
        DateTime EndDate) : IRequest;

    internal class UpdateCourseCommandHandler(ICourseRepository courseRepo) : IRequestHandler<UpdateCourseCommand>
    {
        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            Course course = new()
            {
                Id = request.CourseId,
                Code = request.Code,
                Title = request.Title,
                Description = request.Description,
                DurationInHours = request.DurationInHours,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await courseRepo.UpdateCourseAsync(course, cancellationToken);
        }
    }
}
