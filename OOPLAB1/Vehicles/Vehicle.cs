namespace OOPLAB1.Vehicles;

// Base class of all vehicles (variant 6).
// Units: fuel in litres, distance and mileage in km, consumption rate in L/100 km.
// An electric car keeps its energy in a battery (kWh) - see ElectricCar.
public abstract class Vehicle
{
    public const int MinDevelopYear = 1886;   // the first car
    protected const double Tolerance = 1e-9;  // doubles are not exact: 24.1 may be stored as 24.0999...

    public string RegistrationNumber { get; }
    public string Brand { get; }
    public string Model { get; }
    public double FuelLevel { get; private set; }
    public double TankCapacity { get; }
    public double Mileage { get; private set; }
    public double FuelConsumptionRate { get; }
    public FuelType FuelType { get; }
    public int DevelopYear { get; }
    public int Passengers { get; private set; }

    protected Vehicle(string registrationNumber, string brand, string model,
                      double fuelLevel, double tankCapacity, double mileage,
                      double fuelConsumptionRate, FuelType fuelType, int developYear)
    {
        RegistrationNumber = NormalizeRegistrationNumber(registrationNumber);
        Brand = RequireText(brand, "Brand");
        Model = RequireText(model, "Model");

        if (!Enum.IsDefined(fuelType))
            throw new ArgumentException("Unknown fuel type.");
        if (fuelType == FuelType.Electric)
        {
            if (tankCapacity != 0 || fuelLevel != 0)
                throw new ArgumentException("An electric vehicle has no fuel tank.");
        }
        else
        {
            EnsurePositive(tankCapacity, "Tank capacity");
            EnsureInRange(fuelLevel, 0, tankCapacity, "Fuel level");
        }
        EnsureNotNegative(mileage, "Mileage");
        EnsurePositive(fuelConsumptionRate, "Fuel consumption rate");
        if (developYear < MinDevelopYear || developYear > DateTime.Now.Year)
            throw new ArgumentException(
                $"Year must be from {MinDevelopYear} to {DateTime.Now.Year}.");

        FuelLevel = fuelLevel;
        TankCapacity = tankCapacity;
        Mileage = mileage;
        FuelConsumptionRate = fuelConsumptionRate;
        FuelType = fuelType;
        DevelopYear = developYear;
    }

    // "Car", "Truck", ... - every vehicle type names itself.
    public abstract string TypeName { get; }

    // Older vehicles pay a higher tax.
    public double Tax => DevelopYear switch
    {
        < 2000 => 30,
        < 2010 => 20,
        < 2020 => 10,
        _ => 0
    };

    public double FreeTankCapacity => TankCapacity - FuelLevel;

    // Unit of the fuel: litres (an electric car uses kWh).
    public virtual string FuelUnit => "L";

    // Short text about the fuel for lists, e.g. "Fuel: 32.0 / 50.0 L".
    public virtual string EnergyStatus => $"Fuel: {FuelLevel:F1} / {TankCapacity:F1} L";

    // How many km the vehicle can drive with the current fuel and load.
    public virtual double Range => FuelLevel / FuelConsumption(100) * 100;

    // Passenger seats besides the driver. 0 = the vehicle does not carry passengers.
    public virtual int MaxPassengers => 0;

    public int FreeSeats => MaxPassengers - Passengers;

    // Extra consumption for every passenger: 0.02 = +2 %.
    protected virtual double ConsumptionIncreasePerPassenger => 0;

    // Fuel needed for the distance with the current load.
    // Subclasses add their own load (cargo, sidecar) on top of it.
    public virtual double FuelConsumption(double distance)
    {
        double passengersFactor = 1 + Passengers * ConsumptionIncreasePerPassenger;
        return distance / 100 * FuelConsumptionRate * passengersFactor;
    }

    public virtual void Drive(double distance)
    {
        EnsurePositive(distance, "Distance");
        double fuelNeeded = FuelConsumption(distance);
        if (fuelNeeded > FuelLevel + Tolerance)
            throw new VehicleException(
                $"Not enough fuel: {fuelNeeded:F1} L needed, {FuelLevel:F1} L in the tank " +
                $"(enough for {Range:F0} km).");

        BurnFuel(fuelNeeded);
        AddMileage(distance);
    }

    // Adds fuel to the tank and returns how much it cost.
    public virtual double Refuel(double amount, double pricePerUnit)
    {
        EnsurePositive(amount, "Amount of fuel");
        EnsureNotNegative(pricePerUnit, "Price");
        if (amount > FreeTankCapacity + Tolerance)
            throw new VehicleException($"Only {FreeTankCapacity:F1} L fits into the tank.");

        FuelLevel = Math.Min(TankCapacity, FuelLevel + amount);
        return amount * pricePerUnit;
    }

    public void BoardPassengers(int count)
    {
        EnsurePositive(count, "Number of passengers");
        if (MaxPassengers == 0)
            throw new VehicleException($"{TypeName} {RegistrationNumber} has no passenger seats.");
        if (count > FreeSeats)
            throw new VehicleException($"Only {FreeSeats} free seat(s).");

        Passengers += count;
    }

    public void DropOffPassengers(int count)
    {
        EnsurePositive(count, "Number of passengers");
        if (count > Passengers)
            throw new VehicleException($"Only {Passengers} passenger(s) on board.");

        Passengers -= count;
    }

    // Full description; subclasses append their own lines.
    public virtual string GetInfo()
    {
        string info =
            $"{TypeName} {RegistrationNumber} - {Brand} {Model} ({DevelopYear})\n" +
            $"  Fuel type: {FuelType}\n" +
            $"  {EnergyStatus}\n" +
            $"  Consumption: {FuelConsumption(100):F2} {FuelUnit}/100 km " +
            $"(base {FuelConsumptionRate:F2})\n" +
            $"  Range: {Range:F0} km\n" +
            $"  Mileage: {Mileage:F0} km\n" +
            $"  Tax: {Tax}";
        if (MaxPassengers > 0)
            info += $"\n  Passengers: {Passengers} / {MaxPassengers}";
        return info;
    }

    // One line for lists.
    public override string ToString()
    {
        string name = $"{Brand} {Model}";
        return $"{RegistrationNumber,-10} {TypeName,-12} {name,-22} {EnergyStatus,-31} {Mileage,8:F0} km";
    }

    // " ab-1234 " -> "AB-1234"
    public static string NormalizeRegistrationNumber(string registrationNumber)
    {
        string value = (registrationNumber ?? "").Trim().ToUpperInvariant();
        bool validChars = value.All(c => char.IsLetterOrDigit(c) || c == '-');
        if (value.Length < 2 || value.Length > 10 || !validChars)
            throw new ArgumentException(
                "Registration number must have 2-10 letters, digits or '-', e.g. AB-1234.");
        return value;
    }

    protected void AddMileage(double distance)
    {
        Mileage += distance;
    }

    // Takes fuel from the tank; the caller has already checked that there is enough.
    protected void BurnFuel(double amount)
    {
        FuelLevel = Math.Max(0, FuelLevel - amount);
    }

    // Vehicles with a fuel tank cannot be electric - electric cars are created as ElectricCar.
    protected static FuelType EnsureNotElectric(FuelType fuelType)
    {
        if (fuelType == FuelType.Electric)
            throw new ArgumentException("An electric vehicle needs a battery - only ElectricCar has one.");
        return fuelType;
    }

    // The checks are written so that NaN and infinity are rejected too.
    protected static void EnsurePositive(double value, string what)
    {
        if (!double.IsFinite(value) || value <= 0)
            throw new ArgumentException($"{what} must be greater than zero.");
    }

    protected static void EnsureNotNegative(double value, string what)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentException($"{what} must be zero or greater.");
    }

    protected static void EnsureInRange(double value, double min, double max, string what)
    {
        if (!double.IsFinite(value) || value < min || value > max)
            throw new ArgumentException($"{what} must be from {min:0.##} to {max:0.##}.");
    }

    private static string RequireText(string value, string what)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{what} cannot be empty.");
        return value.Trim();
    }
}
