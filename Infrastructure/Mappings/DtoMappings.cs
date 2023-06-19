using Common.Extensions;
using DataAccess.Entities;
using Infrastructure.Models;
using Infrastructure.Models.Users;

namespace Infrastructure.Mappings;

public class DtoMappings : AutoMapper.Profile
{
    public DtoMappings()
    {
        CreateMap<Profile, ProfileDto>()
            .ForMember(dto => dto.Gender, conf => conf.MapFrom(profile => profile.Gender.Name))
            .ForMember(dto => dto.ImageIds, conf => conf.MapFrom(profile => profile.Images.Select(x => x.Id).ToArray()))
            .ForMember(dto => dto.Age, conf => conf.MapFrom(profile => profile.BirthDate.GetAge()));

        CreateMap<Message, MessageDto>();

        CreateMap<Vote, LikeDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.LikeDate, conf => conf.MapFrom(vote => vote.CreatedOn.ToShortDateString()));

        CreateMap<Vote, MatchDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.MatchedOn, conf => conf.MapFrom(vote => vote.MatchedOn.ToShortDateString()));
    }
}
