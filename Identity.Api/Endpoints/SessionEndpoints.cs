using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared;
using System.Security.Claims;

namespace Identity.Api.Endpoints
{

    public class SessionEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            // Register new users (create a user resource)
            app.MapPost("users", () => { });

            // Create a new session (login & issue JWT)
            app.MapPost("session", async (
                string username,
                string password,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IRefreshTokenRepository refreshTokenRepo,
                IAuthService authService) =>
            {
                var user = await userManager.FindByNameAsync(username);
                if (user is null)
                {
                    return Results.Unauthorized();
                }

                var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
                if (!result.Succeeded)
                {
                    return Results.Unauthorized();
                }

                var roles = await userManager.GetRolesAsync(user);

                var accessToken = authService.GenerateAccessToken(user.Id,username, [.. roles]);
                var refreshToken = authService.GenerateRefreshToken();

                await refreshTokenRepo.SaveRefreshTokenAsync(user.Id, refreshToken);

                return Results.Ok(new { accessToken, refreshToken = RefreshTokenDto.Map(refreshToken) });
            });

            // Destroy the session (logout - optional)
            app.MapDelete("session", async (HttpContext httpContext, UserManager<ApplicationUser> userManager) =>
            {
                // revock refresh token
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

            }).RequireAuthorization();

            // Refresh an access token (optional)
            app.MapPost("session/refresh", () => { });
        }
    }
}
