using Asp.Versioning;
using Asp.Versioning.Builder;
using Identity.Application.DTOs;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Identity.Api.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthencationEndpoints(this IEndpointRouteBuilder app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapPost("/register", Register);
            versionedGroup.MapPost("/login", Login);
            versionedGroup.MapPost("/logout", Logout);
            versionedGroup.MapPost("/refresh", Refresh);
        }

        public static Task<IResult> Register()
        {
            throw new NotImplementedException();
        }

        public static async Task<IResult> Login(LoginRequestDto dto, IUserService userService)
        {
            var result = await userService.LoginAsync(dto.Username, dto.Password);

            if (result is null)
            {
                return Results.Unauthorized();
            }

            var (accessToken, refreshToken) = result.Value;

            return Results.Ok(new
            {
                accessToken,
                refreshToken = RefreshTokenDto.Map(refreshToken)
            });
        }

        [Authorize]
        public static async Task<IResult> Logout(IUserService userService, HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is not null)
            {
                await userService.LogoutAsync(userId);
            }

            return Results.Ok();
        }

        public static async Task<IResult> Refresh(RefreshRequestDto dto, IUserService userService, HttpContext httpContext)
        {
            var result = await userService.RefreshTokenAsync(dto.RefreshToken);

            if (result is null)
            {
                return Results.Unauthorized();
            }

            var (newAccessToken,newRefreshToken) = result.Value;

            // Return new tokens
            return Results.Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = RefreshTokenDto.Map(newRefreshToken)
            });
        }
    }
}
