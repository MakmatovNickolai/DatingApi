using DataAccess;
using DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.DataAccess;

public class ApplicationDbContextTests
{
    private readonly ApplicationDbContext _context;
    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "Tests")
           .Options;
        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_SetsCreatedOnAndModifiedOnProperties()
    {
        // Arrange
        var entity = new User
        {
            Id = Guid.NewGuid(),
            Email = "test",
            AppVersion = "test",
            DeviceGmtOffset = "test",
            DeviceInfo = "test",
            DeviceLocale = "test",
            DeviceUniqueId = "test",
            Platform = "test",
        };

        // Act 
        await _context.Set<User>().AddAsync(entity);
        await _context.SaveChangesAsync();
        var user = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == entity.Id);
        user!.AppVersion = "test2";
        await _context.SaveChangesAsync();

        // Assert
        var modifiedUser = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == entity.Id);
        Assert.NotEqual(DateTime.MinValue, modifiedUser!.CreatedOn);
        Assert.NotEqual(DateTime.MinValue, modifiedUser!.ModifiedOn);
    }
}
