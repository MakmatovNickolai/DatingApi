using Common.Models;

namespace Auth.Dto;

public class RegisterResponse : BaseResponse
{
    public Guid UserId { get; set; }
}
