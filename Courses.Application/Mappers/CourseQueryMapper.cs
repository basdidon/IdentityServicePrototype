using Courses.Application.DTOs;
using Courses.Application.Features.Courses.Queries;
using Courses.Core.Enums;

namespace Courses.Application.Mappers
{
    public static class CourseQueryMapper
    {
        public static CoursesQuery ToCoursesQuery(this CourseQueryFilters filters)
        {
            return new CoursesQuery(
                CourseStatus: filters.Status ?? CourseStatus.None,
                OrderBy: filters.OrderBy ?? string.Empty,
                IsAscending: filters.IsAscending ?? true,
                Page: filters.Page ?? 1,
                PageSize: filters.PageSize ?? 20);
        }

        public static CoursesByStudentIdQuery ToCoursesByStudentIdQuery(this CourseQueryFilters filters, Guid studentId)
        {
            return new CoursesByStudentIdQuery(
                StudentId: studentId,
                CourseStatus: filters.Status ?? CourseStatus.None,
                OrderBy: filters.OrderBy ?? string.Empty,
                IsAscending: filters.IsAscending ?? true,
                Page: filters.Page ?? 1,
                PageSize: filters.PageSize ?? 20);
        }

        public static CoursesByInstructorIdQuery ToCoursesByInstructorIdQuery(this CourseQueryFilters filters, Guid instructorId)
        {
            return new CoursesByInstructorIdQuery(
                InstructorId: instructorId,
                CourseStatus: filters.Status ?? CourseStatus.None,
                OrderBy: filters.OrderBy ?? string.Empty,
                IsAscending: filters.IsAscending ?? true,
                Page: filters.Page ?? 1,
                PageSize: filters.PageSize ?? 20);
        }
    }

}
