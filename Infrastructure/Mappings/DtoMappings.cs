using Common.Extensions;
using DataAccess.Entities;
using DataAccess.Entities.Identity;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;

namespace Infrastructure.Mappings;

public class DtoMappings : AutoMapper.Profile
{
    public DtoMappings()
    {
        CreateMap<User, UserDetailUpdateRequest>().ReverseMap();

        CreateMap<User, UserDetailDto>().ReverseMap();

        CreateMap<Profile, ProfileDto>()
            .ForMember(dto => dto.Gender, conf => conf.MapFrom(profile => profile.Gender.Name))
            .ForMember(dto => dto.ImageIds, conf => conf.MapFrom(profile => profile.Images.Select(x => x.Id).ToArray()))
            .ForMember(dto => dto.Age, conf => conf.MapFrom(profile => profile.BirthDate.GetAge()));

        CreateMap<Message, MessageDto>()
            .ForMember(dto => dto.CreatedOn, conf => conf.MapFrom(message => message.CreatedOn.ToString("o")));

        CreateMap<Vote, LikeDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.LikedOn, conf => conf.MapFrom(vote => vote.CreatedOn.ToString("o")));

        CreateMap<Vote, MatchDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.MatchedOn, conf => conf.MapFrom(vote => vote.MatchedOn.ToString("o")));
    }
}