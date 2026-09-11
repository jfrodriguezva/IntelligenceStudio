using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class MatchEventQueries(MisDbContext database) : IMatchEventQueries
{
    public async Task<IReadOnlyList<MatchEventSummary>> GetForFixtureAsync(Guid fixtureId, CancellationToken cancellationToken) =>
        await (from matchEvent in database.MatchEvents.AsNoTracking()
               join player in database.Players.AsNoTracking() on matchEvent.PlayerId equals player.Id into players
               from player in players.DefaultIfEmpty()
               where matchEvent.FixtureId == fixtureId
               orderby matchEvent.Minute, matchEvent.Id
               select new MatchEventSummary(matchEvent.Type.ToString(), matchEvent.Minute, player == null ? null : player.DisplayName, matchEvent.Note))
            .ToListAsync(cancellationToken);
}
