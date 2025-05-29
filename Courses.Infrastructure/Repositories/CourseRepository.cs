using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Core.Entities;
using Courses.Infrastructure.Data;
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
        public async Task<ICollection<Course>> GetCoursesAsync(CourseQuery query, CancellationToken ct = default)
        {
            var courses = context.Courses.AsQueryable();

            // by instructor
            if(query.InstructorId is Guid instructorId)
            {
                courses = courses.Where(x => x.InstructorId == instructorId);
            }
           
            // status
            if(query.Status == CourseQuery.CourseStatus.NotStarted)
            {
                courses = courses.Where(x => x.StartDate > DateTime.UtcNow);
            }
            else if (query.Status == CourseQuery.CourseStatus.Ongoing)
            {
                courses = courses.Where(x => x.StartDate < DateTime.UtcNow && x.EndDate > DateTime.UtcNow);
            }
            else if(query.Status == CourseQuery.CourseStatus.Ended)
            {
                courses = courses.Where(x => x.EndDate < DateTime.UtcNow);
            }

            if(!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                var isAscending = query.IsAscending ?? true;
                courses = query.OrderBy switch
                {
                    nameof(Course.Title) => isAscending ? courses.OrderBy(x => x.Title) : courses.OrderByDescending(x=>x.Title),
                    nameof(Course.StartDate) => isAscending?courses.OrderBy(x => x.StartDate) : courses.OrderByDescending(x=>x.StartDate),
                    _ => courses
                };
            }
                
            return await courses.ToListAsync(ct);
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

        public Task UnEnrollStudentAsync(Guid courseId, Guid studentId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        // Query
        public async Task<ICollection<CourseStudent>> GetEnrolledStudentsAsync(Guid courseId, CancellationToken ct = default)
        {
            return await context.CourseStudents.AsNoTracking()
                .Where(cs=>cs.CourseId == courseId)
                .ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetStudentCoursesAsync(Guid studentId, CancellationToken ct = default)
        {
            return await context.CourseStudents.AsNoTracking()
                .Where(cs=>cs.StudentId == studentId)
                .Select(cs=>cs.Course)
                .ToListAsync(ct);
        }
        #endregion
    }
}
