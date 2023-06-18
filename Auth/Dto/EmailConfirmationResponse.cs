using Common.Models;

namespace Auth.Dto;

public class EmailConfirmationResponse : BaseResponse
{
    public bool IsConfirmed { get; set; }
}