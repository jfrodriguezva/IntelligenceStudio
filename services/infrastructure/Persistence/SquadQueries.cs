using Microsoft.EntityFrameworkCore;
using Mis.Application.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class SquadQueries(MisDbContext database) : ISquadQueries
{
    public async Task<IReadOnlyList<SquadPlayerSummary>> GetForTeamAsync(Guid teamId, CancellationToken cancellationToken) =>
        await (from membership in database.SquadMemberships.AsNoTracking()
               join player in database.Players.AsNoTracking() on membership.PlayerId equals player.Id
               where membership.TeamId == teamId && membership.ValidTo == null
               orderby membership.Position, player.DisplayName
               select new SquadPlayerSummary(player.Id, player.DisplayName, membership.Position.ToString()))
            .ToListAsync(cancellationToken);
}
