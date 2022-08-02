using System.Net.Mqtt;

namespace Simulation.Proxies;

public class MqttTrafficControlService : ITrafficControlService
{
    private readonly IMqttClient _client;

    public MqttTrafficControlService(int camNumber)
    {
        // connect to mqtt broker
        var config = new MqttConfiguration() {
            KeepAliveSecs = 60,
            Port = 1883
        };
        var mqttHost = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "localhost";
        _client = MqttClient.CreateAsync(mqttHost, config).Result;
        var sessionState = _client.ConnectAsync(
            new MqttClientCredentials(clientId: $"camerasim{camNumber}")).Result;
    }

    public void SendVehicleEntry(VehicleRegistered vehicleRegistered)
    {
        var eventJson = JsonSerializer.Serialize(vehicleRegistered);
        var message = new MqttApplicationMessage("trafficcontrol/entrycam", Encoding.UTF8.GetBytes(eventJson));
        _client.PublishAsync(message, MqttQualityOfService.AtMostOnce).Wait();
    }

    public void SendVehicleExit(VehicleRegistered vehicleRegistered)
    {
        var eventJson = JsonSerializer.Serialize(vehicleRegistered);
        var message = new MqttApplicationMessage("trafficcontrol/exitcam", Encoding.UTF8.GetBytes(eventJson));
        _client.PublishAsync(message, MqttQualityOfService.AtMostOnce).Wait();
    }
}
