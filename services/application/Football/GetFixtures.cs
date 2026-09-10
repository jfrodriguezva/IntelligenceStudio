namespace Mis.Application.Football;

public interface IFixtureQueries
{
    Task<IReadOnlyList<FixtureSummary>> GetRecentAndUpcomingAsync(CancellationToken cancellationToken);

    Task<FixtureSummary?> GetByIdAsync(Guid fixtureId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamMatchStatisticSummary>> GetStatisticsAsync(Guid fixtureId, CancellationToken cancellationToken);
}

public sealed record FixtureSummary(
    Guid Id,
    string Competition,
    string Season,
    Guid HomeTeamId,
    string HomeTeam,
    Guid AwayTeamId,
    string AwayTeam,
    DateTimeOffset? KickoffUtc,
    string Status,
    int? HomeGoals,
    int? AwayGoals);

public sealed record TeamMatchStatisticSummary(string Team, int? PossessionPercent, int? Shots, int? ShotsOnTarget, int? Corners);
