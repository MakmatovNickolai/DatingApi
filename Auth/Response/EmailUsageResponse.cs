using Common.Models;

namespace Auth.Response;

public class EmailUsageResponse : BaseResponse
{
    public bool IsInUse { get; set; }
}
