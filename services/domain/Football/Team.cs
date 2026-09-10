namespace Mis.Domain.Football;

public sealed class Team
{
    private Team()
    {
    }

    public Team(Guid id, string name)
    {
        Id = id;
        Name = RequireName(name);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public void Rename(string name) => Name = RequireName(name);

    private static string RequireName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
