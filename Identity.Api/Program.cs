using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Builder;
using Identity.Application.Extensions;
using Identity.Infrastructure;
using Identity.Infrastructure.Auth;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shared;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));

// Add services to the container.
builder.Services.AddApplication()
    .AddInfrastucture();

// Endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddJwtAuth(builder.Configuration);

var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/.well-known/jwks.json", (IOptions<JwtAuthenticationSettings> options) =>
{
    Console.WriteLine("[Get] jwks.json");
    // Load the private key from the privatekey.pem
    string privateKeyPem = System.IO.File.ReadAllText(options.Value.PrivateKeyPath);
    using RSA rsa = RSA.Create();
    // Import the private key from XML
    rsa.ImportFromPem(privateKeyPem);

    // Extract the public key from the private key
    RSAParameters publicKeyParameters = rsa.ExportParameters(false); // 'false' means public key only

    // Convert the public key to JWKS format
    var jwk = new
    {
        kty = "RSA",
        use = "sig",
        kid = "your-key-id",  // Key identifier (set a unique ID for your key)
        e = Base64UrlEncode(publicKeyParameters.Exponent!),
        n = Base64UrlEncode(publicKeyParameters.Modulus!),
        alg = "RS256"
    };

    // Create JWKS (JSON Web Key Set)
    var jwks = new
    {
        keys = new[] { jwk }
    };

    // Return the JWKS as JSON
    return Results.Ok(jwks);

    // Helper method to base64url encode without padding
    static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
});




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

app.UseInfrastructure();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
