using OOPLAB1.Vehicles.Air;
using OOPLAB1.Vehicles.Air.Balloons;
using OOPLAB1.Vehicles.Air.Helicopters;

namespace OOPLAB1.UI;

// Flight in the console: the user enters the distance, the average wind and the altitude,
// sees the flight plan with the fuel check and decides whether to take off.
public static class FlightDialog
{
    public static void Run(AirVehicle aircraft)
    {
        Console.WriteLine($"{aircraft.EnergyStatus}, speed {aircraft.CruiseSpeed:F0} km/h, " +
                          $"max altitude {aircraft.AltitudeLimit:F0} m.");
        double distance = InputReader.ReadDouble("Flight distance (km): ", 1, 20_000);
        double wind = ReadWind(aircraft);
        if (aircraft.GroundSpeed(wind) <= 0)
        {
            Console.WriteLine($"With this wind {aircraft.TypeName} cannot move forward.");
            return;
        }
        if (aircraft is Helicopter helicopter)
            ChooseClimbAngle(helicopter);

        // Distance, wind and the climb angle are asked first: a short flight does not allow
        // a high altitude.
        double maxAltitude = Math.Floor(aircraft.MaxAltitudeFor(distance, wind));
        if (maxAltitude < AirVehicle.MinAltitude)
        {
            Console.WriteLine($"{distance:0.#} km is too short to climb even to {AirVehicle.MinAltitude} m.");
            return;
        }
        double altitude = InputReader.ReadDouble(
            $"Flight altitude (m, {AirVehicle.MinAltitude}-{maxAltitude:F0}): ", AirVehicle.MinAltitude, maxAltitude);

        FlightPlan plan = aircraft.PlanFlight(distance, altitude, wind);
        PrintPlan(plan, aircraft);
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

        aircraft.Fly(distance, altitude, wind);
        ConsolePrinter.Success($"Landed after {ConsolePrinter.FormatHours(plan.FlightHours)}. " +
                               $"{aircraft.EnergyStatus}, mileage {aircraft.Mileage:F0} km.");
    }

    // A balloon has no engine and only flies with the wind; the others may meet a headwind.
    private static double ReadWind(AirVehicle aircraft)
    {
        if (aircraft is Balloon and not Airship)
            return InputReader.ReadDouble("Wind speed (km/h, the balloon flies with it): ", 1, 150);
        return InputReader.ReadDouble("Average wind (km/h, + tailwind, - headwind): ", -150, 150);
    }

    // A helicopter pilot chooses the climb angle for every flight.
    private static void ChooseClimbAngle(Helicopter helicopter)
    {
        double angle = InputReader.ReadDouble(
            $"Climb angle (degrees, {Helicopter.MinClimbAngle}-{helicopter.AllowedClimbAngle:0.#}): ",
            Helicopter.MinClimbAngle, helicopter.AllowedClimbAngle);
        helicopter.SetClimbAngle(angle);
    }

    private static void PrintPlan(FlightPlan plan, AirVehicle aircraft)
    {
        Console.WriteLine($"Flight plan: {plan.Distance:0.#} km at {plan.Altitude:F0} m");
        Console.WriteLine($"  Wind {plan.Wind:+0;-0;0} km/h, ground speed {plan.GroundSpeed:F0} km/h");
        Console.WriteLine($"  Climb:   {Slope(plan.ClimbDistance, plan.Altitude),-22} (fuel x{AirVehicle.ClimbFuelFactor})");
        Console.WriteLine($"  Cruise:  {plan.CruiseDistance:F1} km");
        Console.WriteLine($"  Descent: {Slope(plan.DescentDistance, plan.Altitude),-22} (fuel x{AirVehicle.DescentFuelFactor})");
        Console.WriteLine($"  Path {plan.PathLength:F1} km, flight time {ConsolePrinter.FormatHours(plan.FlightHours)}");
        string reserve = aircraft.FuelReserve > 0
            ? $" + {aircraft.FuelReserve * 100:0}% reserve = {plan.FuelRequired:F1} L"
            : " (no reserve needed)";
        Console.WriteLine($"  Fuel: {plan.FuelBurned:F1} L{reserve}, in the tank {aircraft.FuelLevel:F1} L");
    }

    // "49.2 km at 3.4 deg", or "vertical" when the slope has no horizontal part.
    private static string Slope(double horizontalKm, double altitude)
    {
        if (horizontalKm < 0.001)
            return "vertical";
        double degrees = Math.Atan(altitude / 1000 / horizontalKm) * 180 / Math.PI;
        return $"{horizontalKm:F1} km at {degrees:F1} deg";
    }
}
