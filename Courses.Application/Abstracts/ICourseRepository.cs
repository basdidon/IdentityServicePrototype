using Courses.Application.DTOs;
using Courses.Core.Entities;

namespace Courses.Application.Abstracts
{
    public interface ICourseRepository
    {
        Task<Guid> CreateCourseAsync(Course course, CancellationToken ct = default);
        Task UpdateCourseAsync(Course course, CancellationToken ct = default);
        Task DeleteCourseAsync(Guid courseId, CancellationToken ct = default);
        
        Task<Course?> GetCourseById(Guid courseId, CancellationToken ct = default);
        Task<Course?> GetCourseByCode(string code, CancellationToken ct = default);
        Task<ICollection<Course>> GetCoursesAsync(CourseQuery courseQuery, CancellationToken ct = default);

        // enrollment
        Task EnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default);
        Task UnEnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default);

        Task<ICollection<CourseStudent>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct = default);
        Task<ICollection<Course>> GetStudentCoursesAsync(Guid studentId, CancellationToken ct = default);
    }
}
