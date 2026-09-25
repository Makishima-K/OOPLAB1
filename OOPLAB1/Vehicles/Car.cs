namespace OOPLAB1.Vehicles;

// Passenger car. Every passenger adds 2 % to the fuel consumption.
public class Car : Vehicle
{
    public const int MinDoors = 2;
    public const int MaxDoors = 5;
    public const int MinSeats = 2;   // seats include the driver's seat
    public const int MaxSeats = 9;
    public const double PassengerConsumptionIncrease = 0.02;

    public int NumberOfDoors { get; }
    public int Seats { get; }

    public Car(string registrationNumber, string brand, string model,
               double fuelLevel, double tankCapacity, double mileage,
               double fuelConsumptionRate, FuelType fuelType, int developYear,
               int numberOfDoors, int seats)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, EnsureNotElectric(fuelType), developYear)
    {
        NumberOfDoors = ValidateDoors(numberOfDoors);
        Seats = ValidateSeats(seats);
    }

    // For ElectricCar: it has a battery instead of a fuel tank, so the tank is 0 L of 0 L.
    protected Car(string registrationNumber, string brand, string model, double mileage,
                  double energyConsumptionRate, int developYear, int numberOfDoors, int seats)
        : base(registrationNumber, brand, model, 0, 0, mileage,
               energyConsumptionRate, FuelType.Electric, developYear)
    {
        NumberOfDoors = ValidateDoors(numberOfDoors);
        Seats = ValidateSeats(seats);
    }

    public override string TypeName => "Car";

    public override int MaxPassengers => Seats - 1;

    protected override double ConsumptionIncreasePerPassenger => PassengerConsumptionIncrease;

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Doors: {NumberOfDoors}, seats: {Seats}";
    }

    private static int ValidateDoors(int doors)
    {
        if (doors < MinDoors || doors > MaxDoors)
            throw new ArgumentException($"Number of doors must be from {MinDoors} to {MaxDoors}.");
        return doors;
    }

    private static int ValidateSeats(int seats)
    {
        if (seats < MinSeats || seats > MaxSeats)
            throw new ArgumentException($"Number of seats must be from {MinSeats} to {MaxSeats}.");
        return seats;
    }
}
