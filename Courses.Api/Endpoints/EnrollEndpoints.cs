using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using System.Security.Claims;

namespace Courses.Api.Endpoints
{
    public static class EnrollEndpoints
    {
        public static void MapEnrollEndpoints(this IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapPost("/courses/{courseId}/enroll", Enroll)
                .RequireAuthorization();

            versionedGroup.MapPost("/courses/{courseId}/unenroll", Unenroll)
                .RequireAuthorization();
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> Enroll(Guid courseId, [FromServices] IEnrollmentService enrollmentService, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                await enrollmentService.EnrollStudentAsync(courseId, studentId);
                return Results.Accepted();
            }

            return Results.Unauthorized();
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> Unenroll(Guid courseId, [FromServices] IEnrollmentService enrollmentService, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                await enrollmentService.RemoveEnrollmentAsync(courseId,studentId);
                return Results.Accepted();
            }

            return Results.Unauthorized();
        }
    }
}
