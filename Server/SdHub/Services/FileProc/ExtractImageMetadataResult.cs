using System.Collections.Generic;
using SdHub.Models;
using SdHub.Services.FileProc.Metadata;

namespace SdHub.Services.FileProc;

public class ExtractImageMetadataResult
{
    public ExtractImageMetadataResult(IReadOnlyList<ImageParsedMetadataTagModel> parsedTags, IReadOnlyList<ImageMetadataDirectory> rawDirectories, int height, int width)
    {
        ParsedTags = parsedTags;
        RawDirectories = rawDirectories;
        Height = height;
        Width = width;
    }

    public IReadOnlyList<ImageParsedMetadataTagModel> ParsedTags { get; init; }
    public IReadOnlyList<ImageMetadataDirectory> RawDirectories { get; init; }

    public int Height { get; set; }
    public int Width { get; set; }
}