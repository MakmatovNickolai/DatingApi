
using Common.Models;

namespace Auth.Dto;

public class EmailUsageResponse : BaseResponse
{
    public bool IsInUse { get; set; }
}
