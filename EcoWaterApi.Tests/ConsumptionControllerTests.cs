using Microsoft.AspNetCore.Mvc.Testing;

namespace EcoWaterApi.Tests;

public class ConsumptionControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConsumptionControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var response = await _client.GetAsync("/api/consumptions");

        response.EnsureSuccessStatusCode();
    }
}