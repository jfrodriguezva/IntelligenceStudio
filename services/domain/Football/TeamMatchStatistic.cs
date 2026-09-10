namespace Mis.Domain.Football;

public sealed class TeamMatchStatistic
{
    private TeamMatchStatistic() { }

    public TeamMatchStatistic(Guid id, Guid fixtureId, Guid teamId, int? possessionPercent, int? shots, int? shotsOnTarget, int? corners)
    {
        if (possessionPercent is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(possessionPercent));
        if (shots is < 0 || shotsOnTarget is < 0 || corners is < 0) throw new ArgumentOutOfRangeException(nameof(shots));
        if (shotsOnTarget is not null && shots is not null && shotsOnTarget > shots) throw new ArgumentException("Shots on target cannot exceed shots.");
        Id = id;
        FixtureId = fixtureId;
        TeamId = teamId;
        PossessionPercent = possessionPercent;
        Shots = shots;
        ShotsOnTarget = shotsOnTarget;
        Corners = corners;
    }

    public Guid Id { get; private set; }
    public Guid FixtureId { get; private set; }
    public Guid TeamId { get; private set; }
    public int? PossessionPercent { get; private set; }
    public int? Shots { get; private set; }
    public int? ShotsOnTarget { get; private set; }
    public int? Corners { get; private set; }
}
