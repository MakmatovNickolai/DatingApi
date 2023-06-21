using Common.Models;

namespace Auth.Response;

public class CheckGoogleUserResponse : BaseResponse
{
    public bool IdExists { get; set; }
    public bool EmailExists { get; set; }
}