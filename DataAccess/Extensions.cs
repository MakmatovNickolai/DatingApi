using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataModel(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        serviceCollection.TryAddScoped(typeof(ICompositeKeyRepository<>), typeof(CompositeKeyRepository<>));
        serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();

        return serviceCollection;
    }
}
