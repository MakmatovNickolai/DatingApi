using Common.Models;

namespace Auth.Dto;

public class CheckGoogleUserResponse : BaseResponse
{
    public bool IdExists { get; set; }
    public bool EmailExists { get; set; }
}