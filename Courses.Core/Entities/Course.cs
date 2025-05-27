namespace Courses.Core.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;  // CS101
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int DurationInHours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid InstructorId { get; set; }

        public ICollection<CourseStudent> CourseStudents { get; set; } = [];
    }
}
