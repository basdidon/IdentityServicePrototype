using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Shared;
using System.Security.Claims;

namespace Identity.Api.Endpoints
{
    public static class SessionEndpoints
    {
        public static void MapSessionEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("users", () => { });
            app.MapPost("/login", Login);
            app.MapPost("/logout", Logout);
            app.MapPost("/refresh", Refresh);
        }

        public static async Task<IResult> Login(
                LoginRequestDto dto,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IRefreshTokenRepository refreshTokenRepo,
                IAuthService authService)
        {
            var user = await userManager.FindByNameAsync(dto.Username);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);

            var accessToken = authService.GenerateAccessToken(user.Id, dto.Username, [.. roles]);
            var refreshToken = authService.GenerateRefreshToken();

            await refreshTokenRepo.SaveRefreshTokenAsync(user.Id, refreshToken);

            return Results.Ok(new { accessToken, refreshToken = RefreshTokenDto.Map(refreshToken) });
        }

        [Authorize]
        public static async Task<IResult> Logout(HttpContext httpContext, UserManager<ApplicationUser> userManager,IRefreshTokenRepository refreshTokenRepo)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                return Results.Unauthorized();

            // TODO:
            // - find refresh token by userId
            // - revock refresh token 

            return Results.Ok();
        }

        public static async Task<IResult> Refresh(RefreshRequestDto dto ,HttpContext httpContext)
        {
            return await Task.FromResult(Results.Ok());
        }
    }
}
