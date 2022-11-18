using System;
using AutoMapper;
using Flurl;
using SdHub.Database.Entities.Files;

namespace SdHub.Automapper.Converters;

public class DirectLinkConverter : IValueConverter<FileEntity, string>
{
    public string Convert(FileEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(entity.Storage?.BaseUrl))
        {
            throw new Exception("BaseUrl is null. Can't convert to direct link");
        }

        return entity.Storage.BaseUrl.AppendPathSegment(entity.PathOnStorage);
    }
}