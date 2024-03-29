﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Albums;

namespace SdHub.Database.Configurators.Albums;

public class AlbumConfigurator : IEntityTypeConfiguration<AlbumEntity>
{
    public void Configure(EntityTypeBuilder<AlbumEntity> builder)
    {
        builder.HasIndex(x => x.ShortToken).IsUnique();
        builder.Property(x => x.ShortToken).IsRequired();
    }
}