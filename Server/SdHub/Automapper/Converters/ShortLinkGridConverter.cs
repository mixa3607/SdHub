using System;
using AutoMapper;
using Flurl;
using Microsoft.Extensions.Options;
using SdHub.Database.Entities.Grids;
using SdHub.Options;

namespace SdHub.Automapper.Converters;

public class ShortLinkGridConverter : IValueConverter<GridEntity, string>
{
    private readonly AppInfoOptions _appInfo;

    public ShortLinkGridConverter(IOptions<AppInfoOptions> appInfo)
    {
        _appInfo = appInfo.Value;
    }

    public string Convert(GridEntity entity, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(_appInfo.BaseUrl))
        {
            throw new Exception("BaseUrl in settings not set");
        }

        return _appInfo.BaseUrl.AppendPathSegment("g").AppendPathSegment(entity.ShortToken);
    }
}