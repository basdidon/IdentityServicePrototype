using Courses.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
    }
}
