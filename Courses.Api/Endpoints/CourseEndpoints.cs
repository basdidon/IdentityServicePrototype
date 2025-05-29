using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.Abstracts;
using Courses.Application.DTOs;
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
        }

        public static async Task<IResult> CreateCourse(CreateCourseDto dto, [FromServices] ICourseService courseService, HttpContext httpContext)
        {
            var isAdmin = httpContext.User.IsInRole(ApplicationRoles.Admin);
            var isInstructor = httpContext.User.IsInRole(ApplicationRoles.Instructor);

            // Note: User can have roles
            if (dto.InstructorId != Guid.Empty)
            {
                if (!isAdmin)
                    return Results.BadRequest();

                // TODO: validate instructor exist if instructor id not admin
                var courseId = await courseService.CreateCourseAsync(dto);
                return Results.Created($"/courses/{courseId}", new { courseId });
            }
            else
            {
                if (!isInstructor)
                    return Results.BadRequest();

                if (Guid.TryParse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid instructorId))
                {
                    // use user-id from claim to refer
                    var courseId = await courseService.CreateCourseAsync(dto with { InstructorId = instructorId });
                    return Results.Created($"/courses/{courseId}", new { courseId });
                }
            }

            return Results.Forbid();
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
    }
}
