using Microsoft.EntityFrameworkCore;
using SdHub.Database.Entities;
using SdHub.Database.Entities.Bins;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.User;
using SdHub.Database.Entities.Users;

namespace SdHub.Database;

public class SdHubDbContext : DbContext
{
    //files
    public DbSet<FileStorageEntity> FileStorages { get; set; } = null!;
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<DirectoryEntity> Dirs { get; set; } = null!;

    //images
    public DbSet<ImageEntity> Images { get; set; } = null!;
    public DbSet<ImageRawMetadataEntity> ImageRawMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataEntity> ImageParsedMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataTagEntity> ImageParsedMetadataTags { get; set; } = null!;

    //bins
    public DbSet<ModelEntity> Models { get; set; }
    public DbSet<ModelVersionEntity> ModelVersions { get; set; }
    public DbSet<HypernetEntity> Hypernets { get; set; }
    public DbSet<HypernetVersionEntity> HypernetVersions { get; set; }
    public DbSet<VaeEntity> Vaes { get; set; }
    public DbSet<VaeVersionEntity> VaeVersions { get; set; }
    public DbSet<EmbeddingEntity> Embeddings { get; set; }
    public DbSet<EmbeddingVersionEntity> EmbeddingVersions { get; set; }

    public DbSet<GenerationSampleEntity> GenerationSamples { get; set; }
    public DbSet<ImageUploaderEntity> ImageUploaders { get; set; } = null!;

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;

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