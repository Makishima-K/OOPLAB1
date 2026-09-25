using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;

namespace OOPLAB1.UI;

// Console dialogs that create vehicles from the user's answers.
public static class VehicleCreator
{
    // Types offered in "Add a vehicle". A new type = one line here + one Create... method.
    private static readonly (string Name, Func<string, Vehicle> Create)[] Types =
    {
        ("Car", CreateCar),
        ("Electric car", CreateElectricCar),
        ("Truck", CreateTruck),
        ("Motorcycle", CreateMotorcycle),
        ("Airplane", CreateAirplane),
        // TODO: ("Helicopter", CreateHelicopter) - see Vehicles/Air/Helicopter.cs
    };

    private static readonly FuelType[] CombustionFuels = { FuelType.Petrol, FuelType.Diesel, FuelType.Gas };
    private static readonly FuelType[] AviationFuels = { FuelType.Petrol, FuelType.Kerosene };

    // Asks for the type and all data. Returns null if the user cancels.
    public static Vehicle? Create(Fleet fleet)
    {
        int type = InputReader.Choose("Vehicle type:", Types.Select(t => t.Name).ToList());
        if (type < 0)
            return null;

        string registrationNumber = ReadRegistrationNumber(fleet);
        return Types[type].Create(registrationNumber);
    }

    private static Vehicle CreateCar(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank();
        var (doors, seats) = ReadDoorsAndSeats();
        return new Car(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                       info.Mileage, tank.ConsumptionRate, fuelType, info.Year, doors, seats);
    }

    private static Vehicle CreateElectricCar(string registrationNumber)
    {
        var info = ReadCommonInfo();
        double capacity = InputReader.ReadDouble("Battery capacity (kWh): ", 1, 300);
        double charge = InputReader.ReadDouble($"Battery charge (kWh, 0-{capacity:0.#}): ", 0, capacity);
        double rate = InputReader.ReadDouble("Energy consumption (kWh/100 km): ", 1, 100);
        var (doors, seats) = ReadDoorsAndSeats();
        return new ElectricCar(registrationNumber, info.Brand, info.Model, charge, capacity,
                               info.Mileage, rate, info.Year, doors, seats);
    }

    private static Vehicle CreateTruck(string registrationNumber)
    {
        var info = ReadCommonInfo();
        Console.WriteLine("Fuel type: Diesel");
        var tank = ReadFuelTank();
        double cargoCapacity = InputReader.ReadDouble("Cargo capacity (kg): ", 100, 40000);
        return new Truck(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                         info.Mileage, tank.ConsumptionRate, info.Year, cargoCapacity);
    }

    private static Vehicle CreateMotorcycle(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank();
        bool hasSidecar = InputReader.ReadYesNo("Sidecar attached?");
        return new Motorcycle(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                              info.Mileage, tank.ConsumptionRate, fuelType, info.Year, hasSidecar);
    }

    private static Vehicle CreateAirplane(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(maxCapacity: 400_000, maxConsumptionRate: 2000);
        double maxAltitude = InputReader.ReadDouble("Maximum altitude (m): ", AirVehicle.MinAltitude, 20_000);
        double climbRate = InputReader.ReadDouble("Climb rate (m/s): ", 0.5, 300);
        double descentRate = InputReader.ReadDouble("Descent rate (m/s): ", 0.5, 300);
        double cruiseSpeed = InputReader.ReadDouble("Cruise speed (km/h): ", 50, 3500);
        return new Airplane(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                            info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
                            maxAltitude, climbRate, descentRate, cruiseSpeed);
    }

    // Asks until the number has a valid format and is not used in the fleet yet.
    private static string ReadRegistrationNumber(Fleet fleet)
    {
        while (true)
        {
            string input = InputReader.ReadLine("Registration number (e.g. AB-1234): ");
            try
            {
                string number = Vehicle.NormalizeRegistrationNumber(input);
                if (!fleet.Contains(number))
                    return number;
                ConsolePrinter.Warning($"{number} is already in the fleet.");
            }
            catch (ArgumentException e)
            {
                ConsolePrinter.Warning(e.Message);
            }
        }
    }

    private static (string Brand, string Model, int Year, double Mileage) ReadCommonInfo()
    {
        string brand = InputReader.ReadText("Brand: ");
        string model = InputReader.ReadText("Model: ");
        int year = InputReader.ReadInt($"Year of manufacture ({Vehicle.MinDevelopYear}-{DateTime.Now.Year}): ",
                                       Vehicle.MinDevelopYear, DateTime.Now.Year);
        double mileage = InputReader.ReadDouble("Mileage (km): ", 0, 10_000_000);
        return (brand, model, year, mileage);
    }

    private static FuelType ReadFuelType(FuelType[] options)
    {
        var names = options.Select(fuel => fuel.ToString()).ToList();
        return options[InputReader.Choose("Fuel type:", names, allowCancel: false)];
    }

    // The limits are only against typing mistakes; aircraft get bigger ones.
    private static (double Capacity, double FuelLevel, double ConsumptionRate) ReadFuelTank(
        double maxCapacity = 2000, double maxConsumptionRate = 150)
    {
        double capacity = InputReader.ReadDouble("Tank capacity (L): ", 1, maxCapacity);
        double fuelLevel = InputReader.ReadDouble($"Fuel level (L, 0-{capacity:0.#}): ", 0, capacity);
        double rate = InputReader.ReadDouble("Fuel consumption (L/100 km): ", 0.5, maxConsumptionRate);
        return (capacity, fuelLevel, rate);
    }

    private static (int Doors, int Seats) ReadDoorsAndSeats()
    {
        int doors = InputReader.ReadInt($"Number of doors ({Car.MinDoors}-{Car.MaxDoors}): ",
                                        Car.MinDoors, Car.MaxDoors);
        int seats = InputReader.ReadInt($"Seats incl. the driver ({Car.MinSeats}-{Car.MaxSeats}): ",
                                        Car.MinSeats, Car.MaxSeats);
        return (doors, seats);
    }
}
