using OOPLAB1.Vehicles;

namespace OOPLAB1.UI;

// Actions that only some vehicle types have: passengers, cargo, trailer, sidecar, charging.
// The menu is built for the chosen vehicle, so it shows only what this vehicle can do.
public static class VehicleActionsMenu
{
    public static void Run(Vehicle vehicle)
    {
        var menu = new Menu($"SPECIAL ACTIONS: {vehicle.RegistrationNumber}", "Back",
                            () => DescribeState(vehicle));

        if (vehicle.MaxPassengers > 0)
        {
            menu.Add("Board passengers", () => BoardPassengers(vehicle))
                .Add("Drop off passengers", () => DropOffPassengers(vehicle));
        }

        switch (vehicle)
        {
            case Truck truck:
                menu.Add("Load cargo", () => LoadCargo(truck))
                    .Add("Unload cargo", () => UnloadCargo(truck))
                    .Add("Attach trailer", () => AttachTrailer(truck))
                    .Add("Detach trailer", () => DetachTrailer(truck));
                break;
            case Motorcycle motorcycle:
                menu.Add("Attach sidecar", () => AttachSidecar(motorcycle))
                    .Add("Detach sidecar", () => DetachSidecar(motorcycle));
                break;
            case ElectricCar electricCar:
                menu.Add("Time to full charge", () => ShowTimeToFullCharge(electricCar))
                    .Add("Cost of full charge", () => ShowChargingCost(electricCar));
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
        switch (vehicle)
        {
            case Truck truck:
                parts.Add($"cargo {truck.CurrentCargo:F0} / {truck.TotalCapacity:F0} kg");
                parts.Add(truck.HasTrailer ? $"trailer {truck.TrailerCapacity:F0} kg" : "no trailer");
                break;
            case Motorcycle motorcycle:
                parts.Add(motorcycle.HasSidecar ? "with sidecar" : "no sidecar");
                break;
            case ElectricCar electricCar:
                parts.Add(electricCar.EnergyStatus);
                break;
        }
        if (vehicle.MaxPassengers > 0)
            parts.Add($"passengers {vehicle.Passengers} / {vehicle.MaxPassengers}");
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

    private static void LoadCargo(Truck truck)
    {
        if (truck.FreeCapacity < 0.1)
        {
            Console.WriteLine("The truck is fully loaded.");
            return;
        }
        double weight = InputReader.ReadDouble($"Weight (kg, up to {truck.FreeCapacity:0.#}): ",
                                               0.1, truck.FreeCapacity);
        truck.LoadCargo(weight);
        ConsolePrinter.Success($"Loaded {weight:0.#} kg, the truck is {truck.LoadPercent:F0}% full.");
    }

    private static void UnloadCargo(Truck truck)
    {
        if (truck.CurrentCargo < 0.1)
        {
            Console.WriteLine("The truck is empty.");
            return;
        }
        double weight = InputReader.ReadDouble($"Weight (kg, up to {truck.CurrentCargo:0.#}): ",
                                               0.1, truck.CurrentCargo);
        truck.UnloadCargo(weight);
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
        ConsolePrinter.Success($"Trailer attached, total capacity {truck.TotalCapacity:F0} kg.");
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

    private static void ShowChargingCost(ElectricCar car)
    {
        double price = InputReader.ReadDouble("Electricity price (EUR per kWh): ", 0, 5);
        double cost = car.CalculateChargingCost(price);
        Console.WriteLine($"Charging {car.FreeBatteryCapacity:F1} kWh to 100% costs {cost:F2} EUR.");
    }
}
