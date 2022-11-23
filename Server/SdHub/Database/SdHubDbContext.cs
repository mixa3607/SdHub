using Microsoft.EntityFrameworkCore;
using SdHub.Database.Entities;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.User;
using SdHub.Database.Entities.Users;

namespace SdHub.Database;

public class SdHubDbContext : DbContext
{
    public DbSet<FileStorageEntity> FileStorages { get; set; } = null!;
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<DirectoryEntity> Dirs { get; set; } = null!;

    public DbSet<ImageEntity> Images { get; set; } = null!;
    public DbSet<ImageRawMetadataEntity> ImageRawMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataEntity> ImageParsedMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataTagEntity> ImageParsedMetadataTags { get; set; }

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<ImageUploaderEntity> ImageUploaders { get; set; } = null!;

    public DbSet<UserPlanEntity> UserPlans { get; set; } = null!;

    public DbSet<TempCodeEntity> TempCodes { get; set; } = null!;

    public SdHubDbContext(DbContextOptions<SdHubDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}