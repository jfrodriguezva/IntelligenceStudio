using Microsoft.EntityFrameworkCore;
using Mis.Domain.Acquisition;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class MisDbContext(DbContextOptions<MisDbContext> options) : DbContext(options)
{
    public DbSet<Competition> Competitions => Set<Competition>();

    public DbSet<Season> Seasons => Set<Season>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Fixture> Fixtures => Set<Fixture>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<SquadMembership> SquadMemberships => Set<SquadMembership>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
    public DbSet<MatchNote> MatchNotes => Set<MatchNote>();
    public DbSet<TeamMatchStatistic> TeamMatchStatistics => Set<TeamMatchStatistic>();

    public DbSet<ProviderEntityMapping> ProviderEntityMappings => Set<ProviderEntityMapping>();

    public DbSet<SyncRun> SyncRuns => Set<SyncRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("football");

        modelBuilder.Entity<Competition>(builder =>
        {
            builder.ToTable("competitions");
            builder.HasKey(competition => competition.Id);
            builder.Property(competition => competition.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Season>(builder =>
        {
            builder.ToTable("seasons");
            builder.HasKey(season => season.Id);
            builder.Property(season => season.Label).HasMaxLength(50).IsRequired();
            builder.HasIndex(season => new { season.CompetitionId, season.Label }).IsUnique();
            builder.HasOne<Competition>().WithMany().HasForeignKey(season => season.CompetitionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Team>(builder =>
        {
            builder.ToTable("teams");
            builder.HasKey(team => team.Id);
            builder.Property(team => team.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Fixture>(builder =>
        {
            builder.ToTable("fixtures", table => table.HasCheckConstraint("CK_fixtures_distinct_teams", "[HomeTeamId] <> [AwayTeamId]"));
            builder.HasKey(fixture => fixture.Id);
            builder.Property(fixture => fixture.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.HasIndex(fixture => new { fixture.KickoffUtc, fixture.Id });
            builder.HasIndex(fixture => new { fixture.SeasonId, fixture.KickoffUtc, fixture.Id });
            builder.HasOne<Season>().WithMany().HasForeignKey(fixture => fixture.SeasonId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(fixture => fixture.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(fixture => fixture.AwayTeamId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Player>(builder =>
        {
            builder.ToTable("players");
            builder.HasKey(player => player.Id);
            builder.Property(player => player.DisplayName).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<SquadMembership>(builder =>
        {
            builder.ToTable("squad_memberships");
            builder.HasKey(membership => membership.Id);
            builder.Property(membership => membership.Position).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.HasIndex(membership => new { membership.PlayerId, membership.TeamId, membership.ValidFrom }).IsUnique();
            builder.HasOne<Player>().WithMany().HasForeignKey(membership => membership.PlayerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(membership => membership.TeamId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MatchEvent>(builder =>
        {
            builder.ToTable("match_events");
            builder.HasKey(matchEvent => matchEvent.Id);
            builder.Property(matchEvent => matchEvent.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(matchEvent => matchEvent.Note).HasMaxLength(1000);
            builder.HasIndex(matchEvent => new { matchEvent.FixtureId, matchEvent.Minute, matchEvent.Id });
            builder.HasOne<Fixture>().WithMany().HasForeignKey(matchEvent => matchEvent.FixtureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Player>().WithMany().HasForeignKey(matchEvent => matchEvent.PlayerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MatchNote>(builder =>
        {
            builder.ToTable("match_notes");
            builder.HasKey(note => note.Id);
            builder.Property(note => note.Tag).HasMaxLength(40).IsRequired();
            builder.Property(note => note.Text).HasMaxLength(2000).IsRequired();
            builder.HasIndex(note => new { note.FixtureId, note.Minute, note.Id });
            builder.HasOne<Fixture>().WithMany().HasForeignKey(note => note.FixtureId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TeamMatchStatistic>(builder =>
        {
            builder.ToTable("team_match_statistics");
            builder.HasKey(statistic => statistic.Id);
            builder.HasIndex(statistic => new { statistic.FixtureId, statistic.TeamId }).IsUnique();
            builder.HasOne<Fixture>().WithMany().HasForeignKey(statistic => statistic.FixtureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Team>().WithMany().HasForeignKey(statistic => statistic.TeamId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProviderEntityMapping>(builder =>
        {
            builder.ToTable("provider_entity_mappings", "acquisition");
            builder.HasKey(mapping => mapping.Id);
            builder.Property(mapping => mapping.Provider).HasMaxLength(50).IsRequired();
            builder.Property(mapping => mapping.ResourceType).HasMaxLength(50).IsRequired();
            builder.Property(mapping => mapping.ExternalId).HasMaxLength(100).IsRequired();
            builder.HasIndex(mapping => new { mapping.Provider, mapping.ResourceType, mapping.ExternalId }).IsUnique();
        });

        modelBuilder.Entity<SyncRun>(builder =>
        {
            builder.ToTable("sync_runs", "acquisition");
            builder.HasKey(run => run.Id);
            builder.Property(run => run.Provider).HasMaxLength(50).IsRequired();
            builder.Property(run => run.Status).HasMaxLength(20).IsRequired();
            builder.Property(run => run.ErrorCode).HasMaxLength(100);
            builder.HasIndex(run => run.RequestedAt);
        });
    }
}
