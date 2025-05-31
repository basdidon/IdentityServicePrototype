using Asp.Versioning;
using Asp.Versioning.Builder;
using Courses.Application.Features.Courses.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

            versionedGroup.MapPost("/courses/{courseId}/enroll", Enroll);
            versionedGroup.MapPost("/courses/{courseId}/unenroll", Unenroll);
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> Enroll(Guid courseId, ISender sender, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                await sender.Send(new EnrollStudentCommand(courseId, studentId));
                return Results.Accepted();
            }

            return Results.Unauthorized();
        }

        [Authorize(Roles = ApplicationRoles.Student)]
        public static async Task<IResult> Unenroll(Guid courseId, ISender sender, HttpContext httpContext)
        {
            var studentClaimId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(studentClaimId, out Guid studentId))
            {
                await sender.Send(new UnEnrollStudentCommand(courseId, studentId));
                return Results.Accepted();
            }

            return Results.Unauthorized();
        }
    }
}
