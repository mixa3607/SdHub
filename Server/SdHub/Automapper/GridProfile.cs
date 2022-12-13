using AutoMapper;
using SdHub.Automapper.Converters;
using SdHub.Database.Entities.Grids;
using SdHub.Models.Grid;

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