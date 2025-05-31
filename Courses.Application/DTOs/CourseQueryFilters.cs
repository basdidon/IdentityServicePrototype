using Courses.Core.Enums;

namespace Courses.Application.DTOs
{
    public class CourseQueryFilters
    {
        // Query Properties
        public CourseStatus? Status { get; set; }

        // Sort
        public string? OrderBy { get; set; }
        public bool? IsAscending { get; set; }
        // Pagination
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
