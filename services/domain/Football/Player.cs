namespace Mis.Domain.Football;

public sealed class Player
{
    private Player() { }

    public Player(Guid id, string displayName, DateOnly? birthDate = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        Id = id;
        DisplayName = displayName.Trim();
        BirthDate = birthDate;
    }

    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
}
