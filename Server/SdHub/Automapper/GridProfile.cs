using AutoMapper;
using SdHub.Database.Entities.Grids;
using SdHub.Models.Grid;

namespace SdHub.Automapper;

public class GridProfile : Profile
{
    public GridProfile()
    {
        CreateMap<GridEntity, GridModel>(MemberList.Destination);
        CreateMap<GridImageEntity, GridImageModel>(MemberList.Destination);
    }
}