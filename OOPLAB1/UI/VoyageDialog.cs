using OOPLAB1.Fuel;
using OOPLAB1.Vehicles.Water;
using OOPLAB1.Vehicles.Water.Boats;

namespace OOPLAB1.UI;

// Voyage in the console: the user enters the distance, the current, the smallest depth of
// the route (and the wind for a sailboat, the wind from the menu is the default), sees the
// voyage plan with the fuel check and decides whether to set sail.
public static class VoyageDialog
{
    public static void Run(WaterVehicle vessel, Wind defaultWind)
    {
        Console.WriteLine($"{vessel.EnergyStatus}, speed {vessel.SpeedThroughWater:F1} km/h, " +
                          $"draft {vessel.Draft:F1} m.");
        double distance = InputReader.ReadDouble("Voyage distance (km): ", 1, 30_000);
        double current = InputReader.ReadDouble("Current (km/h, + with the current, - against): ", -30, 30);
        if (vessel is Sailboat sailboat)
            sailboat.SetWind(InputReader.ReadDoubleOrDefault(
                $"Average wind for the sails (km/h), Enter = {defaultWind.Speed:0.#}: ",
                0, Wind.MaxSpeed, defaultWind.Speed));
        double depth = InputReader.ReadDouble("Smallest depth on the route (m): ", 0.1, 11_000);

        VoyagePlan plan = vessel.PlanVoyage(distance, current, depth);
        PrintPlan(plan, vessel);
        if (!vessel.HasEnoughFuel(plan))
        {
            ConsolePrinter.Error($"Voyage cancelled: not enough fuel, " +
                                 $"{plan.FuelRequired - vessel.FuelLevel:F1} L more is needed.");
            return;
        }
        if (!InputReader.ReadYesNo("Set sail?"))
        {
            Console.WriteLine("The voyage was not started.");
            return;
        }

        vessel.Sail(distance, current, depth);
        ConsolePrinter.Success($"Arrived after {ConsolePrinter.FormatHours(plan.Hours)}. " +
                               $"{vessel.EnergyStatus}, mileage {vessel.Mileage:F0} km.");
    }

    private static void PrintPlan(VoyagePlan plan, WaterVehicle vessel)
    {
        double extra = plan.WaterDistance - plan.Distance;
        string currentEffect = Math.Abs(extra) < 0.05 ? "no current"
            : extra > 0 ? $"+{extra:F1} km against the current"
            : $"{extra:F1} km thanks to the current";
        Console.WriteLine($"Voyage plan: {plan.Distance:0.#} km, current {plan.Current:+0.#;-0.#;0} km/h");
        Console.WriteLine($"  Speed: {plan.SpeedThroughWater:F1} km/h through the water, {plan.GroundSpeed:F1} km/h over the ground");
        Console.WriteLine($"  Way through the water: {plan.WaterDistance:F1} km ({currentEffect})");
        Console.WriteLine($"  Draft {plan.Draft:F1} m, route depth {plan.RouteDepth:F1} m");
        Console.WriteLine($"  Time {ConsolePrinter.FormatHours(plan.Hours)}");
        string reserve = vessel.FuelReserve > 0
            ? $" + {vessel.FuelReserve * 100:0}% reserve = {plan.FuelRequired:F1} L"
            : "";
        Console.WriteLine($"  Fuel: {plan.FuelBurned:F1} L{reserve}, on board {vessel.FuelLevel:F1} L");
    }
}
