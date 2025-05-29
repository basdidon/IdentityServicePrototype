using Courses.Application.Abstracts;

namespace Courses.Application.Services
{
    public class EnrollmentService(ICourseRepository courseRepo) : IEnrollmentService
    {
        public async Task EnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct)
        {
            await courseRepo.EnrollStudentAsync(courseId, studentId, ct);
        }

        public async Task<List<Guid>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct)
        {
            var courseStudents = await courseRepo.GetEnrolledStudentsAsync(courseId, ct);
            return courseStudents.Select(x => x.StudentId).ToList();
        }

        public async Task RemoveEnrollmentAsync(Guid courseId, Guid studentId, CancellationToken ct)
        {
            await courseRepo.UnEnrollStudentAsync(courseId, studentId, ct);
        }
    }
}
