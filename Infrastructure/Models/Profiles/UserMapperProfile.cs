using AutoMapper;
using DataAccess.Entities.Identity;
using Infrastructure.Models.Users;

namespace Infrastructure.Models.Profiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<User, UserDetailUpdateRequest>().ReverseMap();
        CreateMap<User, UserDetailDto>().ReverseMap();
    }
}
