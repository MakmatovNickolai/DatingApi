
using Common.Models;

namespace Infrastructure.Models.Response;

public class VoteResponse : BaseResponse
{
    public bool IsMutual { get; set; }
}