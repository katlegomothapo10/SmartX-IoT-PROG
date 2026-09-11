using SmartX.API.Models;
using SmartX.API.Services;

namespace SmartX.API.Utils;

public static class DataSeeder
{
    public static void SeedSensors(SensorDataService service)
    {
        var sensors = new[]
        {
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:01",
                DeploymentLocation = "Hydro Bay 1 / Tray A",
                Category = SensorType.Environmental,
                ZoneId = "zone-1",
                FacilityId = "facility-a",
                Description = "Soil moisture sensor"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:02",
                DeploymentLocation = "Hydro Bay 1 / Tray B",
                Category = SensorType.Environmental,
                ZoneId = "zone-1",
                FacilityId = "facility-a",
                Description = "Temperature and humidity sensor"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:03",
                DeploymentLocation = "Zone 1 / Power Panel",
                Category = SensorType.PowerConsumption,
                ZoneId = "zone-1",
                FacilityId = "facility-a",
                Description = "Main power meter"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:04",
                DeploymentLocation = "Zone 2 / Pump Room",
                Category = SensorType.PowerConsumption,
                ZoneId = "zone-2",
                FacilityId = "facility-a",
                Description = "Pump circuit meter"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:05",
                DeploymentLocation = "Hydro Bay 2 / Valve 1",
                Category = SensorType.Actuator,
                ZoneId = "zone-2",
                FacilityId = "facility-a",
                Description = "Irrigation valve actuator"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:06",
                DeploymentLocation = "Hydro Bay 2 / Valve 2",
                Category = SensorType.Actuator,
                ZoneId = "zone-2",
                FacilityId = "facility-a",
                Description = "Drain valve actuator"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:07",
                DeploymentLocation = "Zone 3 / Grid Tie",
                Category = SensorType.PowerConsumption,
                ZoneId = "zone-3",
                FacilityId = "facility-a",
                Description = "Solar grid tie meter"
            },
            new Sensor
            {
                MacAddress = "AA:BB:CC:DD:EE:08",
                DeploymentLocation = "Zone 3 / Battery Bank",
                Category = SensorType.Environmental,
                ZoneId = "zone-3",
                FacilityId = "facility-a",
                Description = "Battery temperature sensor"
            }
        };

        foreach (var sensor in sensors)
        {
            try
            {
                service.RegisterSensor(sensor);
            }
            catch (InvalidOperationException)
            {
                // already seeded
            }
        }

        service.SimulateBulkTelemetry();
    }
}