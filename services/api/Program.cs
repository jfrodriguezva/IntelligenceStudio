using Mis.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/api/v1/system/status", () =>
    Results.Ok(new SystemStatusResponse("Madrid Intelligence Studio", "development-foundation")))
    .WithName("GetSystemStatus")
    .Produces<SystemStatusResponse>();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();