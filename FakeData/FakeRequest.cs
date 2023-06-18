using Auth.Dto;

namespace FakeData;

public static class FakeRequest
{
    public static RegisterEmailRequest RegisterEmail(Guid[] imageIds)
    {
        var request = new RegisterEmailRequest
        {
            Email = Faker.Internet.Email(),
            Password = "123456",
            AppVersion = "v1",
            BirthDate = Faker.Identification.DateOfBirth(),
            Description = Faker.Lorem.Paragraph(),
            DeviceGmtOffset = "0",
            DeviceInfo = "Windows 10",
            DeviceLocale = "en",
            DeviceUniqueId = Faker.RandomNumber.Next().ToString(),
            Gender = "Female",
            IsDeviceEmulator = false,
            IsDeviceRooted = false,
            Name = Faker.Name.First(),
            Platform = "Windows",
            ImageIds = imageIds
        };
        return request;
    }

}