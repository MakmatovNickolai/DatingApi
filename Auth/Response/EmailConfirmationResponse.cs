using Common.Models;

namespace Auth.Response;

public class EmailConfirmationResponse : BaseResponse
{
    public bool IsConfirmed { get; set; }
}