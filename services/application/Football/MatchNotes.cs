namespace Mis.Application.Football;

public sealed record MatchNoteSummary(Guid Id, int Minute, string Tag, string Text, DateTimeOffset CapturedAt);
public sealed record CreateMatchNoteRequest(int Minute, string Tag, string Text);

public interface IMatchNoteService
{
    Task<IReadOnlyList<MatchNoteSummary>> GetAsync(Guid fixtureId, CancellationToken cancellationToken);
    Task<MatchNoteSummary> CreateAsync(Guid fixtureId, CreateMatchNoteRequest request, CancellationToken cancellationToken);
}
