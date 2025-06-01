using Courses.Application.Abstracts;
using Courses.Core.Entities;
using Courses.Core.Enums;
using Courses.Infrastructure.Data;
using Courses.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : ICourseRepository
    {
        // Mutation
        public async Task<Guid> CreateCourseAsync(Course course, CancellationToken ct = default)
        {
            await context.Courses.AddAsync(course, ct);
            await context.SaveChangesAsync(ct);
            return course.Id;
        }

        public async Task UpdateCourseAsync(Course course, CancellationToken ct = default)
        {
            context.Courses.Update(course);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateCourseInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct = default)
        {
            Course course = new() { Id = courseId };
            context.Attach(course);
            course.InstructorId = instructorId;

            context.Entry(course).Property(x => x.InstructorId).IsModified = true;

            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteCourseAsync(Guid courseId, CancellationToken ct = default)
        {
            var course = await context.Courses.FindAsync([courseId], ct);
            if (course is not null)
            {
                context.Courses.Remove(course);
                await context.SaveChangesAsync(ct);
            }
        }

        // Query
        public async Task<ICollection<Course>> GetCoursesAsync(
            CourseStatus courseStatus,
            string orderBy,
            bool isAscending,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            return await context.Courses.AsQueryable()
                .AsNoTracking()
                .ByStatus(courseStatus)
                .OrderBy(orderBy, isAscending)
                .ToPage(pageNumber, pageSize)
                .ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetCoursesByStudentAsync(
            Guid studentId,
            CourseStatus courseStatus,
            string orderBy,
            bool isAscending,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            return await context.CourseStudents.AsQueryable()
                .AsNoTracking()
                .Where(x => x.StudentId == studentId)
                .Select(x => x.Course)
                .ByStatus(courseStatus)
                .OrderBy(orderBy, isAscending)
                .ToPage(pageNumber, pageSize)
                .ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetCoursesByInstructorAsync(
            Guid instructorId,
            CourseStatus courseStatus,
            string orderBy,
            bool isAscending,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            return await context.Courses.AsQueryable()
                .AsNoTracking()
                .Where(x => x.InstructorId == instructorId)
                .ByStatus(courseStatus)
                .OrderBy(orderBy, isAscending)
                .ToPage(pageNumber, pageSize)
                .ToListAsync(ct);
        }

        public async Task<Course?> GetCourseByCode(string code, CancellationToken ct = default)
        {
            return await context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code, ct);
        }

        public async Task<Course?> GetCourseById(Guid courseId, CancellationToken ct = default)
        {
            return await context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId, ct);
        }

        #region Enrollment
        // Mutation
        public async Task EnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default)
        {
            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken: ct);
            var courseStudent = new CourseStudent() { StudentId = studentId };

            course?.CourseStudents.Add(courseStudent);
            await context.SaveChangesAsync(ct);
        }

        public async Task UnEnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default)
        {
            var course = await context.Courses.Include(x=>x.CourseStudents)
                .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken: ct);
            var toRemove = course?.CourseStudents.FirstOrDefault(x => x.StudentId == studentId);

            if (toRemove != null)
            {
                course?.CourseStudents.Remove(toRemove);
                await context.SaveChangesAsync(ct);
            }
        }

        // Query
        public async Task<ICollection<CourseStudent>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct = default)
        {
            return await context.CourseStudents.AsNoTracking()
                .Where(cs => cs.CourseId == courseId)
                .ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetStudentCoursesAsync(Guid studentId, CancellationToken ct = default)
        {
            return await context.CourseStudents.AsNoTracking()
                .Where(cs => cs.StudentId == studentId)
                .Select(cs => cs.Course)
                .ToListAsync(ct);
        }
        #endregion
    }
}
