using Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IPasswordHasher, PasswordHasher>();
        serviceCollection.AddScoped<ICodeGenerator, CodeGenerator>();
        return serviceCollection;
    }
}