using Microsoft.EntityFrameworkCore;
using SdHub.Database.Entities.Albums;
using SdHub.Database.Entities.Bins;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Grids;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.Tags;
using SdHub.Database.Entities.Users;

namespace SdHub.Database;

public class SdHubDbContext : DbContext
{
    public const string SchemaName = "public";
    public const string HistoryTable = "__EFMigrationsHistory";

    //tags
    public DbSet<TagEntity> Tags { get; set; } = null!;
    public DbSet<ModelTagEntity> ModelTags { get; set; } = null!;
    public DbSet<VaeTagEntity> VaeTags { get; set; } = null!;
    public DbSet<HypernetTagEntity> HypernetTags { get; set; } = null!;
    public DbSet<EmbeddingTagEntity> EmbeddingTags { get; set; } = null!;

    //files
    public DbSet<FileStorageEntity> FileStorages { get; set; } = null!;
    public DbSet<FileEntity> Files { get; set; } = null!;
    public DbSet<DirectoryEntity> Dirs { get; set; } = null!;

    //images
    public DbSet<ImageEntity> Images { get; set; } = null!;
    public DbSet<ImageRawMetadataEntity> ImageRawMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataEntity> ImageParsedMetadata { get; set; } = null!;
    public DbSet<ImageParsedMetadataTagEntity> ImageParsedMetadataTags { get; set; } = null!;

    //grids
    public DbSet<GridEntity> Grids { get; set; } = null!;
    public DbSet<GridImageEntity> GridImages { get; set; } = null!;

    //albums
    public DbSet<AlbumEntity> Albums { get; set; } = null!;
    public DbSet<AlbumImageEntity> AlbumImages { get; set; } = null!;

    //bins
    public DbSet<ModelEntity> Models { get; set; } = null!;
    public DbSet<ModelVersionEntity> ModelVersions { get; set; } = null!;
    public DbSet<ModelVersionFileEntity> ModelVersionFiles { get; set; } = null!;
    public DbSet<HypernetEntity> Hypernets { get; set; } = null!;
    public DbSet<HypernetVersionEntity> HypernetVersions { get; set; } = null!;
    public DbSet<VaeEntity> Vaes { get; set; } = null!;
    public DbSet<VaeVersionEntity> VaeVersions { get; set; } = null!;
    public DbSet<EmbeddingEntity> Embeddings { get; set; } = null!;
    public DbSet<EmbeddingVersionEntity> EmbeddingVersions { get; set; } = null!;

    public DbSet<GenerationSampleEntity> GenerationSamples { get; set; } = null!;
    public DbSet<ImageUploaderEntity> ImageUploaders { get; set; } = null!;
    public DbSet<UserPlanEntity> UserPlans { get; set; } = null!;

    //auth
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserApiTokenEntity> ApiTokens { get; set; } = null!;
    public DbSet<TempCodeEntity> TempCodes { get; set; } = null!;


    public SdHubDbContext(DbContextOptions<SdHubDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}