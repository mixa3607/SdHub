using AutoMapper;
using SdHub.Automapper.Converters;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.User;
using SdHub.Database.Entities.Users;
using SdHub.Models;
using SdHub.Storage;

namespace SdHub.Automapper;

public class FileProfile : Profile
{
    public FileProfile()
    {
        CreateMap<FileSaveResult, FileEntity>(MemberList.Source);
        CreateMap<FileEntity, FileModel>(MemberList.Destination)
            .ForMember(x => x.DirectUrl, o => o.ConvertUsing<DirectLinkConverter, FileEntity>(x => x));
        CreateMap<ImageEntity, ImageModel>(MemberList.Destination)
            .ForMember(x => x.ShortUrl, o => o.ConvertUsing<ShortLinkConverter, ImageEntity>(x => x));
        CreateMap<ImageParsedMetadataEntity, ImageParsedMetadataModel>(MemberList.Destination).ReverseMap();
        CreateMap<ImageParsedMetadataTagEntity, ImageParsedMetadataTagModel>(MemberList.Destination).ReverseMap();
        CreateMap<UserEntity, ImageOwnerModel>(MemberList.Destination);

        CreateMap<UserPlanEntity, UserPlanModel>(MemberList.Destination);
    }
}