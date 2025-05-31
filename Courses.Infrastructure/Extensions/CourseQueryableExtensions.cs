using Courses.Core.Entities;
using Courses.Core.Enums;
using Courses.Infrastructure.Extensions;

namespace Courses.Infrastructure.Extensions
{
    internal static class CourseQueryableExtensions
    {
        public static IQueryable<Course> ByStatus(this IQueryable<Course> courses, CourseStatus courseStatus)
        {
            var now = DateTime.Now;
            // status
            if (courseStatus == CourseStatus.NotStarted)
            {
                courses = courses.Where(x => x.StartDate > now);
            }
            else if (courseStatus == CourseStatus.Ongoing)
            {
                courses = courses.Where(x => x.StartDate < now && x.EndDate > now);
            }
            else if (courseStatus == CourseStatus.Ended)
            {
                courses = courses.Where(x => x.EndDate < now);
            }

            return courses;
        }

        public static IQueryable<Course> OrderBy(this IQueryable<Course> courses, string orderBy, bool isAscending = true)
        {
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                courses = orderBy switch
                {
                    nameof(Course.Title) => isAscending ? courses.OrderBy(x => x.Title) : courses.OrderByDescending(x => x.Title),
                    nameof(Course.StartDate) => isAscending ? courses.OrderBy(x => x.StartDate) : courses.OrderByDescending(x => x.StartDate),
                    _ => courses
                };
            }
            return courses;
        }

        public static IQueryable<Course> ToPage(this IQueryable<Course> courses, int pageNumber, int pageSize)
        {
            if (pageNumber > 0 && pageSize > 0)
            {
                int skip = (pageNumber - 1) * pageSize;
                courses = courses.Skip(skip).Take(pageSize);
            }

            return courses;
        }

    }
}
