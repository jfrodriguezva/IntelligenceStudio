namespace Mis.Domain.Football;

public sealed class Season
{
    private Season()
    {
    }

    public Season(Guid id, Guid competitionId, string label)
    {
        Id = id;
        CompetitionId = competitionId;
        Label = RequireLabel(label);
    }

    public Guid Id { get; private set; }

    public Guid CompetitionId { get; private set; }

    public string Label { get; private set; } = string.Empty;

    private static string RequireLabel(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
