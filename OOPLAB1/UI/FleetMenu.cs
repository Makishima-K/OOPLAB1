using OOPLAB1.Fuel;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;
using OOPLAB1.Vehicles.Ground.Cars;
using OOPLAB1.Vehicles.Water;

namespace OOPLAB1.UI;

// Main menu of the program. Works with every vehicle through the Vehicle base class;
// only charging, flying and sailing have to know the exact type
// (ElectricCar, AirVehicle, WaterVehicle).
public sealed class FleetMenu
{
    private readonly Fleet _fleet;
    private readonly FuelPrices _prices;
    private readonly Wind _wind;

    public FleetMenu(Fleet fleet, FuelPrices prices, Wind wind)
    {
        _fleet = fleet;
        _prices = prices;
        _wind = wind;
    }

    public void Run()
    {
        new Menu("VEHICLE FLEET", "Exit", () => $"Vehicles in the fleet: {_fleet.Count}")
            .Add("Add a vehicle", AddVehicle)
            .Add("Show all vehicles", ShowAllVehicles)
            .Add("Vehicle details", ShowDetails)
            .Add("Drive / fly / sail", DriveOrFly)
            .Add("Refuel / charge", RefuelOrCharge)
            .Add("Special actions (passengers, cargo, sidecar...)", SpecialActions)
            .Add("Remove a vehicle", RemoveVehicle)
            .Add("Fuel prices and wind", ChangePriceOrWind)
            .Run();
    }

    private void AddVehicle()
    {
        Vehicle? vehicle = VehicleCreator.Create(_fleet);
        if (vehicle == null)
            return;

        _fleet.Add(vehicle);
        ConsolePrinter.Success($"{vehicle.TypeName} {vehicle.RegistrationNumber} added to the fleet.");
    }

    private void ShowAllVehicles()
    {
        if (_fleet.Count == 0)
        {
            Console.WriteLine("The fleet is empty.");
            return;
        }
        foreach (Vehicle vehicle in _fleet.Vehicles)
            Console.WriteLine(vehicle);
    }

    private void ShowDetails()
    {
        Vehicle? vehicle = SelectVehicle();
        if (vehicle != null)
            Console.WriteLine(vehicle.GetInfo());
    }

    private void DriveOrFly()
    {
        Vehicle? vehicle = SelectVehicle();
        if (vehicle is AirVehicle aircraft)
            FlyOrTaxi(aircraft);
        else if (vehicle is WaterVehicle vessel)
            VoyageDialog.Run(vessel, _wind);
        else if (vehicle != null)
            Drive(vehicle);
    }

    // Aircraft fly; an airplane can also drive on the ground (taxi).
    private void FlyOrTaxi(AirVehicle aircraft)
    {
        int choice = aircraft.CanDrive
            ? InputReader.Choose("Fly or drive?", new[] { "Fly", "Drive on the ground (taxi)" })
            : 0;
        if (choice == 0)
            FlightDialog.Run(aircraft, _wind);
        else if (choice == 1)
            Drive(aircraft);
    }

    private static void Drive(Vehicle vehicle)
    {
        string unit = vehicle.FuelUnit;
        Console.WriteLine($"{vehicle.EnergyStatus}, consumption {vehicle.FuelConsumption(100):F2} " +
                          $"{unit}/100 km, range {vehicle.Range:F0} km.");
        double distance = InputReader.ReadDouble("Distance (km): ", 0.1, 100_000);
        double used = vehicle.FuelConsumption(distance);
        vehicle.Drive(distance);
        ConsolePrinter.Success($"Drove {distance:0.#} km and used {used:F2} {unit}. " +
                               $"{vehicle.EnergyStatus}, mileage {vehicle.Mileage:F0} km.");
    }

    private void RefuelOrCharge()
    {
        Vehicle? vehicle = SelectVehicle();
        if (vehicle is ElectricCar electricCar)
            ChargeBattery(electricCar);
        else if (vehicle != null)
            RefuelTank(vehicle);
    }

    private void RefuelTank(Vehicle vehicle)
    {
        Console.WriteLine($"{vehicle.EnergyStatus}, free space {vehicle.FreeTankCapacity:F1} L.");
        if (vehicle.FreeTankCapacity < 0.1)
        {
            Console.WriteLine("The tank is already full.");
            return;
        }
        double price = _prices.GetPrice(vehicle.FuelType);
        Console.WriteLine($"{vehicle.FuelType} costs {_prices.Describe(vehicle.FuelType)}, " +
                          $"a full tank = {vehicle.FreeTankCapacity * price:F2} EUR.");
        double amount = InputReader.ReadDouble("Amount of fuel (L): ", 0.1, vehicle.FreeTankCapacity);
        double cost = vehicle.Refuel(amount, price);
        ConsolePrinter.Success($"Added {amount:F1} L for {cost:F2} EUR. {vehicle.EnergyStatus}.");
    }

    private void ChargeBattery(ElectricCar car)
    {
        Console.WriteLine($"{car.EnergyStatus}, free {car.FreeBatteryCapacity:F1} kWh.");
        if (car.FreeBatteryCapacity < 0.1)
        {
            Console.WriteLine("The battery is already full.");
            return;
        }
        double price = _prices.GetPrice(FuelType.Electric);
        Console.WriteLine($"Electricity costs {_prices.Describe(FuelType.Electric)}, " +
                          $"a full charge = {car.CalculateChargingCost(price):F2} EUR.");
        double amount = InputReader.ReadDouble("Energy to add (kWh): ", 0.1, car.FreeBatteryCapacity);
        double power = InputReader.ReadDouble("Charger power (kW): ", 1, 350);
        double hours = InputReader.ReadDouble("Available time (h): ", 0.01, 48);

        var (added, spentHours, cost) = car.Charge(amount, power, hours, price);
        ConsolePrinter.Success($"Charged {added:F1} kWh in {ConsolePrinter.FormatHours(spentHours)} " +
                               $"for {cost:F2} EUR. {car.EnergyStatus}.");
        if (added < amount - 0.01)
            ConsolePrinter.Warning($"Not enough time: charging {amount:F1} kWh at {power:0.#} kW " +
                                   $"takes {ConsolePrinter.FormatHours(amount / power)}.");
    }

    private void SpecialActions()
    {
        Vehicle? vehicle = SelectVehicle();
        if (vehicle != null)
            VehicleActionsMenu.Run(vehicle, _prices);
    }

    private void RemoveVehicle()
    {
        Vehicle? vehicle = SelectVehicle();
        if (vehicle == null)
            return;

        if (InputReader.ReadYesNo($"Remove {vehicle.RegistrationNumber} ({vehicle.Brand} {vehicle.Model})?"))
        {
            _fleet.Remove(vehicle);
            ConsolePrinter.Success($"{vehicle.RegistrationNumber} removed from the fleet.");
        }
    }

    // Shows all prices and the wind (the last line); the chosen one gets a new value.
    private void ChangePriceOrWind()
    {
        FuelType[] fuels = Enum.GetValues<FuelType>();
        var lines = fuels.Select(fuel => $"{fuel,-9} {_prices.Describe(fuel)}").ToList();
        lines.Add($"{"Wind",-9} {_wind.Describe()}");
        int index = InputReader.Choose("Fuel prices and wind - choose one to change:", lines);
        if (index < 0)
            return;
        if (index == fuels.Length)
        {
            ChangeWind();
            return;
        }

        FuelType fuelType = fuels[index];
        double price = InputReader.ReadDouble(
            $"New {fuelType} price (EUR/{FuelPrices.UnitOf(fuelType)}): ", 0, 10);
        _prices.SetPrice(fuelType, price);
        ConsolePrinter.Success($"{fuelType} now costs {_prices.Describe(fuelType)}.");
    }

    // The new wind becomes the default for the next flights and voyages.
    private void ChangeWind()
    {
        double speed = InputReader.ReadDouble($"New average wind speed (km/h, 0-{Wind.MaxSpeed:0}): ",
                                              0, Wind.MaxSpeed);
        _wind.SetSpeed(speed);
        ConsolePrinter.Success($"Wind is now {_wind.Describe()}.");
    }

    // Returns null if the fleet is empty or the user cancels.
    private Vehicle? SelectVehicle()
    {
        if (_fleet.Count == 0)
        {
            Console.WriteLine("The fleet is empty - add a vehicle first.");
            return null;
        }
        var lines = _fleet.Vehicles.Select(vehicle => vehicle.ToString()).ToList();
        int index = InputReader.Choose("Select a vehicle:", lines);
        return index < 0 ? null : _fleet.Vehicles[index];
    }
}
