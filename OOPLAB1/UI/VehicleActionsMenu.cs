using OOPLAB1.Fuel;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;

namespace OOPLAB1.UI;

// Actions that only some vehicles have: passengers, cargo, trailer, sidecar, charging.
// The menu is built for the chosen vehicle, so it shows only what this vehicle can do.
public static class VehicleActionsMenu
{
    public static void Run(Vehicle vehicle, FuelPrices prices)
    {
        var menu = new Menu($"SPECIAL ACTIONS: {vehicle.RegistrationNumber}", "Back",
                            () => DescribeState(vehicle));

        if (vehicle.MaxPassengers > 0)
        {
            menu.Add("Board passengers", () => BoardPassengers(vehicle))
                .Add("Drop off passengers", () => DropOffPassengers(vehicle));
        }
        if (vehicle.MaxCargo > 0)
        {
            menu.Add("Load cargo", () => LoadCargo(vehicle))
                .Add("Unload cargo", () => UnloadCargo(vehicle));
        }

        switch (vehicle)
        {
            case Truck truck:
                menu.Add("Attach trailer", () => AttachTrailer(truck))
                    .Add("Detach trailer", () => DetachTrailer(truck));
                break;
            case Motorcycle motorcycle:
                menu.Add("Attach sidecar", () => AttachSidecar(motorcycle))
                    .Add("Detach sidecar", () => DetachSidecar(motorcycle));
                break;
            case ElectricCar electricCar:
                menu.Add("Time to full charge", () => ShowTimeToFullCharge(electricCar))
                    .Add("Cost of full charge", () => ShowChargingCost(electricCar, prices));
                break;
        }

        if (menu.Count == 0)
        {
            Console.WriteLine($"{vehicle.TypeName} has no special actions.");
            return;
        }
        menu.Run();
    }

    // Status line under the menu title: shows how the actions change the vehicle.
    private static string DescribeState(Vehicle vehicle)
    {
        var parts = new List<string> { $"{vehicle.Brand} {vehicle.Model}" };
        if (vehicle.MaxPassengers > 0)
            parts.Add($"passengers {vehicle.Passengers} / {vehicle.MaxPassengers}");
        if (vehicle.MaxCargo > 0)
            parts.Add($"cargo {vehicle.CurrentCargo:F0} / {vehicle.MaxCargo:F0} kg");
        switch (vehicle)
        {
            case Truck truck:
                parts.Add(truck.HasTrailer ? $"trailer {truck.TrailerCapacity:F0} kg" : "no trailer");
                break;
            case Motorcycle motorcycle:
                parts.Add(motorcycle.HasSidecar ? "with sidecar" : "no sidecar");
                break;
            case ElectricCar electricCar:
                parts.Add(electricCar.EnergyStatus);
                break;
            case AirVehicle aircraft:
                parts.Add($"fuel reserve {aircraft.FuelReserve * 100:0}%");
                break;
        }
        parts.Add($"consumption {vehicle.FuelConsumption(100):F2} {vehicle.FuelUnit}/100 km");
        return string.Join(", ", parts);
    }

    private static void BoardPassengers(Vehicle vehicle)
    {
        if (vehicle.FreeSeats == 0)
        {
            Console.WriteLine("All seats are taken.");
            return;
        }
        int count = InputReader.ReadInt($"How many passengers (1-{vehicle.FreeSeats}): ",
                                        1, vehicle.FreeSeats);
        vehicle.BoardPassengers(count);
        ConsolePrinter.Success($"{count} passenger(s) boarded.");
    }

    private static void DropOffPassengers(Vehicle vehicle)
    {
        if (vehicle.Passengers == 0)
        {
            Console.WriteLine("There are no passengers.");
            return;
        }
        int count = InputReader.ReadInt($"How many passengers (1-{vehicle.Passengers}): ",
                                        1, vehicle.Passengers);
        vehicle.DropOffPassengers(count);
        ConsolePrinter.Success($"{count} passenger(s) got off.");
    }

    private static void LoadCargo(Vehicle vehicle)
    {
        if (vehicle.FreeCargoCapacity < 0.1)
        {
            Console.WriteLine("It is fully loaded.");
            return;
        }
        double weight = InputReader.ReadDouble($"Weight (kg, up to {vehicle.FreeCargoCapacity:0.#}): ",
                                               0.1, vehicle.FreeCargoCapacity);
        vehicle.LoadCargo(weight);
        ConsolePrinter.Success($"Loaded {weight:0.#} kg, {vehicle.LoadPercent:F0}% of the capacity is used.");
    }

    private static void UnloadCargo(Vehicle vehicle)
    {
        if (vehicle.CurrentCargo < 0.1)
        {
            Console.WriteLine("There is no cargo.");
            return;
        }
        double weight = InputReader.ReadDouble($"Weight (kg, up to {vehicle.CurrentCargo:0.#}): ",
                                               0.1, vehicle.CurrentCargo);
        vehicle.UnloadCargo(weight);
        ConsolePrinter.Success($"Unloaded {weight:0.#} kg.");
    }

    private static void AttachTrailer(Truck truck)
    {
        if (truck.HasTrailer)
        {
            Console.WriteLine("A trailer is already attached.");
            return;
        }
        double capacity = InputReader.ReadDouble("Trailer capacity (kg): ", 100, 30000);
        truck.AttachTrailer(capacity);
        ConsolePrinter.Success($"Trailer attached, total capacity {truck.MaxCargo:F0} kg.");
    }

    private static void DetachTrailer(Truck truck)
    {
        truck.DetachTrailer();
        ConsolePrinter.Success("Trailer detached.");
    }

    private static void AttachSidecar(Motorcycle motorcycle)
    {
        motorcycle.AttachSidecar();
        ConsolePrinter.Success("Sidecar attached.");
    }

    private static void DetachSidecar(Motorcycle motorcycle)
    {
        motorcycle.DetachSidecar();
        ConsolePrinter.Success("Sidecar detached.");
    }

    private static void ShowTimeToFullCharge(ElectricCar car)
    {
        double power = InputReader.ReadDouble("Charger power (kW): ", 1, 350);
        string time = ConsolePrinter.FormatHours(car.TimeToFullCharge(power));
        Console.WriteLine($"Charging {car.FreeBatteryCapacity:F1} kWh at {power:0.#} kW takes {time}.");
    }

    private static void ShowChargingCost(ElectricCar car, FuelPrices prices)
    {
        double cost = car.CalculateChargingCost(prices.GetPrice(FuelType.Electric));
        Console.WriteLine($"Charging {car.FreeBatteryCapacity:F1} kWh to 100% costs {cost:F2} EUR " +
                          $"({prices.Describe(FuelType.Electric)}).");
    }
}
