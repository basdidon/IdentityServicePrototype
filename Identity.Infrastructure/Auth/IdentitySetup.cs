using Identity.Application.Interfaces;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Options;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Auth
{
    public static class IdentitySetup
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtIssuerOptions>(configuration.GetSection("Jwt"));
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtIssuerOptions>()!;

            services.AddTransient<IAuthService, JwtService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Configure JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // Parse the keys and return them as SecurityKey array
                var rsaKey = RSA.Create();
                string publicKeyPem = File.ReadAllText(jwtOptions.PublicKeyPath);
                rsaKey.ImportFromPem(publicKeyPem);

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = false,
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    //ValidIssuers = [],
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    //ValidAudiences = [],
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new RsaSecurityKey(rsaKey)
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
