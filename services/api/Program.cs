using Mis.Api.Contracts;
using Mis.Api.Configuration;
using Mis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

EncryptedSecretsLoader.AddIfPresent(builder.Configuration, builder.Environment.ContentRootPath);

builder.Services.AddProblemDetails();
var connectionString = builder.Configuration.GetConnectionString("MisDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:MisDatabase is required.");
}

builder.Services.AddDbContext<MisDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHealthChecks().AddDbContextCheck<MisDbContext>();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/api/v1/system/status", () =>
    Results.Ok(new SystemStatusResponse("Madrid Intelligence Studio", "development-foundation")))
    .WithName("GetSystemStatus")
    .Produces<SystemStatusResponse>();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
