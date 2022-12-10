using AutoMapper;
using SdHub.Automapper.Converters;
using SdHub.Database.Entities.Grids;
using SdHub.Database.Entities.Images;
using SdHub.Models.Grid;
using SdHub.Models.Image;

namespace SdHub.Automapper;

public class GridProfile : Profile
{
    public GridProfile()
    {
        CreateMap<GridEntity, GridModel>(MemberList.Destination)
            .ForMember(x => x.ShortUrl, o => o.ConvertUsing<ShortLinkGridConverter, GridEntity>(x => x));
        CreateMap<GridImageEntity, GridImageModel>(MemberList.Destination);
    }
}