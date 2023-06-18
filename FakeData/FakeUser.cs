using DataAccess.Entities.Identity;

namespace FakeData;

public static class FakeUser
{
    public static User GetNext()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            AppVersion = "test",
            DeviceGmtOffset = "test",
            DeviceInfo = "test",
            DeviceLocale = "test",
            DeviceUniqueId = "test",
            Platform = "android",
        };
    }
}