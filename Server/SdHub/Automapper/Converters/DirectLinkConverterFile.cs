using AutoMapper;
using Flurl;
using SdHub.Database.Entities.Files;

namespace SdHub.Automapper.Converters;

public class DirectLinkConverterFile : IValueConverter<FileEntity, string?>
{
    public string? Convert(FileEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(entity.Storage?.BaseUrl))
        {
            return null;
        }

        return entity.Storage.BaseUrl.AppendPathSegment(entity.PathOnStorage);
    }
}