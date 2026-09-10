namespace Mis.Domain.Football;

public enum MatchEventType { Goal, Assist, Card, Substitution, Observation }

public sealed class MatchEvent
{
    private MatchEvent() { }

    public MatchEvent(Guid id, Guid fixtureId, Guid? playerId, MatchEventType type, int minute, string? note = null)
    {
        if (minute is < 0 or > 130) throw new ArgumentOutOfRangeException(nameof(minute));
        Id = id;
        FixtureId = fixtureId;
        PlayerId = playerId;
        Type = type;
        Minute = minute;
        Note = note?.Trim();
    }

    public Guid Id { get; private set; }
    public Guid FixtureId { get; private set; }
    public Guid? PlayerId { get; private set; }
    public MatchEventType Type { get; private set; }
    public int Minute { get; private set; }
    public string? Note { get; private set; }
}
