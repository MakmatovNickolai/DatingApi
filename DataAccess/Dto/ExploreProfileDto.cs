using DataAccess.Entities;

namespace DataAccess.Dto;

public class ExploreProfileDto : Profile
{
    public int DistanceInMeters { get; set; }
}