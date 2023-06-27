using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class UserResponse : BaseResponse
{
    public UserDto User { get; set; }
}