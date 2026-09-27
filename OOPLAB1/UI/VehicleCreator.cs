using OOPLAB1.Fuel;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;
using OOPLAB1.Vehicles.Air.Balloons;
using OOPLAB1.Vehicles.Air.Helicopters;
using OOPLAB1.Vehicles.Air.Planes;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;
using OOPLAB1.Vehicles.Water.Boats;
using OOPLAB1.Vehicles.Water.Ships;

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
        ("Passenger airplane", CreatePassengerAirplane),
        ("Cargo airplane", CreateCargoAirplane),
        ("Light airplane", CreateLightAirplane),
        ("Balloon", CreateBalloon),
        ("Airship", CreateAirship),
        ("Helicopter", CreateHelicopter),
        ("Cargo helicopter", CreateCargoHelicopter),
        ("Motor boat", CreateMotorBoat),
        ("Cargo ship", CreateCargoShip),
        ("Passenger liner", CreatePassengerLiner),
        ("Sailboat", CreateSailboat),
    };

    private static readonly FuelType[] CombustionFuels = { FuelType.Petrol, FuelType.Diesel, FuelType.Gas };
    private static readonly FuelType[] AviationFuels = { FuelType.Petrol, FuelType.Kerosene };

    // Airliners and ships have huge tanks and consumption, so their input limits are bigger.
    private const double AircraftMaxTank = 400_000;
    private const double AircraftMaxConsumption = 2000;
    private const double ShipMaxTank = 20_000_000;
    private const double ShipMaxConsumption = 100_000;

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
        SteeringSide steeringSide = ReadSteeringSide();
        DateOnly inspection = ReadInspectionDate(info.Year);
        return new Car(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                       info.Mileage, tank.ConsumptionRate, fuelType, info.Year, doors, seats,
                       steeringSide, inspection);
    }

    private static Vehicle CreateElectricCar(string registrationNumber)
    {
        var info = ReadCommonInfo();
        double capacity = InputReader.ReadDouble("Battery capacity (kWh): ", 1, 300);
        double charge = InputReader.ReadDouble($"Battery charge (kWh, 0-{capacity:0.#}): ", 0, capacity);
        double rate = InputReader.ReadDouble("Energy consumption (kWh/100 km): ", 1, 100);
        var (doors, seats) = ReadDoorsAndSeats();
        SteeringSide steeringSide = ReadSteeringSide();
        DateOnly inspection = ReadInspectionDate(info.Year);
        return new ElectricCar(registrationNumber, info.Brand, info.Model, charge, capacity,
                               info.Mileage, rate, info.Year, doors, seats, steeringSide, inspection);
    }

    private static Vehicle CreateTruck(string registrationNumber)
    {
        var info = ReadCommonInfo();
        Console.WriteLine("Fuel type: Diesel");
        var tank = ReadFuelTank();
        double cargoCapacity = InputReader.ReadDouble("Cargo capacity (kg): ", 100, 40000);
        SteeringSide steeringSide = ReadSteeringSide();
        DateOnly inspection = ReadInspectionDate(info.Year);
        return new Truck(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                         info.Mileage, tank.ConsumptionRate, info.Year, cargoCapacity,
                         steeringSide, inspection);
    }

    private static Vehicle CreateMotorcycle(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank();
        bool hasSidecar = InputReader.ReadYesNo("Sidecar attached?");
        DateOnly inspection = ReadInspectionDate(info.Year);
        return new Motorcycle(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                              info.Mileage, tank.ConsumptionRate, fuelType, info.Year, hasSidecar,
                              inspection);
    }

    private static Vehicle CreatePassengerAirplane(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(AircraftMaxTank, AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 50);
        int seats = InputReader.ReadInt($"Passenger seats (1-{PassengerAirplane.MaxSeats}): ",
                                        1, PassengerAirplane.MaxSeats);
        return new PassengerAirplane(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
            tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
            flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed, seats);
    }

    private static Vehicle CreateCargoAirplane(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(AircraftMaxTank, AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 50);
        double cargoCapacity = InputReader.ReadDouble("Cargo capacity (kg): ", 100, 150_000);
        return new CargoAirplane(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
            tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
            flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed, cargoCapacity);
    }

    private static Vehicle CreateLightAirplane(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(AircraftMaxTank, AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 50);
        return new LightAirplane(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
            tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
            flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed);
    }

    private static Vehicle CreateBalloon(string registrationNumber)
    {
        var info = ReadCommonInfo();
        Console.WriteLine("Fuel type: Gas (propane)");
        var tank = ReadFuelTank(maxCapacity: 1000, maxConsumptionRate: AircraftMaxConsumption);
        var flight = ReadFlightData("Typical wind (km/h, the fuel rate is given for it): ", 1);
        return new Balloon(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                           info.Mileage, tank.ConsumptionRate, info.Year,
                           flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed);
    }

    private static Vehicle CreateAirship(string registrationNumber)
    {
        var info = ReadCommonInfo();
        Console.WriteLine("Fuel type: Petrol (engines)");
        var tank = ReadFuelTank(maxCapacity: 20_000, maxConsumptionRate: AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 10);
        return new Airship(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                           info.Mileage, tank.ConsumptionRate, info.Year,
                           flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed);
    }

    private static Vehicle CreateHelicopter(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(AircraftMaxTank, AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 50);
        double maxClimbAngle = ReadMaxClimbAngle();
        return new Helicopter(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
            tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
            flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed, maxClimbAngle);
    }

    private static Vehicle CreateCargoHelicopter(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(AviationFuels);
        var tank = ReadFuelTank(AircraftMaxTank, AircraftMaxConsumption);
        var flight = ReadFlightData("Cruise speed (km/h): ", 50);
        double maxClimbAngle = ReadMaxClimbAngle();
        double cargoCapacity = InputReader.ReadDouble("Cargo capacity inside (kg): ", 100, 30_000);
        double maxSlingLoad = InputReader.ReadDouble("Maximum load on the sling (kg): ", 100, 30_000);
        return new CargoHelicopter(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
            tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
            flight.MaxAltitude, flight.ClimbRate, flight.DescentRate, flight.Speed,
            maxClimbAngle, cargoCapacity, maxSlingLoad);
    }

    private static Vehicle CreateMotorBoat(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank(ShipMaxTank, ShipMaxConsumption);
        var vessel = ReadVesselData("Cruise speed (km/h): ", "Draft (m): ");
        return new MotorBoat(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                             info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
                             vessel.Speed, vessel.Draft);
    }

    private static Vehicle CreateCargoShip(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank(ShipMaxTank, ShipMaxConsumption);
        var vessel = ReadVesselData("Cruise speed, empty (km/h): ", "Draft when empty (m): ");
        double loadedDraft = InputReader.ReadDouble($"Draft when fully loaded (m, {vessel.Draft:0.#}-30): ",
                                                    vessel.Draft, 30);
        double cargoCapacity = InputReader.ReadDouble("Cargo capacity (kg): ", 1000, 500_000_000);
        return new CargoShip(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                             info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
                             vessel.Speed, vessel.Draft, loadedDraft, cargoCapacity);
    }

    private static Vehicle CreatePassengerLiner(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank(ShipMaxTank, ShipMaxConsumption);
        var vessel = ReadVesselData("Cruise speed (km/h): ", "Draft (m): ");
        int seats = InputReader.ReadInt($"Passenger places (1-{PassengerLiner.MaxSeats}): ",
                                        1, PassengerLiner.MaxSeats);
        return new PassengerLiner(registrationNumber, info.Brand, info.Model, tank.FuelLevel,
                                  tank.Capacity, info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
                                  vessel.Speed, vessel.Draft, seats);
    }

    private static Vehicle CreateSailboat(string registrationNumber)
    {
        var info = ReadCommonInfo();
        FuelType fuelType = ReadFuelType(CombustionFuels);
        var tank = ReadFuelTank();
        var vessel = ReadVesselData("Speed under the engine (km/h): ", "Draft (m): ");
        return new Sailboat(registrationNumber, info.Brand, info.Model, tank.FuelLevel, tank.Capacity,
                            info.Mileage, tank.ConsumptionRate, fuelType, info.Year,
                            vessel.Speed, vessel.Draft);
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

    // Common data of all aircraft. A balloon has no engine: its speed is the wind speed.
    private static (double MaxAltitude, double ClimbRate, double DescentRate, double Speed) ReadFlightData(
        string speedPrompt, double minSpeed)
    {
        double maxAltitude = InputReader.ReadDouble("Maximum altitude (m): ", AirVehicle.MinAltitude, 20_000);
        double climbRate = InputReader.ReadDouble("Climb rate (m/s): ", 0.5, 300);
        double descentRate = InputReader.ReadDouble("Descent rate (m/s): ", 0.5, 300);
        double speed = InputReader.ReadDouble(speedPrompt, minSpeed, 3500);
        return (maxAltitude, climbRate, descentRate, speed);
    }

    private static double ReadMaxClimbAngle()
    {
        return InputReader.ReadDouble(
            $"Maximum climb angle (degrees, {Helicopter.MinClimbAngle}-{Helicopter.VerticalAngle}): ",
            Helicopter.MinClimbAngle, Helicopter.VerticalAngle);
    }

    // Common data of all vessels.
    private static (double Speed, double Draft) ReadVesselData(string speedPrompt, string draftPrompt)
    {
        double speed = InputReader.ReadDouble(speedPrompt, 1, 100);
        double draft = InputReader.ReadDouble(draftPrompt, 0.1, 30);
        return (speed, draft);
    }

    // Cars and trucks only: a motorcycle has handlebars.
    private static SteeringSide ReadSteeringSide()
    {
        SteeringSide[] sides = Enum.GetValues<SteeringSide>();
        var names = sides.Select(side => side.ToString()).ToList();
        return sides[InputReader.Choose("Steering wheel:", names, allowCancel: false)];
    }

    // The inspection may already be expired; a new vehicle has its first one up to 4 years ahead.
    private static DateOnly ReadInspectionDate(int year)
    {
        DateOnly latest = DateOnly.FromDateTime(DateTime.Today).AddYears(GroundVehicle.MaxInspectionYears);
        return InputReader.ReadDate("Technical inspection valid until (yyyy-mm-dd): ",
                                    new DateOnly(year, 1, 1), latest);
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
