using Microsoft.EntityFrameworkCore;
using ProjectLink.Domain.Entities;

namespace ProjectLink.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Session>       Sessions      => Set<Session>();
    public DbSet<StageProgress> StageProgress => Set<StageProgress>();
    public DbSet<ClientMeta>    ClientMeta    => Set<ClientMeta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>(e =>
        {
            e.ToTable("sessions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36);
            e.Property(x => x.SessionId).HasColumnName("session_id").HasMaxLength(36);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true);
            e.HasIndex(x => x.UserId).HasDatabaseName("idx_sessions_user_id");
            e.HasIndex(x => x.SessionId).IsUnique();
        });

        modelBuilder.Entity<StageProgress>(e =>
        {
            e.ToTable("stage_progress");
            e.HasKey(x => new { x.UserId, x.StageId });
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36);
            e.Property(x => x.StageId).HasColumnName("stage_id");
            e.Property(x => x.Stars).HasColumnName("stars");
            e.Property(x => x.ClearedAt).HasColumnName("cleared_at");
        });

        modelBuilder.Entity<ClientMeta>(e =>
        {
            e.ToTable("client_meta");
            e.HasKey(x => x.ClientVersion);
            e.Property(x => x.ClientVersion).HasColumnName("client_version").HasMaxLength(20);
            e.Property(x => x.MetaHash).HasColumnName("meta_hash");
            e.Property(x => x.ProtocolVersion).HasColumnName("protocol_version").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });
    }
}
