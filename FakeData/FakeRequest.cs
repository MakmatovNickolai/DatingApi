using Auth.Request;
using Infrastructure.Helpers.Geolocation;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;

namespace FakeData;

public static class FakeRequest
{
    public static RegisterEmailRequest RegisterEmail(Guid[] imageIds)
    {
        var images = imageIds.Select((id, index) => new ImageDto
        {
            Id = id,
            OrderIndex = index
        }).ToArray();
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
            Images = images
        };
        return request;
    }

    public static UpdateLocationRequest UpdateLocation(GeoCoordinate coordinate)
    {
        var request = new UpdateLocationRequest
        {
           Longitude = coordinate.Longitude,
           Latitude = coordinate.Latitude
        };
        return request;
    }
}