using OOPLAB1.Vehicles.Ground;

namespace OOPLAB1.UI;

// Trip of a ground vehicle: the road (it gives the speed limit) and the average gradient are
// asked first, because they change the consumption and the range, then the distance.
// An expired technical inspection is only a warning.
public static class DriveDialog
{
    private static readonly RoadType[] Roads = { RoadType.City, RoadType.Highway };

    public static void Run(GroundVehicle vehicle)
    {
        if (vehicle.InspectionExpired)
            ConsolePrinter.Warning($"Technical inspection expired on {vehicle.InspectionValidUntil:yyyy-MM-dd}: " +
                                   "pass it in Special actions.");

        var lines = Roads.Select(road =>
            $"{GroundVehicle.RoadName(road),-8} {vehicle.SpeedLimitOn(road):0} km/h").ToList();
        int choice = InputReader.Choose("Road:", lines);
        if (choice < 0)
            return;
        double gradient = InputReader.ReadDoubleOrDefault(
            "Average gradient (%, + uphill, - downhill), Enter = 0: ",
            -GroundVehicle.MaxGradient, GroundVehicle.MaxGradient, 0);
        vehicle.SetRoad(Roads[choice], gradient);

        string unit = vehicle.FuelUnit;
        Console.WriteLine($"{vehicle.EnergyStatus}, consumption {vehicle.FuelConsumption(100):F2} {unit}/100 km " +
                          $"on this road, range {vehicle.Range:F0} km.");
        double distance = InputReader.ReadDouble("Distance (km): ", 0.1, 100_000);
        double used = vehicle.FuelConsumption(distance);
        double hours = vehicle.TravelHours(distance);

        vehicle.Drive(distance);
        ConsolePrinter.Success($"Drove {distance:0.#} km in {ConsolePrinter.FormatHours(hours)} and used " +
                               $"{used:F2} {unit}. {vehicle.EnergyStatus}, mileage {vehicle.Mileage:F0} km.");
    }
}
