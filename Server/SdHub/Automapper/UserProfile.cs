using AutoMapper;
using SdHub.Database.Entities.Users;
using SdHub.Models;

namespace SdHub.Automapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserModel>(MemberList.Destination);
    }
}