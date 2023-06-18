namespace Common.Models;

public class SuccessfulResponse : BaseResponse
{
    public SuccessfulResponse(string message)
    {
        Success = true;
        Message = message;
    }
}