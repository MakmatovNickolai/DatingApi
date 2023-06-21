using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class ExploreResponse : BaseResponse
{
    public List<ProfileDto> Profiles { get; set; }
}