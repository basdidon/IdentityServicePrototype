using Courses.Api.Configurations;
using Courses.Api.Endpoints;
using Courses.Application.Configurations;
using Courses.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentaion()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UsePresentaion();
app.UseInfrastructure();
app.MapCourseEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.Run();
