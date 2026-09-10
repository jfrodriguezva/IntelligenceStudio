namespace Mis.Domain.Football;

public sealed class Fixture
{
    private Fixture()
    {
    }

    public Fixture(
        Guid id,
        Guid seasonId,
        Guid homeTeamId,
        Guid awayTeamId,
        DateTimeOffset? kickoffUtc,
        FixtureStatus status)
    {
        if (homeTeamId == awayTeamId)
        {
            throw new ArgumentException("A fixture requires two different teams.");
        }

        Id = id;
        SeasonId = seasonId;
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        KickoffUtc = kickoffUtc;
        Status = status;
    }

    public Guid Id { get; private set; }

    public Guid SeasonId { get; private set; }

    public Guid HomeTeamId { get; private set; }

    public Guid AwayTeamId { get; private set; }

    public DateTimeOffset? KickoffUtc { get; private set; }

    public FixtureStatus Status { get; private set; }

    public int? RegulationHomeGoals { get; private set; }

    public int? RegulationAwayGoals { get; private set; }

    public void ApplyStatus(FixtureStatus status, DateTimeOffset? kickoffUtc) => (Status, KickoffUtc) = (status, kickoffUtc);

    public void RecordRegulationScore(int? homeGoals, int? awayGoals)
    {
        if (homeGoals is < 0 || awayGoals is < 0 || (homeGoals is null) != (awayGoals is null))
        {
            throw new ArgumentException("Regulation goals must be non-negative and known together.");
        }

        RegulationHomeGoals = homeGoals;
        RegulationAwayGoals = awayGoals;
    }
}
