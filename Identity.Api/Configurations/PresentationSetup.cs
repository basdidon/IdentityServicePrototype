﻿using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Identity.Api.Endpoints;
using Microsoft.OpenApi.Models;

namespace Identity.Api.Configurations
{
    public static class PresentationSetup
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // Endpoints
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static void UsePresentation(this WebApplication app)
        {
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.MapWellKnownEndpoint();

            app.MapAuthencationEndpoints();
            app.MapUserEndpoints();

            app.MapAdminOnlyEndpoints();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();
                    foreach (var description in descriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"API {description.GroupName.ToUpperInvariant()}");
                    }
                });
            }
        }
    }
}
