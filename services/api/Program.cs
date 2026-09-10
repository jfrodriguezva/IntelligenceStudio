using Mis.Api.Contracts;
using Mis.Api.Configuration;
using Mis.Application.Acquisition;
using Mis.Application.Football;
using Mis.Infrastructure.Acquisition;
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
builder.Services.AddScoped<IFixtureQueries, FixtureQueries>();
builder.Services.AddHealthChecks().AddDbContextCheck<MisDbContext>();
builder.Services.AddHttpClient<IManualFixtureSync, ApiFootballFixtureSync>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FootballData:ApiFootball:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-apisports-key", builder.Configuration["FootballData:ApiFootball:ApiKey"]!);
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<MisDbContext>();
    if (!await database.Fixtures.AnyAsync())
    {
        await scope.ServiceProvider.GetRequiredService<IManualFixtureSync>().ExecuteAsync(CancellationToken.None);
    }
}

app.MapGet("/api/v1/system/status", () =>
    Results.Ok(new SystemStatusResponse("Madrid Intelligence Studio", "development-foundation")))
    .WithName("GetSystemStatus")
    .Produces<SystemStatusResponse>();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.MapGet("/api/v1/fixtures", async (IFixtureQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetRecentAndUpcomingAsync(token)))
    .WithName("GetFixtures")
    .Produces<IReadOnlyList<FixtureSummary>>();

app.Run();
