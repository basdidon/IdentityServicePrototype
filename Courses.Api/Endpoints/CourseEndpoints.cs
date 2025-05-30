using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using System.Security.Claims;

namespace Courses.Api.Endpoints
{
    public static class CourseEndpoints
    {
        public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapPost("/courses", CreateCourse)
                .RequireAuthorization("CreateCourse");

            versionedGroup.MapPut("/courses/{id:guid}", UpdateCourse);

            versionedGroup.MapDelete("/courses/{id:guid}", DeleteCourse);

            versionedGroup.MapGet("/courses/{id:guid}", GetCourseById);
            versionedGroup.MapGet("/courses", GetCourses);
            versionedGroup.MapGet("/student/courses", GetEnrolledCourses);
            versionedGroup.MapGet("/students/{studentId:guid}/courses", GetCoursesByStudentId);
            versionedGroup.MapGet("/instructor/courses", GetCoursesForInstructor);
            versionedGroup.MapGet("/instructor/{instructorId:guid}/courses", GetCoursesByInstructorId);
        }

        public static async Task<IResult> CreateCourse(CreateCourseDto dto, [FromServices] ICourseService courseService, HttpContext httpContext)
        {
            var user = httpContext.User;
            var isAdmin = user.IsInRole(ApplicationRoles.Admin);
            var isInstructor = user.IsInRole(ApplicationRoles.Instructor);

            // Note: User can have more than one role
            if (dto.InstructorId != Guid.Empty)
            {
                if (!isAdmin)
                    return Results.Forbid();

                // TODO: validate instructor exist if instructor id not admin
                var courseId = await courseService.CreateCourseAsync(dto);
                return Results.Created($"/courses/{courseId}", new { courseId });
            }

            if (!isInstructor)
                return Results.Forbid();

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid instructorId))
            {
                // use user-id from claim to refer
                var courseId = await courseService.CreateCourseAsync(dto with { InstructorId = instructorId });
                return Results.Created($"/courses/{courseId}", new { courseId });
            }

            return Results.Unauthorized(); // cannot get user ID
        }

        public static async Task<IResult> UpdateCourse(Guid id, UpdateCourseDto dto, [FromServices] ICourseService courseService)
        {
            await courseService.UpdateCourseAsync(id, dto);
            return Results.NoContent();
        }

        public static async Task<IResult> DeleteCourse(Guid id, [FromServices] ICourseService courseService)
        {
            await courseService.DeleteCourseAsync(id);
            return Results.NoContent();
        }

        public static async Task<IResult> GetCourses([AsParameters] CourseQuery query, [FromServices] ICourseService courseService)
        {
            var courses = await courseService.GetCoursesAsync(query);
            return Results.Ok(courses);
        }

        public static async Task<IResult> GetCourseById(Guid id, [FromServices] ICourseService courseService)
        {
            var course = await courseService.GetCourseByIdAsync(id);
            return course is null ? Results.NotFound() : Results.Ok(course);
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> GetEnrolledCourses([AsParameters] CourseQuery query, [FromServices] ICourseService courseService, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                var courses = await courseService.GetCoursesByStudentAsync(studentId, query);
                return Results.Ok(courses);
            }

            return Results.Unauthorized();
        }

        [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Instructor}")]
        public static async Task<IResult> GetCoursesByStudentId(Guid studentId, [AsParameters] CourseQuery query, [FromServices] ICourseService courseService)
        {
            if (studentId == Guid.Empty)
                return Results.BadRequest("studentId can be empty");
            var courses = await courseService.GetCoursesByStudentAsync(studentId, query);
            return Results.Ok(courses);
        }

        public static async Task<IResult> GetCoursesForInstructor([AsParameters] CourseQuery query, [FromServices] ICourseService courseService, HttpContext httpContext)
        {
            var instructorClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(instructorClaimId, out Guid instructorId))
            {
                var courses = await courseService.GetCoursesByInstructorAsync(instructorId, query);
                return Results.Ok(courses);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> GetCoursesByInstructorId(Guid instructorId, [AsParameters] CourseQuery query, [FromServices] ICourseService courseService)
        {
            if (instructorId == Guid.Empty)
                return Results.BadRequest("instructorId can be empty");
            var courses = await courseService.GetCoursesByInstructorAsync(instructorId, query);
            return Results.Ok(courses);
        }

    }
}
