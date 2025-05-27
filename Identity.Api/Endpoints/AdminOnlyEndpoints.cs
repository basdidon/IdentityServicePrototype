using Microsoft.AspNetCore.Authorization;
using Shared;
using Shared.Constants;

namespace Identity.Api.Endpoints
{
    public class AdminOnlyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            // [Authorize(Roles = "Admin,OtherRole")] for more than 1 role
            app.MapGet("/admin-only", [Authorize(Roles = ApplicationRoles.Admin)] () =>
            {
                return Results.Ok("hello, Admin");
            });
        }
    }
}
