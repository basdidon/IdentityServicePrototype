using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Courses.Application.Features.Courses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
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

            // create
            versionedGroup.MapPost("/instructors/{instructorId:guid}/courses", AdminCreateCourse);
            versionedGroup.MapPost("/instructors/me/courses",InstructorCreateCourse);

            // update
            versionedGroup.MapPut("/courses/{courseId:guid}", UpdateCourse);
            versionedGroup.MapPatch("/courses/{courseId:guid}/instructor", UpdateCourseInstructor);

            // delete
            versionedGroup.MapDelete("/courses/{courseId:guid}", DeleteCourse);

            // query
            versionedGroup.MapGet("/courses", GetCourses);
            versionedGroup.MapGet("/courses/{id:guid}", GetCourseById);
            // query - admin
            versionedGroup.MapGet("/students/{studentId:guid}/courses", GetCoursesByStudentId);
            versionedGroup.MapGet("/instructor/{instructorId:guid}/courses", GetCoursesByInstructorId);
            // query - instructor
            versionedGroup.MapGet("/instructor/courses", GetCoursesForInstructor);
            // query - student
            versionedGroup.MapGet("/student/courses", GetEnrolledCourses);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public static async Task<IResult> AdminCreateCourse(Guid instructorId, CreateCourseCommand command,ISender sender)
        {
            var courseId = await sender.Send(command with { InstructorId = instructorId });
            return Results.Created($"/courses/{courseId}", new { courseId });
        }

        [Authorize(Roles = ApplicationRoles.Instructor)]
        public static async Task<IResult> InstructorCreateCourse(CreateCourseCommand command,ISender sender,HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid instructorId))
            {
                var courseId = await sender.Send(command with { InstructorId = instructorId });
                return Results.Created($"/courses/{courseId}", new { courseId });
            }

            return Results.Unauthorized();
        }

        [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Instructor}")]
        public static async Task<IResult> UpdateCourse(Guid courseId, UpdateCourseCommand command, ISender sender)
        {
            await sender.Send(command with { CourseId = courseId });
            return Results.NoContent();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public static async Task<IResult> UpdateCourseInstructor(Guid courseId, UpdateCourseInstructorCommand command,ISender sender)
        {
            await sender.Send(command with { CourseId = courseId });
            return Results.NoContent();
        }

        [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Instructor}")]
        public static async Task<IResult> DeleteCourse(Guid courseId, ISender sender)
        {
            await sender.Send(new RemoveCourseCommand(courseId));
            return Results.NoContent();
        }

        // Query

        public static async Task<IResult> GetCourses([AsParameters] CourseQueryFilters query, [FromServices] ICourseService courseService)
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
        public static async Task<IResult> GetEnrolledCourses([AsParameters] CourseQueryFilters query, [FromServices] ICourseService courseService, HttpContext httpContext)
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
        public static async Task<IResult> GetCoursesByStudentId(Guid studentId, [AsParameters] CourseQueryFilters query, [FromServices] ICourseService courseService)
        {
            if (studentId == Guid.Empty)
                return Results.BadRequest("studentId can be empty");
            var courses = await courseService.GetCoursesByStudentAsync(studentId, query);
            return Results.Ok(courses);
        }

        public static async Task<IResult> GetCoursesForInstructor([AsParameters] CourseQueryFilters query, [FromServices] ICourseService courseService, HttpContext httpContext)
        {
            var instructorClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(instructorClaimId, out Guid instructorId))
            {
                var courses = await courseService.GetCoursesByInstructorAsync(instructorId, query);
                return Results.Ok(courses);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> GetCoursesByInstructorId(Guid instructorId, [AsParameters] CourseQueryFilters query, [FromServices] ICourseService courseService)
        {
            if (instructorId == Guid.Empty)
                return Results.BadRequest("instructorId can be empty");
            var courses = await courseService.GetCoursesByInstructorAsync(instructorId, query);
            return Results.Ok(courses);
        }

    }
}
