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
builder.Services.AddScoped<IMatchEventQueries, MatchEventQueries>();
builder.Services.AddScoped<IMatchNoteService, MatchNoteService>();
builder.Services.AddScoped<ITacticalSceneService, TacticalSceneService>();
builder.Services.AddSingleton<IEvidenceStorage>(_ => new LocalEvidenceStorage(Path.Combine(builder.Environment.ContentRootPath, ".local-data", "evidence")));
builder.Services.AddScoped<IEvidenceAssetService, EvidenceAssetService>();
builder.Services.AddScoped<IMatchPredictionService, MatchPredictionService>();
builder.Services.AddScoped<ISyncRunQueries, SyncRunQueries>();
builder.Services.AddHealthChecks().AddDbContextCheck<MisDbContext>();
builder.Services.AddHttpClient<IManualFixtureSync, ApiFootballFixtureSync>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FootballData:ApiFootball:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-apisports-key", builder.Configuration["FootballData:ApiFootball:ApiKey"]!);
});
builder.Services.AddHttpClient<IBaselinePredictionRuntime, BaselinePredictionRuntime>(client => client.BaseAddress = new Uri(builder.Configuration["MachineLearning:BaseUrl"] ?? "http://localhost:8000"));

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

app.MapGet("/api/v1/fixtures", async (string? competition, string? season, string? status, DateOnly? from, DateOnly? to, int? page, int? pageSize, IFixtureQueries queries, CancellationToken token) =>
    Results.Ok(await queries.SearchAsync(new FixtureSearch(competition, season, status, from, to, page ?? 1, pageSize ?? 20), token)))
    .WithName("GetFixtures")
    .Produces<FixturePage>();

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

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/events", async (Guid fixtureId, IMatchEventQueries queries, CancellationToken token) =>
    Results.Ok(await queries.GetForFixtureAsync(fixtureId, token)))
    .WithName("GetFixtureEvents")
    .Produces<IReadOnlyList<MatchEventSummary>>();

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/predictions/latest", async (Guid fixtureId, IMatchPredictionService predictions, CancellationToken token) =>
{
    var prediction = await predictions.GetLatestAsync(fixtureId, token);
    return prediction is null ? Results.NotFound() : Results.Ok(prediction);
}).WithName("GetLatestPrediction").Produces<MatchPredictionSummary>().Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/v1/fixtures/{fixtureId:guid}/predictions/baseline", async (Guid fixtureId, HttpRequest request, IConfiguration configuration, IMatchPredictionService predictions, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(request.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    try { return Results.Created($"/api/v1/fixtures/{fixtureId}/predictions/latest", await predictions.GenerateBaselineAsync(fixtureId, token)); }
    catch (KeyNotFoundException) { return Results.NotFound(); }
    catch (HttpRequestException) { return Results.Problem("El runtime de ML no está disponible.", statusCode: StatusCodes.Status503ServiceUnavailable); }
}).WithName("GenerateBaselinePrediction").Produces<MatchPredictionSummary>(StatusCodes.Status201Created).Produces(StatusCodes.Status401Unauthorized).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status503ServiceUnavailable);

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/notes", async (Guid fixtureId, HttpRequest request, IConfiguration configuration, IMatchNoteService notes, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(request.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    return Results.Ok(await notes.GetAsync(fixtureId, token));
})
    .WithName("GetMatchNotes")
    .Produces<IReadOnlyList<MatchNoteSummary>>()
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/v1/fixtures/{fixtureId:guid}/notes", async (Guid fixtureId, CreateMatchNoteRequest request, HttpRequest httpRequest, IConfiguration configuration, IMatchNoteService notes, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(httpRequest.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    try { return Results.Created($"/api/v1/fixtures/{fixtureId}/notes", await notes.CreateAsync(fixtureId, request, token)); }
    catch (KeyNotFoundException) { return Results.NotFound(); }
})
    .WithName("CreateMatchNote")
    .Produces<MatchNoteSummary>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/tactical-scenes", async (Guid fixtureId, HttpRequest request, IConfiguration configuration, ITacticalSceneService scenes, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(request.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    return Results.Ok(await scenes.GetForFixtureAsync(fixtureId, token));
})
    .WithName("GetTacticalScenes")
    .Produces<IReadOnlyList<TacticalSceneSummary>>()
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/v1/fixtures/{fixtureId:guid}/tactical-scenes", async (Guid fixtureId, CreateTacticalSceneRequest request, HttpRequest httpRequest, IConfiguration configuration, ITacticalSceneService scenes, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(httpRequest.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    try { return Results.Created($"/api/v1/fixtures/{fixtureId}/tactical-scenes", await scenes.CreateAsync(fixtureId, request, token)); }
    catch (KeyNotFoundException) { return Results.NotFound(); }
    catch (ArgumentException exception) { return Results.BadRequest(new { error = exception.Message }); }
})
    .WithName("CreateTacticalScene")
    .Produces<TacticalSceneSummary>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/v1/fixtures/{fixtureId:guid}/evidence", async (Guid fixtureId, HttpRequest request, IConfiguration configuration, IEvidenceAssetService evidence, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(request.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    return Results.Ok(await evidence.GetForFixtureAsync(fixtureId, token));
})
    .WithName("GetFixtureEvidence")
    .Produces<IReadOnlyList<EvidenceAssetSummary>>()
    .Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/v1/fixtures/{fixtureId:guid}/evidence", async (Guid fixtureId, HttpRequest request, IFormFile? file, int? minute, string? description, IConfiguration configuration, IEvidenceAssetService evidence, CancellationToken token) =>
{
    if (!AdminRefreshAuthorization.IsAuthorized(request.Headers[AdminRefreshAuthorization.HeaderName].ToString(), configuration["AdminAccess:RefreshKey"])) return Results.Unauthorized();
    if (file is null || !EvidenceUploadValidation.IsAllowed(file) || string.IsNullOrWhiteSpace(description)) return Results.BadRequest(new { error = "Archivo o metadatos de evidencia no válidos." });
    try
    {
        await using var content = file.OpenReadStream();
        return Results.Created($"/api/v1/fixtures/{fixtureId}/evidence", await evidence.CreateAsync(fixtureId, new CreateEvidenceAssetRequest(minute ?? 0, description, Path.GetFileName(file.FileName), file.ContentType.ToLowerInvariant(), file.Length, content), token));
    }
    catch (KeyNotFoundException) { return Results.NotFound(); }
    catch (ArgumentException exception) { return Results.BadRequest(new { error = exception.Message }); }
})
    .WithName("CreateFixtureEvidence")
    .Accepts<IFormFile>("multipart/form-data")
    .Produces<EvidenceAssetSummary>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

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
