using Courses.Application.DTOs;
using Courses.Core.Entities;

namespace Courses.Application.Mappers
{
    public static class CourseMapper
    {
        public static CourseQueryDto ToDto(this Course course)
        => new()
        {
            CourseId = course.Id,
            Code = course.Code,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            DurationInHours = course.DurationInHours,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
        };

        public static CourseStudentQueryDto ToDto(this CourseStudent courseStudent)
        => new()
        {
            StudentId = courseStudent.StudentId,
            EnrolledAt = courseStudent.EnrolledAt,
        };
    }

}
