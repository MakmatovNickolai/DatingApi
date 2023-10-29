using DataAccess.Entities;
using DataAccess.Entities.Identity;
using Infrastructure.Helpers.Geolocation;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;

namespace Infrastructure.Mappings;

public class DtoMappings : AutoMapper.Profile
{
    public DtoMappings()
    {
        CreateMap<User, UserDetailUpdateRequest>().ReverseMap();

        CreateMap<Image, ImageDto>();

        GeoCoordinate? currentProfileCoordinate = null;
        CreateMap<Profile, ProfileDto>()            
            .ForMember(dto => dto.Gender, conf => conf.MapFrom(profile => profile.Gender.Name))
            .ForMember(dto => dto.Images, conf => conf.MapFrom(profile => profile.Images))
            .ForMember(dto => dto.DateOfBirth, conf => conf.MapFrom(profile => profile.BirthDate.ToString("o")))              
            .ForMember(dto => dto.RecentOn, conf => conf.MapFrom(profile => profile.RecentOn.ToString("o")))
            .ForMember(dto => dto.DistanceKm, conf => conf.MapFrom(profile => currentProfileCoordinate == null ? 0 :
                Convert.ToInt32(
                    DistanceCalculator.CalculateDistance(
                        new GeoCoordinate(profile.Latitude, profile.Longitude),
                        currentProfileCoordinate
                    )
                )
            ));

        CreateMap<Message, MessageDto>()
            .ForMember(dto => dto.CreatedOn, conf => conf.MapFrom(message => message.CreatedOn.ToString("o")));

        CreateMap<Vote, LikeDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.LikedOn, conf => conf.MapFrom(vote => vote.CreatedOn.ToString("o")));

        CreateMap<Vote, MatchDto>()
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(vote => vote.Voter))
            .ForMember(dto => dto.MatchedOn, conf => conf.MapFrom(vote => vote.MatchedOn.ToString("o")));

        CreateMap<User, UserDto>()
            .ForMember(dto => dto.CreatedOn, conf => conf.MapFrom(message => message.CreatedOn.ToString("o")))
            .ForMember(dto => dto.Profile, conf => conf.MapFrom(user => user.Profile));
    }
}