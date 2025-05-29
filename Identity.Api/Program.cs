using Identity.Api.Configurations;
using Identity.Application.Configurations;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Configurations;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication()
    .AddInfrastucture(builder.Configuration)
    .AddPresentation()
    .AddEndpoints(typeof(Program).Assembly)
    .AddJwtAuth(builder.Configuration);

var app = builder.Build();

app.UsePresentation();
app.UseInfrastructure();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
