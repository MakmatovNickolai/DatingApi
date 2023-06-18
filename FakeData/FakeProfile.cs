using DataAccess.Entities;

namespace FakeData;

public static class FakeProfile
{
    private static Random Random = new Random();
    public static Profile GetNext(Guid userId, Guid genderId)
    {
        return new Profile
        {
            Id = Guid.NewGuid(),
            Name = Faker.Name.First(),
            Description = Faker.Lorem.Paragraph(),
            BirthDate = DateOnly.FromDateTime(Faker.Identification.DateOfBirth()),
            Latitude = Random.NextDouble() * 180 - 90,
            Longitude = Random.NextDouble() * 360 - 180,
            GenderId = genderId,
            UserId = userId,
        };
    }
}
