namespace Mis.Application.Football;

public interface IFixtureQueries
{
    Task<IReadOnlyList<FixtureSummary>> GetRecentAndUpcomingAsync(CancellationToken cancellationToken);
}

public sealed record FixtureSummary(
    Guid Id,
    string Competition,
    string Season,
    string HomeTeam,
    string AwayTeam,
    DateTimeOffset? KickoffUtc,
    string Status,
    int? HomeGoals,
    int? AwayGoals);
