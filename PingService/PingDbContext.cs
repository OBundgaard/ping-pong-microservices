using Microsoft.EntityFrameworkCore;
using Core.Models.PingService;
using Core.Helpers;

namespace PingService;

public class PingDbContext : DbContext
{
    public DbSet<Credentials> Credentials { get; set; }
    public DbSet<Permissions> Permissions { get; set; }

    public PingDbContext(DbContextOptions<PingDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credentials>().HasKey(u => u.ID);
        modelBuilder.Entity<Credentials>().Property(u => u.ID).ValueGeneratedOnAdd();

        modelBuilder.Entity<Permissions>().HasKey(f => f.ID);
        modelBuilder.Entity<Permissions>().Property(f => f.ID).ValueGeneratedNever();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            MonitorService.Log.Fatal($"[DB Context @ Ping Service] : DATABASE SAVE operation FAILED, {ex}");
            throw new Exception($"[DB Context @ Ping Service] : DATABASE SAVE operation FAILED, {ex}");
        }
    }
}
