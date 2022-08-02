namespace TrafficControlService.Repositories;

public class DaprVehicleStateRepository : IVehicleStateRepository
{
    private const string DAPR_STORE_NAME = "statestore";

    private readonly HttpClient _httpClient;

    public DaprVehicleStateRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VehicleState?> GetVehicleStateAsync(string licenseNumber)
    {
        var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
        var state = await _httpClient.GetFromJsonAsync<VehicleState>(
            $"http://localhost:{daprHttpPort}/v1.0/state/{DAPR_STORE_NAME}/{licenseNumber}");

        return state;    
    }

    public async Task SaveVehicleStateAsync(VehicleState vehicleState)
    {
        var state = new[]
        {
            new { 
                key = vehicleState.LicenseNumber,
                value = vehicleState
            }
        };

        var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
        await _httpClient.PostAsJsonAsync(
            $"http://localhost:{daprHttpPort}/v1.0/state/{DAPR_STORE_NAME}",
            state);    
    }
}