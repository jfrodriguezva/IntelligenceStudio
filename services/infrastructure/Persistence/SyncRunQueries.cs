using Microsoft.EntityFrameworkCore;
using Mis.Application.Acquisition;

namespace Mis.Infrastructure.Persistence;

public sealed class SyncRunQueries(MisDbContext database) : ISyncRunQueries
{
    public async Task<IReadOnlyList<SyncRunSummary>> GetRecentAsync(int take, CancellationToken cancellationToken) =>
        await database.SyncRuns.AsNoTracking()
            .OrderByDescending(run => run.RequestedAt)
            .Take(Math.Clamp(take, 1, 50))
            .Select(run => new SyncRunSummary(run.Id, run.Provider, run.Status, run.RequestedAt, run.CompletedAt, run.ImportedFixtureCount, run.ErrorCode))
            .ToListAsync(cancellationToken);
}
