using Courses.Application.DTOs;
using Courses.Core.Entities;
using Courses.Core.Enums;

namespace Courses.Application.Abstracts
{
    public interface ICourseRepository
    {
        Task<Guid> CreateCourseAsync(Course course, CancellationToken ct = default);
        Task UpdateCourseAsync(Course course, CancellationToken ct = default);
        Task UpdateCourseInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task DeleteCourseAsync(Guid courseId, CancellationToken ct = default);
        
        Task<Course?> GetCourseById(Guid courseId, CancellationToken ct = default);
        Task<Course?> GetCourseByCode(string code, CancellationToken ct = default);
        Task<ICollection<Course>> GetCoursesAsync(CourseStatus courseStatus, string orderBy, bool isAscending, int pageNumber, int pageSize, CancellationToken ct = default);
        Task<ICollection<Course>> GetCoursesByStudentAsync(Guid studentId , CourseStatus courseStatus, string orderBy, bool isAscending, int pageNumber, int pageSize, CancellationToken ct = default);
        Task<ICollection<Course>> GetCoursesByInstructorAsync(Guid instructorId, CourseStatus courseStatus, string orderBy, bool isAscending, int pageNumber, int pageSize, CancellationToken ct = default);

        // enrollment
        Task EnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default);
        Task UnEnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default);

        Task<ICollection<CourseStudent>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct = default);
    }
}
