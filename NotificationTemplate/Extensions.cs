using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NotificationTemplate.Services;
using System.Reflection;

namespace NotificationTemplate;

public static class Extensions
{
    public static IServiceCollection AddNotificationTemplateServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHttpContextAccessor();
        var viewAssembly = Assembly.GetExecutingAssembly();
        var viewAssemblyPart = new CompiledRazorAssemblyPart(viewAssembly);

        serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new EmbeddedFileProvider(viewAssembly));
        });
        serviceCollection.AddMvcCore().AddRazorRuntimeCompilation()
                .AddApplicationPart(viewAssembly)
                .AddRazorViewEngine().PartManager.ApplicationParts.Add(viewAssemblyPart);

        serviceCollection
            .AddScoped<ITemplateService, TemplateService>();

        serviceCollection.AddRazorTemplating();

        return serviceCollection;
    }
}
