using AutoMapper;
using Flurl;
using SdHub.Database.Entities.Files;

namespace SdHub.Automapper.Converters;

public class DirectLinkConverterDir : IValueConverter<DirectoryEntity, string?>
{
    public string? Convert(DirectoryEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(entity.Storage?.BaseUrl))
        {
            return null;
        }

        return entity.Storage.BaseUrl.AppendPathSegment(entity.PathOnStorage);
    }
}