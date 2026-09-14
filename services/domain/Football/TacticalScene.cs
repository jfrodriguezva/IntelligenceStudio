using System.Text.Json;

namespace Mis.Domain.Football;

public sealed class TacticalScene
{
    private TacticalScene() { }

    public TacticalScene(Guid id, Guid fixtureId, string title, string stateJson, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        if (title.Trim().Length > 160) throw new ArgumentException("Title cannot exceed 160 characters.", nameof(title));
        ValidateState(stateJson);

        Id = id;
        FixtureId = fixtureId;
        Title = title.Trim();
        StateJson = stateJson;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid FixtureId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string StateJson { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private static void ValidateState(string stateJson)
    {
        if (string.IsNullOrWhiteSpace(stateJson) || stateJson.Length > 100_000) throw new ArgumentException("A tactical scene state is required.", nameof(stateJson));
        try
        {
            using var document = JsonDocument.Parse(stateJson);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("schemaVersion", out var version) || version.ValueKind != JsonValueKind.Number || version.GetInt32() != 1)
                throw new ArgumentException("Tactical scene state must use schema version 1.", nameof(stateJson));
            if (!root.TryGetProperty("players", out var players) || players.ValueKind != JsonValueKind.Array || players.GetArrayLength() > 11)
                throw new ArgumentException("Tactical scene state must contain at most eleven players.", nameof(stateJson));

            var playerIds = new HashSet<Guid>();
            foreach (var player in players.EnumerateArray())
            {
                if (player.ValueKind != JsonValueKind.Object || !player.TryGetProperty("playerId", out var playerId) || !Guid.TryParse(playerId.GetString(), out var parsedId) || !playerIds.Add(parsedId))
                    throw new ArgumentException("Every scene player must have a unique identifier.", nameof(stateJson));
                if (!player.TryGetProperty("role", out var role) || role.ValueKind != JsonValueKind.String || role.GetString() is not ("Goalkeeper" or "Defender" or "Midfielder" or "Forward"))
                    throw new ArgumentException("Every scene player must have a supported role.", nameof(stateJson));
                ValidateCoordinate(player, "x");
                ValidateCoordinate(player, "y");
            }
        }
        catch (JsonException exception) { throw new ArgumentException("Tactical scene state must contain valid JSON.", nameof(stateJson), exception); }
    }

    private static void ValidateCoordinate(JsonElement player, string coordinate)
    {
        if (!player.TryGetProperty(coordinate, out var value) || value.ValueKind != JsonValueKind.Number || !value.TryGetDouble(out var number) || number is < 0 or > 1)
            throw new ArgumentException("Tactical coordinates must be normalized between 0 and 1.", nameof(player));
    }
}
