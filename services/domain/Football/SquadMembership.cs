namespace Mis.Domain.Football;

public enum FootballPosition { Goalkeeper, Defender, Midfielder, Forward }

public sealed class SquadMembership
{
    private SquadMembership() { }

    public SquadMembership(Guid id, Guid playerId, Guid teamId, FootballPosition position, DateOnly validFrom, DateOnly? validTo = null)
    {
        if (validTo is not null && validTo < validFrom) throw new ArgumentException("Membership cannot end before it starts.");
        Id = id;
        PlayerId = playerId;
        TeamId = teamId;
        Position = position;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid TeamId { get; private set; }
    public FootballPosition Position { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
}
