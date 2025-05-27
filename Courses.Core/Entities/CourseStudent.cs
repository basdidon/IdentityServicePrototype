namespace Courses.Core.Entities
{
    public class CourseStudent
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public Guid StudentId { get; set; }

        public DateTime EnrolledAt { get; set; }
    }
}
