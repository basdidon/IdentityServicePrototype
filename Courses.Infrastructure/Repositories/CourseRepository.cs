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

        private static IQueryable<Course> ApplyCourseQuery(IQueryable<Course> courses,CourseQuery query)
        {
            var now = DateTime.Now;
            // status
            if (query.Status == CourseQuery.CourseStatus.NotStarted)
            {
                courses = courses.Where(x => x.StartDate > now);
            }
            else if (query.Status == CourseQuery.CourseStatus.Ongoing)
            {
                courses = courses.Where(x => x.StartDate < now && x.EndDate > now);
            }
            else if (query.Status == CourseQuery.CourseStatus.Ended)
            {
                courses = courses.Where(x => x.EndDate < now);
            }

            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                var isAscending = query.IsAscending ?? true;
                courses = query.OrderBy switch
                {
                    nameof(Course.Title) => isAscending ? courses.OrderBy(x => x.Title) : courses.OrderByDescending(x => x.Title),
                    nameof(Course.StartDate) => isAscending ? courses.OrderBy(x => x.StartDate) : courses.OrderByDescending(x => x.StartDate),
                    _ => courses
                };
            }

            if (query.Page.HasValue && query.PageSize.HasValue && query.Page > 0 && query.PageSize > 0)
            {
                int skip = (query.Page.Value - 1) * query.PageSize.Value;
                courses = courses.Skip(skip).Take(query.PageSize.Value);
            }

            return courses;
        }

        // Query
        public async Task<ICollection<Course>> GetCoursesAsync(CourseQuery query, CancellationToken ct = default)
        {
            var courses = context.Courses.AsQueryable()
                .AsNoTracking();
            var filteredCourses = ApplyCourseQuery(courses, query);
            return await filteredCourses.ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetCoursesByStudentAsync(Guid studentId,CourseQuery courseQuery, CancellationToken ct= default)
        {
            var courses = context.CourseStudents.AsQueryable()
                .AsNoTracking()
                .Where(x => x.StudentId == studentId)
                .Select(x => x.Course);
            var filteredCourses = ApplyCourseQuery(courses, courseQuery);
            return await filteredCourses.ToListAsync(ct);
        }

        public async Task<ICollection<Course>> GetCoursesByInstructorAsync(Guid instructorId,CourseQuery courseQuery, CancellationToken ct = default)
        {
            var courses = context.Courses.AsQueryable()
                .AsNoTracking()
                .Where(x => x.InstructorId == instructorId);
            var filteredCourses = ApplyCourseQuery(courses, courseQuery);
            return await filteredCourses.ToListAsync(ct);
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
