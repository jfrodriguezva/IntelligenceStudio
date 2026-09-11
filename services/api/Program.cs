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
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("MisDatabase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:MisDatabase is required.");
}

builder.Services.AddDbContext<MisDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IFixtureQueries, FixtureQueries>();
builder.Services.AddScoped<ITeamFormQueries, TeamFormQueries>();
builder.Services.AddScoped<ISquadQueries, SquadQueries>();
builder.Services.AddScoped<ISyncRunQueries, SyncRunQueries>();
builder.Services.AddHealthChecks().AddDbContextCheck<MisDbContext>();
builder.Services.AddHttpClient<IManualFixtureSync, ApiFootballFixtureSync>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FootballData:ApiFootball:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-apisports-key", builder.Configuration["FootballData:ApiFootball:ApiKey"]!);
});

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();

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

app.MapGet("/api/v1/fixtures/{fixtureId:guid}", async (Guid fixtureId, IFixtureQueries queries, CancellationToken token) =>
{
    var fixture = await queries.GetByIdAsync(fixtureId, token);
    return fixture is null ? Results.NotFound() : Results.Ok(fixture);
})
    .WithName("GetFixtureById")
    .Produces<FixtureSummary>()
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/form", async (Guid fixtureId, ITeamFormQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetForFixtureAsync(fixtureId, token)))
    .WithName("GetFixtureTeamForm")
    .Produces<IReadOnlyList<TeamFormSummary>>();

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/statistics", async (Guid fixtureId, IFixtureQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetStatisticsAsync(fixtureId, token)))
    .WithName("GetFixtureStatistics")
    .Produces<IReadOnlyList<TeamMatchStatisticSummary>>();

app.MapGet("/api/v1/teams/{teamId:guid}/squad", async (Guid teamId, ISquadQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetForTeamAsync(teamId, token)))
    .WithName("GetTeamSquad")
    .Produces<IReadOnlyList<SquadPlayerSummary>>();

app.MapGet("/api/v1/acquisition/sync-runs", async (int? take, ISyncRunQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetRecentAsync(take ?? 10, token)))
    .WithName("GetRecentSyncRuns")
    .Produces<IReadOnlyList<SyncRunSummary>>();

app.MapPost("/api/v1/acquisition/fixtures/refresh", async (HttpRequest request, IConfiguration configuration, IManualFixtureSync sync, CancellationToken token) =>
{
    var suppliedKey = request.Headers[AdminRefreshAuthorization.HeaderName].ToString();
    var configuredKey = configuration["AdminAccess:RefreshKey"];
    if (!AdminRefreshAuthorization.IsAuthorized(suppliedKey, configuredKey)) return Results.Unauthorized();
    return Results.Ok(await sync.ExecuteAsync(token));
})
    .WithName("RefreshFixtures")
    .Produces<ManualFixtureSyncResult>()
    .Produces(StatusCodes.Status401Unauthorized);

app.Run();
