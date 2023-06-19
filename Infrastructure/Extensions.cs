using Infrastructure.Config;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationTemplate.Services;

namespace Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<NotificationOptions>(configuration.GetSection("Notification"));
        serviceCollection.Configure<EnvOptions>(configuration.GetSection("Env"));
        serviceCollection.Configure<ImageOptions>(configuration.GetSection("Image"));

        serviceCollection.AddScoped<IImageService, ImageService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IProfilesService, ProfilesService>();
        serviceCollection.AddScoped<IConversationService, ConversationService>();

        serviceCollection.AddScoped<INotificationService>(sc =>
        {
            var config = sc.GetService<IOptions<EnvOptions>>();
            var notification = sc.GetService<IOptions<NotificationOptions>>()!;
            var templateService = sc.GetService<ITemplateService>()!;
            var logger = sc.GetService<ILogger<StagingNotificationService>>()!;
            if (config != null && config.Value.Name == "staging")
            {
                return new StagingNotificationService(templateService, notification, logger);
            }

            return new ProductionNotificationService(logger);
        });

        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return serviceCollection;
    }
}