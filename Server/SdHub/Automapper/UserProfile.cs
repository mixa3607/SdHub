using AutoMapper;
using SdHub.Database.Entities.Users;
using SdHub.Models;
using SdHub.Models.User;

namespace SdHub.Automapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserModel>(MemberList.Destination);
        CreateMap<UserEntity, UserSimpleModel>(MemberList.Destination);
        CreateMap<UserEntity, UserAdminModel>(MemberList.Destination);

        CreateMap<UserPlanEntity, UserPlanModel>(MemberList.Destination);
        CreateMap<UserApiTokenEntity, UserApiTokenModel>(MemberList.Destination);
    }
}