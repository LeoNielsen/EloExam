using Elo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Elo.Api.Infrastructure;

public class EloDbContext : DbContext
{
    public EloDbContext(
        DbContextOptions<EloDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Team>()
            .HasOne(x => x.Attacker)
            .WithMany()
            .HasForeignKey(x => x.AttackerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(x => x.Defender)
            .WithMany()
            .HasForeignKey(x => x.DefenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(x => x.BlueTeam)
            .WithMany()
            .HasForeignKey(x => x.BlueTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(x => x.RedTeam)
            .WithMany()
            .HasForeignKey(x => x.RedTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasIndex(x => new
            {
                x.AttackerId,
                x.DefenderId
            })
            .IsUnique();
    }
}