﻿using Dapr.Client;

namespace TrafficControlService.Controllers;

[ApiController]
[Route("")]
public class TrafficController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IVehicleStateRepository _vehicleStateRepository;
    private readonly ILogger<TrafficController> _logger;
    private readonly ISpeedingViolationCalculator _speedingViolationCalculator;
    private readonly string _roadId;

    public TrafficController(
        ILogger<TrafficController> logger,
        HttpClient httpClient,
        IVehicleStateRepository vehicleStateRepository,
        ISpeedingViolationCalculator speedingViolationCalculator)
    {
        _logger = logger;
        _httpClient = httpClient;
        _vehicleStateRepository = vehicleStateRepository;
        _speedingViolationCalculator = speedingViolationCalculator;
        _roadId = speedingViolationCalculator.GetRoadId();
    }

    [HttpPost("entrycam")]
    public async Task<ActionResult> VehicleEntry(VehicleRegistered msg)
    {
        try
        {
            // log entry
            _logger.LogInformation($"ENTRY detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of vehicle with license-number {msg.LicenseNumber}.");

            // store vehicle state
            var vehicleState = new VehicleState
            {
                LicenseNumber = msg.LicenseNumber,
                EntryTimestamp = msg.Timestamp
            };
            await _vehicleStateRepository.SaveVehicleStateAsync(vehicleState);

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost("exitcam")]
    public async Task<ActionResult> VehicleExit(VehicleRegistered msg, [FromServices] DaprClient daprClient)
    {
        try
        {
            // get vehicle state
            var state = await _vehicleStateRepository.GetVehicleStateAsync(msg.LicenseNumber);
            if (state == default(VehicleState))
            {
                return NotFound();
            }

            // log exit
            _logger.LogInformation($"EXIT detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of vehicle with license-number {msg.LicenseNumber}.");

            // update state
            var exitState = state.Value with { ExitTimestamp = msg.Timestamp };
            await _vehicleStateRepository.SaveVehicleStateAsync(exitState);

            // handle possible speeding violation
            int violation = _speedingViolationCalculator.DetermineSpeedingViolationInKmh(exitState.EntryTimestamp, exitState.ExitTimestamp.Value);
            if (violation > 0)
            {
                _logger.LogInformation($"Speeding violation detected ({violation} KMh) of vehicle" +
                    $"with license-number {state.Value.LicenseNumber}.");

                var speedingViolation = new SpeedingViolation
                {
                    VehicleId = msg.LicenseNumber,
                    RoadId = _roadId,
                    ViolationInKmh = violation,
                    Timestamp = msg.Timestamp
                };

                await daprClient.PublishEventAsync("pubsub", "collectfine", speedingViolation);
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }
}