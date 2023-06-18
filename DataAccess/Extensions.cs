using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataModel(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();

        return serviceCollection;
    }
}
