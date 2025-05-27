namespace Courses.Application.Abstracts
{
    public interface IEnrollmentService
    {
        Task EnrollStudentAsync(Guid courseId, Guid studentId,CancellationToken ct = default);
        Task<List<Guid>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct = default);
        Task RemoveEnrollmentAsync(Guid courseId, Guid studentId, CancellationToken ct = default);
    }
}
