using System;
using AutoMapper;
using Flurl;
using Microsoft.Extensions.Options;
using SdHub.Database.Entities.Images;
using SdHub.Options;

namespace SdHub.Automapper.Converters;

public class ShortLinkImageConverter : IValueConverter<ImageEntity, string>
{
    private readonly AppInfoOptions _appInfo;

    public ShortLinkImageConverter(IOptions<AppInfoOptions> appInfo)
    {
        _appInfo = appInfo.Value;
    }

    public string Convert(ImageEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(_appInfo.BaseUrl))
        {
            throw new Exception("BaseUrl in settings not set");
        }

        return _appInfo.BaseUrl.AppendPathSegment("i").AppendPathSegment(entity.ShortToken);
    }
}