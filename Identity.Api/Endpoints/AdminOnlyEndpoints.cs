using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;
using Shared.Constants;

namespace Identity.Api.Endpoints
{
    public static class AdminOnlyEndpoints
    {
        public static void MapAdminOnlyEndpoints(this IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapGet("/admin-only", [Authorize(Roles = ApplicationRoles.Admin)] () =>
            {
                return Results.Ok("hello, Admin");
            });
        }
    }
}
