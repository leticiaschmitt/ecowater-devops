using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NJsonSchema;
using Reqnroll;
using Xunit;

namespace EcoWater.BddTests.Steps;

[Binding]
public sealed class ApiSteps
{
    private readonly HttpClient _client = new() { BaseAddress = new Uri(Environment.GetEnvironmentVariable("ECOWATER_BASE_URL") ?? "http://localhost:8080") };
    private HttpResponseMessage? _response;
    private string _body = "";
    private string? _token;
    private int _sensorId;
    private int _consumptionId;
    private int _alertId;
    private readonly List<int> _sensorsToDelete = new();
    private const int SensorLimit = 100;

    private static object Sensor(string name = "Sensor BDD", decimal max = SensorLimit) => new
    {
        name, location = "Laboratorio de testes", maxConsumptionLimit = max, isActive = true
    };

    private async Task Send(string method, string path, object? payload = null, bool auth = false)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), path);
        if (payload is not null)
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        if (auth && _token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _response?.Dispose();
        _response = await _client.SendAsync(request);
        _body = await _response.Content.ReadAsStringAsync();
    }

    private JsonElement Json() => JsonDocument.Parse(_body).RootElement.Clone();
    private int ExtractId() => Json().GetProperty("id").GetInt32();

    [Given("I have an access token")]
    public async Task Login()
    {
        await Send("POST", "/api/auth/login");
        Assert.Equal(200, (int)_response!.StatusCode);
        _token = Json().GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(_token));
    }

    [Given("I have created a sensor")]
    public async Task ExistingSensor()
    {
        if (string.IsNullOrEmpty(_token)) await Login();
        await CreateSensor();
        Assert.Equal(201, (int)_response!.StatusCode);
    }

    [When("I create a valid sensor")]
    public async Task CreateSensor()
    {
        await Send("POST", "/api/sensors", Sensor(), true);
        if ((int)_response!.StatusCode == 201)
        {
            _sensorId = ExtractId();
            _sensorsToDelete.Add(_sensorId);
        }
    }

    [When("I update the created sensor")]
    public async Task UpdateSensor() => await Send("PUT", $"/api/sensors/{_sensorId}", Sensor("Sensor BDD Atualizado"), true);

    [When("I delete the created sensor")]
    public async Task DeleteSensor()
    {
        await Send("DELETE", $"/api/sensors/{_sensorId}", auth: true);
        if (_response!.IsSuccessStatusCode) _sensorsToDelete.Remove(_sensorId);
    }

    [When("I get the created sensor")]
    public async Task GetSensor() => await Send("GET", $"/api/sensors/{_sensorId}");

    [When("I register a consumption of {int} liters")]
    public async Task CreateConsumption(int liters)
    {
        await Send("POST", "/api/consumptions", new { sensorId = _sensorId, consumptionLiters = liters, readingDate = DateTime.UtcNow }, true);
        if ((int)_response!.StatusCode == 201) _consumptionId = ExtractId();
    }

    [When("I register consumption for a nonexistent sensor")]
    public async Task InvalidConsumption() => await Send("POST", "/api/consumptions", new { sensorId = 2147483647, consumptionLiters = 50, readingDate = DateTime.UtcNow }, true);

    [When("I get the created consumption")]
    public async Task GetConsumption() => await Send("GET", $"/api/consumptions/{_consumptionId}");

    [When("I list alerts")]
    public async Task ListAlerts() => await Send("GET", "/api/alerts?page=1&pageSize=1000");

    [Then("there must be an alert for the created sensor")]
    public void FindAlert()
    {
        var items = Json().GetProperty("items");
        var match = items.EnumerateArray().FirstOrDefault(a => a.GetProperty("sensorId").GetInt32() == _sensorId && !a.GetProperty("isResolved").GetBoolean());
        Assert.Equal(JsonValueKind.Object, match.ValueKind);
        Assert.Equal("High", match.GetProperty("severity").GetString());
        Assert.Equal("Consumo acima do limite permitido.", match.GetProperty("message").GetString());
        _alertId = match.GetProperty("id").GetInt32();
    }

    [When("I resolve the alert")]
    public async Task ResolveAlert() => await Send("PUT", $"/api/alerts/{_alertId}/resolve", auth: true);

    [When("I list alerts again")]
    public async Task ListAlertsAgain() => await ListAlerts();

    [Then("the generated alert must be resolved")]
    public void CheckResolvedAlert()
    {
        var items = Json().GetProperty("items");
        var alert = items.EnumerateArray().FirstOrDefault(a => a.GetProperty("id").GetInt32() == _alertId);
        Assert.Equal(JsonValueKind.Object, alert.ValueKind);
        Assert.True(alert.GetProperty("isResolved").GetBoolean());
        Assert.Equal(JsonValueKind.String, alert.GetProperty("resolvedAt").ValueKind);
    }

    [Then("no alert must exist for the created sensor")]
    public void CheckNoSensorAlert()
    {
        var items = Json().GetProperty("items");
        Assert.DoesNotContain(items.EnumerateArray(), a => a.GetProperty("sensorId").GetInt32() == _sensorId);
    }

    [When("I send a request with an invalid bearer token to the sensors API")]
    public async Task InvalidJwt()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/sensors")
        {
            Content = new StringContent(JsonSerializer.Serialize(Sensor()), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.invalid.invalid");
        _response?.Dispose();
        _response = await _client.SendAsync(request);
        _body = await _response.Content.ReadAsStringAsync();
    }

    [Then("the JWT must have three segments")]
    public void CheckJwtSegments()
    {
        var token = Json().GetProperty("token").GetString() ?? "";
        Assert.Equal(3, token.Split('.').Length);
    }

    [Then("the consumption response belongs to the created sensor")]
    public void CheckConsumptionSensor() => Assert.Equal(_sensorId, Json().GetProperty("sensorId").GetInt32());

    [When("I send {word} {string} without authentication")]
    public async Task GetUnauthenticated(string method, string path) => await Send(method, path);

    [When("I send {word} {string} without authentication using a valid sensor")]
    public async Task UnauthorizedSensor(string method, string path) => await Send(method, path, Sensor());

    [When("I send {word} {string} without authentication using valid consumption")]
    public async Task UnauthorizedConsumption(string method, string path) => await Send(method, path, new { sensorId = 1, consumptionLiters = 50 });

    [When("I send POST {string} with invalid sensor")]
    public async Task InvalidSensor(string path) => await Send("POST", path, Sensor("Sensor inválido", 0), true);

    [When("I send PUT {string} with valid sensor")]
    public async Task UpdateMissing(string path) => await Send("PUT", path, Sensor(), true);

    [When("I send DELETE {string} with authentication")]
    public async Task DeleteMissing(string path) => await Send("DELETE", path, auth: true);

    [When("I send PUT {string} with authentication")]
    public async Task ResolveMissing(string path) => await Send("PUT", path, auth: true);

    [Then("the response status must be {int}")]
    public void CheckStatus(int code) => Assert.Equal(code, (int)_response!.StatusCode);

    [Then("the response text must be {string}")]
    public void CheckText(string value) => Assert.Equal(value, _body.Trim());

    [Then("the JSON field {string} must be nonempty")]
    public void CheckNonempty(string property) => Assert.False(string.IsNullOrWhiteSpace(Json().GetProperty(property).GetString()));

    [Then("the JSON field {string} must equal {string}")]
    public void CheckString(string property, string expected) => Assert.Equal(expected, Json().GetProperty(property).GetString());

    [Then("the JSON field {string} must be {word}")]
    public void CheckBoolean(string property, string expected) => Assert.Equal(bool.Parse(expected), Json().GetProperty(property).GetBoolean());

    [Then("the response matches schema {string}")]
    public async Task CheckSchema(string schemaName)
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Schemas", schemaName + ".json");
        Assert.True(File.Exists(schemaPath), $"Schema inexistente: {schemaPath}");
        var schema = await JsonSchema.FromFileAsync(schemaPath);
        var errors = schema.Validate(_body);
        Assert.True(errors.Count == 0, $"Schema '{schemaName}' inválido: {string.Join("; ", errors.Select(e => e.ToString()))}\nBODY: {_body}");
    }

    [Then("the environmental impact report contains the created sensor reading")]
    public void VerifyEnvironmentalImpact()
    {
        var items = Json().GetProperty("items");
        Assert.Contains(items.EnumerateArray(), item =>
            item.GetProperty("id").GetInt32() == _consumptionId &&
            item.GetProperty("consumptionLiters").GetDecimal() == 150m &&
            item.GetProperty("hasLeakSuspicion").GetBoolean());
    }

    [Then("the waste summary includes at least {int} liters of suspected waste")]
    public void VerifyWasteSummary(int minimum)
    {
        var report = Json();
        var total = report.GetProperty("totalConsumptionLiters").GetDecimal();
        var waste = report.GetProperty("suspectedWasteLiters").GetDecimal();
        Assert.True(total >= minimum, $"Total consumption {total} is below {minimum}.");
        Assert.True(waste >= minimum, $"Suspected waste {waste} is below {minimum}.");
        Assert.True(waste <= total, $"Suspected waste {waste} exceeds consumption {total}.");
    }

    [Then("the pagination must indicate page {int} and page size {int}")]
    public void VerifyPagination(int page, int pageSize)
    {
        var response = Json();
        Assert.Equal(page, response.GetProperty("page").GetInt32());
        Assert.Equal(pageSize, response.GetProperty("pageSize").GetInt32());
        Assert.True(response.GetProperty("items").GetArrayLength() <= pageSize);
    }

    [AfterScenario]
    public async Task Cleanup()
    {
        try
        {
            foreach (var id in _sensorsToDelete.AsEnumerable().Reverse())
            {
                // No modelo EF Core original, leituras/alertas vinculados usam exclusão em cascata.
                // A limpeza ocorre apenas no banco descartável de testes.
                await Send("DELETE", $"/api/sensors/{id}", auth: true);
                Assert.Equal(200, (int)_response!.StatusCode);
            }
        }
        finally
        {
            _response?.Dispose();
            _client.Dispose();
        }
    }
}
