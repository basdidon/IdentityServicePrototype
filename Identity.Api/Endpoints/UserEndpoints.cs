using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Shared;
using System.Security.Claims;

namespace Identity.Api.Endpoints
{
    public class UserEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("user/me", async (HttpContext httpContext,UserManager<ApplicationUser> userManager) => {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"userId ; {userId}");
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
            }).RequireAuthorization();
        }
    }
}
