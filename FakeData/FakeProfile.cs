using DataAccess.Entities;
using Infrastructure.Helpers.Geolocation;

namespace FakeData;

public class FakeProfile
{
    private GeoCoordinate _initialCoordinate;

    public FakeProfile()
    {
        var latitude = Random.NextDouble() * 180 - 90;
        var longitude = Random.NextDouble() * 360 - 90;
        _initialCoordinate = new GeoCoordinate(latitude, longitude);
    }
    
    private  Random Random = new Random();
    public  Profile GetNext(Guid userId, Guid genderId)
    {
        var distance = 10000;
        var coordinate = LocationGenerator.GenerateRandomCoordinate(_initialCoordinate, distance);
        return new Profile
        {
            Id = Guid.NewGuid(),
            Name = Faker.Name.First(),
            Description = Faker.Lorem.Paragraph(),
            BirthDate = DateOnly.FromDateTime(Faker.Identification.DateOfBirth()),
            Latitude = coordinate.Latitude,
            Longitude = coordinate.Longitude,
            GenderId = genderId,
            UserId = userId,
        };
    }
}
