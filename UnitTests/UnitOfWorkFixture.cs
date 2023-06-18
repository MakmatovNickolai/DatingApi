using DataAccess;
using DataAccess.Entities;
using DataAccess.Entities.Identity;
using DataAccess.Repositories;
using FakeData;
using Microsoft.EntityFrameworkCore;

namespace UnitTests;

public class UnitOfWorkFixture : IDisposable
{
    public UnitOfWork UnitOfWork { get; set; }
    private const int ProfilesCount = 40;
    private const int ImagesCount = 2;
    public UnitOfWorkFixture(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new ApplicationDbContext(options);
        PopulateAsync(context).Wait();
        UnitOfWork = new UnitOfWork(context);
    }
    public void Dispose()
    {
        UnitOfWork.Dispose();
    }

    private async Task PopulateAsync(ApplicationDbContext context)
    {
        var female = await context.Set<Gender>().AddAsync(new Gender
        {
            Id = Guid.NewGuid(),
            Name = "Female"
        });
        var male = await context.Set<Gender>().AddAsync(new Gender
        {
            Id = Guid.NewGuid(),
            Name = "Male"
        });
        var fakeImage = new FakeImage();
        for (var i = 0; i < ProfilesCount; i++)
        {
            var user = FakeUser.GetNext();
            await context.Set<User>().AddAsync(user);
            var genderId = i % 2 == 0 ? female.Entity.Id : male.Entity.Id;
            var profile = FakeProfile.GetNext(user.Id, genderId);
            await context.Set<Profile>().AddAsync(profile);
            for (var j = 0; j < ImagesCount; j++)
            {
                var image = fakeImage.GetNext(profile.Id);
                await context.Set<Image>().AddAsync(image);
            }
        }

        await context.SaveChangesAsync();
    }
}
