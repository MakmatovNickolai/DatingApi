using Common.Models;

namespace Auth.Response;

public class RegisterResponse : BaseResponse
{
    public Guid UserId { get; set; }
}
