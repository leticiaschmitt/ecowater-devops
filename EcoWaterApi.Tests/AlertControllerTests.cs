using Microsoft.AspNetCore.Mvc.Testing;

namespace EcoWaterApi.Tests;

public class AlertControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AlertControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var response = await _client.GetAsync("/api/alerts");

        response.EnsureSuccessStatusCode();
    }
}