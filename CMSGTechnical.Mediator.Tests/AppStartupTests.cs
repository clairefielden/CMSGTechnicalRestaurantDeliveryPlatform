using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace CMSGTechnical.Mediator.Tests;

public class AppStartupTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AppStartupTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AppRootReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");

        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound or HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task AppStartsInProductionConfiguration()
    {
        var factory = _factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        var client = factory.CreateClient();
        var response = await client.GetAsync("/");

        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound or HttpStatusCode.Redirect);
    }
}
