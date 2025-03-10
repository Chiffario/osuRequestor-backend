using Microsoft.EntityFrameworkCore;

namespace osuRequestor.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Models.RequestModel> Requests { get; set; } = null!;
    public DbSet<Models.BeatmapModel> Beatmaps { get; set; } = null!;
    public DbSet<Models.BeatmapSetModel> BeatmapSets { get; set; } = null!;
    public DbSet<Models.UserModel> Users { get; set; } = null!;

    public DbSet<Models.TokenModel> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.RequestModel>()
            .Property(request => request.Date)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Models.BeatmapModel>()
            .Property(item => item.Id)
            .IsRequired();

        modelBuilder.Entity<Models.UserModel>();

    }
}