using Courses.Application.DTOs;

namespace Courses.Application.Abstracts
{
    public interface ICourseService
    {
        Task<Guid> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct = default);
        Task UpdateCourseAsync(Guid courseId, UpdateCourseDto dto, CancellationToken ct = default);
        Task DeleteCourseAsync(Guid courseId, CancellationToken ct = default);

        Task<CourseQueryDto?> GetCourseByIdAsync(Guid courseId, CancellationToken ct = default);
        Task<ICollection<CourseQueryDto>> GetCoursesAsync(CourseQueryFilters query,CancellationToken ct = default);
        Task<ICollection<CourseQueryDto>> GetCoursesByStudentAsync(Guid studentId, CourseQueryFilters query, CancellationToken ct = default);
        Task<ICollection<CourseQueryDto>> GetCoursesByInstructorAsync(Guid instructorId, CourseQueryFilters query, CancellationToken ct = default);

    }
}
