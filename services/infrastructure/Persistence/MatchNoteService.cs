using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class MatchNoteService(MisDbContext database) : IMatchNoteService
{
    public async Task<IReadOnlyList<MatchNoteSummary>> GetAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await database.MatchNotes.AsNoTracking().Where(note => note.FixtureId == fixtureId).OrderBy(note => note.Minute).Select(note => new MatchNoteSummary(note.Id, note.Minute, note.Tag, note.Text, note.CapturedAt)).ToListAsync(cancellationToken);

    public async Task<MatchNoteSummary> CreateAsync(Guid fixtureId, CreateMatchNoteRequest request, CancellationToken cancellationToken)
    {
        if (!await database.Fixtures.AnyAsync(fixture => fixture.Id == fixtureId, cancellationToken)) throw new KeyNotFoundException();
        var note = new MatchNote(Guid.NewGuid(), fixtureId, request.Minute, request.Tag, request.Text, DateTimeOffset.UtcNow);
        database.MatchNotes.Add(note); await database.SaveChangesAsync(cancellationToken);
        return new MatchNoteSummary(note.Id, note.Minute, note.Tag, note.Text, note.CapturedAt);
    }
}
