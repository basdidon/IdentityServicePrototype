namespace Courses.Application.DTOs
{
    public record CreateCourseRequestDto
    {
        public string Code { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

        public int DurationInHours { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }

    public record CreateCourseDto
    {
        public string Code { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

        public Guid InstructorId { get; init; }

        public int DurationInHours { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }

}
