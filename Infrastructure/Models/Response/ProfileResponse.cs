using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class ProfileResponse : BaseResponse
{
    public ProfileDto Profile { get; set; }
}