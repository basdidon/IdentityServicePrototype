using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Mappers;

namespace Courses.Application.Services
{
    internal class CourseService(ICourseRepository courseRepo) : ICourseService
    {
        public async Task<Guid> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default)
        {
            return await courseRepo.CreateCourseAsync(dto.ToEntity(), ct);
        }

        public async Task UpdateCourseAsync(Guid courseId, UpdateCourseDto dto, CancellationToken ct = default)
        {
            await courseRepo.UpdateCourseAsync(dto.ToEntity(courseId), ct);
        }

        public async Task DeleteCourseAsync(Guid courseId, CancellationToken ct = default)
        {
            await courseRepo.DeleteCourseAsync(courseId, ct);
        }

        public async Task<CourseQueryDto?> GetCourseByIdAsync(Guid courseId, CancellationToken ct = default)
        {
            var course = await courseRepo.GetCourseById(courseId, ct);
            return course?.ToDto();
        }

        public async Task<ICollection<CourseQueryDto>> GetCoursesAsync(CourseQuery query, CancellationToken ct = default)
        {
            var courses = await courseRepo.GetCoursesAsync(query, ct);
            return [.. courses.Select(x => x.ToDto())];
        }

        public async Task<ICollection<CourseQueryDto>> GetCoursesByStudentAsync(Guid studentId, CourseQuery query, CancellationToken ct = default)
        {
            var courses = await courseRepo.GetCoursesByStudentAsync(studentId, query, ct);
            return [.. courses.Select(x => x.ToDto())];
        }

        public async Task<ICollection<CourseQueryDto>> GetCoursesByInstructorAsync(Guid instructorId,CourseQuery query, CancellationToken ct = default)
        {
            var courses = await courseRepo.GetCoursesByInstructorAsync(instructorId, query, ct);
            return [.. courses.Select(x => x.ToDto())];
        }
    }
}
