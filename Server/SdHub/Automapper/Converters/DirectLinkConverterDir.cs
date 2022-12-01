using System;
using AutoMapper;
using Flurl;
using SdHub.Database.Entities.Files;

namespace SdHub.Automapper.Converters;

public class DirectLinkConverterDir : IValueConverter<DirectoryEntity, string>
{
    public string Convert(DirectoryEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(entity.Storage?.BaseUrl))
        {
            throw new Exception("BaseUrl is null. Can't convert to direct link");
        }

        return entity.Storage.BaseUrl.AppendPathSegment(entity.PathOnStorage);
    }
}