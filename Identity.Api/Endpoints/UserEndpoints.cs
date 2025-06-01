using Asp.Versioning.Builder;
using Asp.Versioning;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapGet("user/me", Profile)
                .RequireAuthorization();
        }

        public static async Task<IResult> Profile(HttpContext httpContext, UserManager<ApplicationUser> userManager)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                return Results.NotFound();

            return Results.Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }
    }
}
