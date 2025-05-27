using Courses.Application.Abstracts;
using Courses.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Constants;

namespace Courses.Api.Endpoints
{
    public class CourseEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/courses", [Authorize(Roles = $"{ApplicationRoles.Admin},{ApplicationRoles.Teacher}")]
            async (CreateCourseDto dto, ICourseService courseService) =>
            {
                var courseId = await courseService.CreateCourseAsync(dto);
                return Results.Created("/courses/{}", new { courseId });
            }).RequireAuthorization();

            app.MapPut("/courses/{id:guid}", async (
                Guid id,
                UpdateCourseDto dto,
                [FromServices] ICourseService courseService) =>
            {
                await courseService.UpdateCourseAsync(id, dto);
                return Results.NoContent();
            });

            app.MapDelete("/courses/{id:guid}", async (
                Guid id,
                [FromServices] ICourseService courseService) =>
            {
                await courseService.DeleteCourseAsync(id);
                return Results.NoContent();
            });

            app.MapGet("/courses/{id:guid}", async (
                Guid id,
                [FromServices] ICourseService courseService) =>
            {
                var course = await courseService.GetCourseByIdAsync(id);
                return course is null ? Results.NotFound() : Results.Ok(course);
            });

            app.MapGet("/courses", async (
                [AsParameters] CourseQuery query,
                [FromServices] ICourseService courseService) =>
            {
                var courses = await courseService.GetCoursesAsync(query);
                return Results.Ok(courses);
            });
        }
    }
}
