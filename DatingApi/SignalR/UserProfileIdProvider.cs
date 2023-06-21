using Microsoft.AspNetCore.SignalR;

namespace DatingApi.SignalR;

public class UserProfileIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.GetProfileId().ToString();
    }
}
