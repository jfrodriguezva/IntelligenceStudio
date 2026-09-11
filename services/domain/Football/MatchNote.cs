namespace Mis.Domain.Football;

public sealed class MatchNote
{
    private MatchNote() { }

    public MatchNote(Guid id, Guid fixtureId, int minute, string tag, string text, DateTimeOffset capturedAt)
    {
        if (minute is < 0 or > 130) throw new ArgumentOutOfRangeException(nameof(minute));
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Id = id; FixtureId = fixtureId; Minute = minute; Tag = tag.Trim(); Text = text.Trim(); CapturedAt = capturedAt;
    }

    public Guid Id { get; private set; }
    public Guid FixtureId { get; private set; }
    public int Minute { get; private set; }
    public string Tag { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; private set; }
}
