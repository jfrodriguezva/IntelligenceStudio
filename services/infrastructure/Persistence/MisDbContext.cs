using Microsoft.EntityFrameworkCore;
using Mis.Domain.Football;

namespace Mis.Infrastructure.Persistence;

public sealed class MisDbContext(DbContextOptions<MisDbContext> options) : DbContext(options)
{
    public DbSet<Competition> Competitions => Set<Competition>();

    public DbSet<Season> Seasons => Set<Season>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Fixture> Fixtures => Set<Fixture>();

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
    }
}
