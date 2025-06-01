using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.DTOs;
using Courses.Application.Features.Courses.Commands;
using Courses.Application.Features.Courses.Queries;
using Courses.Application.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
            versionedGroup.MapPost("/instructors/{instructorId:guid}/courses", AdminCreateCourse)
                .WithSummary("Admin creates course with assigned instructor.");
            versionedGroup.MapPost("/instructors/me/courses", InstructorCreateCourse)
                .WithSummary("Instructor creates their own course.");

            // update
            versionedGroup.MapPut("/courses/{courseId:guid}", UpdateCourse)
                .WithSummary("Admin or Instructor update course");
            versionedGroup.MapPatch("/courses/{courseId:guid}/instructor", UpdateCourseInstructor)
                .WithSummary("Admin update instructor");

            // delete
            versionedGroup.MapDelete("/courses/{courseId:guid}", DeleteCourse);

            // query
            versionedGroup.MapGet("/courses", GetCourses);
            versionedGroup.MapGet("/courses/{courseId:guid}", GetCourseById);
            versionedGroup.MapGet("/courses/{courseId:guid}/enrolled-students", GetEnrolledStudents);

            // query - admin
            versionedGroup.MapGet("/students/{studentId:guid}/courses", GetCoursesByStudentId);
            versionedGroup.MapGet("/instructors/{instructorId:guid}/courses", GetCoursesByInstructorId);
            // query - instructor
            versionedGroup.MapGet("/instructor/courses", GetCoursesForInstructor);
            // query - student
            versionedGroup.MapGet("/student/courses", GetEnrolledCourses);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public static async Task<IResult> AdminCreateCourse(Guid instructorId, CreateCourseCommand command, ISender sender)
        {
            var courseId = await sender.Send(command with { InstructorId = instructorId });
            return Results.Created($"/courses/{courseId}", new { courseId });
        }

        [Authorize(Roles = ApplicationRoles.Instructor)]
        public static async Task<IResult> InstructorCreateCourse(CreateCourseCommand command, ISender sender, HttpContext httpContext)
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
        public static async Task<IResult> UpdateCourse(Guid courseId, UpdateCourseCommand command, ISender sender, HttpContext httpContext)
        {
            // Step 1: Get the course
            var course = await sender.Send(new CourseByIdQuery(courseId));
            if (course is null)
                return Results.NotFound();

            // Step 2: If admin, allow
            if (httpContext.User.IsInRole(ApplicationRoles.Admin))
            {
                await sender.Send(command with { CourseId = courseId });
                return Results.NoContent();
            }

            // Step 3: Instructor role — check ownership
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid instructorId))
                return Results.Unauthorized();

            if (course.InstructorId != instructorId)
                return Results.Forbid();

            await sender.Send(command with { CourseId = courseId });
            return Results.NoContent();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public static async Task<IResult> UpdateCourseInstructor(Guid courseId, UpdateCourseInstructorCommand command, ISender sender)
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

        public static async Task<IResult> GetCourses([AsParameters] CourseQueryFilters queryFilters, ISender sender)
        {
            var courses = await sender.Send(queryFilters.ToCoursesQuery());
            return Results.Ok(courses);
        }

        public static async Task<IResult> GetCourseById(Guid courseId, ISender sender)
        {
            var course = await sender.Send(new CourseByIdQuery(courseId));
            return course is null ? Results.NotFound() : Results.Ok(course);
        }

        public static async Task<IResult> GetEnrolledStudents(Guid courseId, ISender sender)
        {
            var students = await sender.Send(new EnrolledStudentsQuery(courseId));
            return Results.Ok(students);
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> GetEnrolledCourses([AsParameters] CourseQueryFilters queryFilters, ISender sender, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                var courses = await sender.Send(queryFilters.ToCoursesByStudentIdQuery(studentId));
                return Results.Ok(courses);
            }

            return Results.Unauthorized();
        }

        [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Instructor}")]
        public static async Task<IResult> GetCoursesByStudentId(Guid studentId, [AsParameters] CourseQueryFilters queryFilters, ISender sender)
        {
            if (studentId == Guid.Empty)
                return Results.BadRequest("studentId can be empty");
            var courses = await sender.Send(queryFilters.ToCoursesByStudentIdQuery(studentId));
            return Results.Ok(courses);
        }

        public static async Task<IResult> GetCoursesForInstructor([AsParameters] CourseQueryFilters queryFilters, ISender sender, HttpContext httpContext)
        {
            var instructorClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(instructorClaimId, out Guid instructorId))
            {
                var courses = await sender.Send(queryFilters.ToCoursesByInstructorIdQuery(instructorId));
                return Results.Ok(courses);
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> GetCoursesByInstructorId(Guid instructorId, [AsParameters] CourseQueryFilters queryFilters, ISender sender)
        {
            if (instructorId == Guid.Empty)
                return Results.BadRequest("instructorId can be empty");
            var courses = await sender.Send(queryFilters.ToCoursesByInstructorIdQuery(instructorId));
            return Results.Ok(courses);
        }

    }
}
