using OOPLAB1.Vehicles.Air;

namespace OOPLAB1.UI;

// Flight in the console: the user enters the distance and the altitude, sees the flight
// plan with the fuel check and decides whether to take off.
public static class FlightDialog
{
    public static void Run(AirVehicle aircraft)
    {
        Console.WriteLine($"{aircraft.EnergyStatus}, cruise speed {aircraft.CruiseSpeed:F0} km/h, " +
                          $"max altitude {aircraft.MaxAltitude:F0} m.");
        double distance = InputReader.ReadDouble("Flight distance (km): ", 1, 20_000);

        // The distance is asked first: a short flight does not allow a high altitude.
        double maxAltitude = Math.Floor(aircraft.MaxAltitudeFor(distance));
        if (maxAltitude < AirVehicle.MinAltitude)
        {
            Console.WriteLine($"{distance:0.#} km is too short to climb even to {AirVehicle.MinAltitude} m.");
            return;
        }
        double altitude = InputReader.ReadDouble(
            $"Cruise altitude (m, {AirVehicle.MinAltitude}-{maxAltitude:F0}): ", AirVehicle.MinAltitude, maxAltitude);

        FlightPlan plan = aircraft.PlanFlight(distance, altitude);
        PrintPlan(plan, aircraft.FuelLevel);
        if (!aircraft.HasEnoughFuel(plan))
        {
            ConsolePrinter.Error($"Flight cancelled: not enough fuel, " +
                                 $"{plan.FuelRequired - aircraft.FuelLevel:F1} L more is needed.");
            return;
        }
        if (!InputReader.ReadYesNo("Take off?"))
        {
            Console.WriteLine("The flight was not started.");
            return;
        }

        aircraft.Fly(distance, altitude);
        ConsolePrinter.Success($"Landed after {ConsolePrinter.FormatHours(plan.FlightHours)}. " +
                               $"{aircraft.EnergyStatus}, mileage {aircraft.Mileage:F0} km.");
    }

    private static void PrintPlan(FlightPlan plan, double fuelInTank)
    {
        Console.WriteLine($"Flight plan: {plan.Distance:0.#} km at {plan.Altitude:F0} m");
        Console.WriteLine($"  Climb:   {plan.ClimbDistance,8:F1} km   (fuel x{AirVehicle.ClimbFuelFactor})");
        Console.WriteLine($"  Cruise:  {plan.CruiseDistance,8:F1} km");
        Console.WriteLine($"  Descent: {plan.DescentDistance,8:F1} km   (fuel x{AirVehicle.DescentFuelFactor})");
        Console.WriteLine($"  Path {plan.PathLength:F1} km, flight time {ConsolePrinter.FormatHours(plan.FlightHours)}");
        Console.WriteLine($"  Fuel: {plan.FuelBurned:F1} L + {AirVehicle.FuelReserve * 100:0}% reserve = " +
                          $"{plan.FuelRequired:F1} L, in the tank {fuelInTank:F1} L");
    }
}
