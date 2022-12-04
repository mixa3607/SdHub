using AutoMapper;
using SdHub.Database.Entities.Albums;
using SdHub.Models.Album;

namespace SdHub.Automapper;

public class AlbumProfile : Profile
{
    public AlbumProfile()
    {
        CreateMap<AlbumEntity, AlbumModel>(MemberList.Destination);
        CreateMap<AlbumImageEntity, AlbumImageModel>(MemberList.Destination);
    }
}