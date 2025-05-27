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

        public static Course ToEntity(this CreateCourseDto dto)
        => new()
        {
            Code = dto.Code,
            Title = dto.Title,
            Description = dto.Description,
            InstructorId = dto.InstructorId,
            DurationInHours = dto.DurationInHours,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        public static Course ToEntity(this UpdateCourseDto dto, Guid courseId)
        => new()
        {
            Id = courseId,
            Code = dto.Code,
            Title = dto.Title,
            Description = dto.Description,
            InstructorId = dto.InstructorId,
            DurationInHours = dto.DurationInHours,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

    }

}
