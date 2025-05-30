namespace Courses.Application.DTOs
{
    public class CourseQuery
    {
        public enum CourseStatus
        {
            None,
            NotStarted,
            Ongoing,
            Ended,
        }
        // Query Properties
        public CourseStatus? Status { get; set; } = CourseStatus.None;

        // Sort
        public string? OrderBy { get; set; }
        public bool? IsAscending { get; set; } = true;
        // Pagination
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
