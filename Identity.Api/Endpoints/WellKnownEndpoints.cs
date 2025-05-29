using Identity.Application.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Identity.Api.Endpoints
{
    public static class WellKnownEndpoints
    {
        public static void MapWellKnownEndpoint(this WebApplication app)
        {
            // Serve .well-known/openid-configuration
            app.MapGet("/.well-known/openid-configuration", (HttpContext ctx) =>
            {
                var baseUrl = $"http://identity.api:8080";
                return Results.Json(new
                {
                    issuer = "identity.api",
                    jwks_uri = $"{baseUrl}/.well-known/jwks.json"
                });
            });

            app.MapGet("/.well-known/jwks.json", (IAuthService authService,IOptions<JwtIssuerOptions> options) =>
            {
                Console.WriteLine("[Get] jwks.json");

                var jwk = authService.GetJsonWebKey();

                // Create JWKS (JSON Web Key Set)
                var jwks = new
                {
                    keys = new[] { jwk }
                };

                // Return the JWKS as JSON
                return Results.Ok(jwks);
            });


        }
    }
}
