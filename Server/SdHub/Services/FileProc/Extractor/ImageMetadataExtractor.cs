using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MetadataExtractor;
using SdHub.Services.FileProc.Metadata;
using Directory = MetadataExtractor.Directory;

namespace SdHub.Services.FileProc.Extractor;

public class ImageMetadataExtractor : IImageMetadataExtractor
{
    public IReadOnlyList<ImageMetadataDirectory> ExtractMetadata(Stream fileStream,
        CancellationToken ct = default)
    {
        var meta = ImageMetadataReader.ReadMetadata(fileStream);

        var dirs = new List<ImageMetadataDirectory>(meta.Count);
        foreach (var dir in meta)
        {
            var rDirTags = dir.Tags.Select(x => MapTag(x, dir)).ToArray();
            var rDir = new ImageMetadataDirectory
            {
                Type = dir.Name,
                Tags = rDirTags
            };
            dirs.Add(rDir);
        }

        return dirs;
    }

    private static ImageMetadataTag MapTag(Tag x, Directory dir)
    {
        var tag = new ImageMetadataTag(x.Type, dir.GetTagName(x.Type), new ImageMetadataTagValue()
        {
            AsString = dir.GetDescription(x.Type),
            Object = dir.GetObject(x.Type)
        });
        if (tag.Value.Object is Array)
        {
            tag.Value.IsArray = true;
            tag.Value.Type = tag.Value.Object.GetType().GetElementType()?.FullName;
        }
        else
        {
            tag.Value.Type = tag.Value.Object?.GetType().FullName;
        }

        return tag;
    }
}