using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UnitTests;

internal abstract class AbstractTestServerFixture : IAsyncLifetime
{
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly List<HttpClient> clients;
    private WebApplication? app;
    private TestServer? server;

    protected AbstractTestServerFixture()
    {
        cancellationTokenSource = new();
        clients = new();
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                EnvironmentName = Environments.Development
            });
        builder.WebHost
            .UseShutdownTimeout(TimeSpan.FromSeconds(5))
            .UseTestServer();

        await ConfigureApplication(builder);

        app = builder.Build();

        await ConfigurePipeline(app);

        server = (TestServer)app.Services.GetRequiredService<IServer>();

        await app.StartAsync(cancellationTokenSource.Token);
    }

    protected abstract Task ConfigureApplication(WebApplicationBuilder builder);

    protected abstract Task ConfigurePipeline(WebApplication app);

    async Task IAsyncLifetime.DisposeAsync()
    {
        foreach (var client in clients)
        {
            client.Dispose();
        }

        cancellationTokenSource.Cancel();

        app?.StopAsync(TimeSpan.FromSeconds(30));
        await (app?.DisposeAsync() ?? ValueTask.CompletedTask);
        server?.Dispose();
    }

    public HttpClient CreateClient()
    {
        return CreateClientInternal(
            new RedirectHandler(7),
            new CookieContainerHandler());
    }


    // MIT License
    // loosely based on the source code of https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing
    // found at https://github.com/dotnet/aspnetcore/tree/main/src/Mvc/Mvc.Testing/src
    // (version at the time of writing: https://github.com/dotnet/aspnetcore/tree/cbb4916ecc63785267a750a78b42f3a769230509/src/Mvc/Mvc.Testing/src)
    private HttpClient CreateClientInternal(
        params DelegatingHandler[] handlers)
    {
        HttpClient client;
        if (handlers.Length == 0)
        {
            client = server!.CreateClient();
        }
        else
        {
            for (var i = handlers.Length - 1; i > 0; i--)
            {
                handlers[i - 1].InnerHandler = handlers[i];
            }

            var serverHandler = server!.CreateHandler();
            handlers[^1].InnerHandler = serverHandler;

            client = new HttpClient(handlers[0]);
        }

        clients.Add(client);

        client.BaseAddress = server.BaseAddress;

        return client;
    }
}
