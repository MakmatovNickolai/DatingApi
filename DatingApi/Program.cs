using Auth;
using Common;
using Common.Models;
using DataAccess;
using DatingApi.Hubs;
using DatingApi.Middlewares;
using DatingApi.SignalR;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationTemplate;
using Serilog;
using System.Net.Mime;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;
var services = builder.Services;

services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger()
    );
});


services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

services.AddCommonServices()
    .AddAuth(configuration)
    .AddDataModel()
    .AddNotificationTemplateServices(configuration)
    .AddApplicationServices(configuration)
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var result = new ValidationFailedResult(context.ModelState);

            result.ContentTypes.Add(MediaTypeNames.Application.Json);
            result.ContentTypes.Add(MediaTypeNames.Application.Xml);
            return result;
        };
    })
    .AddNewtonsoftJson();

services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    })
    .AddCors(config =>
    {
        config.AddPolicy("Default", builder =>
        {
            builder
                .WithOrigins("http://localhost:5000", "https://localhost:5000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
});

services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

services.AddHangfire(configuration =>
    {
        configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(1)
                });
    })
    .AddHangfireServer();

services.AddSignalR();
services.AddSingleton<IUserIdProvider, UserProfileIdProvider>();
services.AddScoped<HubContextService>();
services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults
        .MimeTypes.Concat(new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("Default");

if (builder.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ConversationHub>("/conversationHub");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
    context!.Database.Migrate();
}

app.Run();

public partial class Program { }