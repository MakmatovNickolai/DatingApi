using DataAccess;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.DataAccess;

public class UnitOfWorkTests
{
    private readonly IUnitOfWork _unitOfWork;
    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Tests")
                .Options;
        var context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(context);
    }
}
