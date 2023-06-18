using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace UnitTests;

internal class WebApiServerFixture : AbstractTestServerFixture
{
    protected override Task ConfigureApplication(
        WebApplicationBuilder builder)
    {
        //var config = /* your test config */

        //Program.ConfigureApplication(
        //    builder,
        //    config);

        builder.WebHost.UseTestServer();

        return Task.CompletedTask;
    }

    protected override Task ConfigurePipeline(
        WebApplication app)
    {
        // Program.ConfigurePipeline(app);

        return Task.CompletedTask;
    }
}
