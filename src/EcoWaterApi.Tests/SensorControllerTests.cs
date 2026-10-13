using Microsoft.AspNetCore.Mvc.Testing;

namespace EcoWaterApi.Tests;

public class SensorControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SensorControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var response = await _client.GetAsync("/api/sensors");

        response.EnsureSuccessStatusCode();
    }
}