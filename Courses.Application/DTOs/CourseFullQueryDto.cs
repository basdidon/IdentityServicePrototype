namespace Courses.Application.DTOs
{
    public class CourseFullQueryDto
    {
        public Guid CourseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid InstructorId { get; set; }

        public int DurationInHours { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<CourseStudentDto> CourseStudents { get; set; } = [];
    }

    public class CourseStudentDto
    {
        public Guid StudentId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
