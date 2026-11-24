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
}