using Dapr.Client;

namespace TrafficControlService.Repositories;

public class DaprVehicleStateRepository : IVehicleStateRepository
{
    private const string DAPR_STORE_NAME = "statestore";

    private readonly DaprClient _daprClient;

    public DaprVehicleStateRepository(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<VehicleState?> GetVehicleStateAsync(string licenseNumber)
    {
        return await _daprClient.GetStateAsync<VehicleState>(
            DAPR_STORE_NAME, licenseNumber);        
    }

    public async Task SaveVehicleStateAsync(VehicleState vehicleState)
    {
        await _daprClient.SaveStateAsync(
            DAPR_STORE_NAME, vehicleState.LicenseNumber, vehicleState);
    }
}