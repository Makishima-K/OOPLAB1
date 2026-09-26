namespace OOPLAB1.Vehicles.Air;

// Cargo airplane: heavy cargo makes the flight itself use more fuel, +1 % for every tonne.
// It is not a bigger reserve - the reserve stays the normal 10 %.
public class CargoAirplane : Airplane
{
    public const double ConsumptionIncreasePerTonne = 0.01;

    public double CargoCapacity { get; }   // kg

    public CargoAirplane(string registrationNumber, string brand, string model,
                         double fuelLevel, double tankCapacity, double mileage,
                         double fuelConsumptionRate, FuelType fuelType, int developYear,
                         double maxAltitude, double climbRate, double descentRate,
                         double cruiseSpeed, double cargoCapacity)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
        EnsurePositive(cargoCapacity, "Cargo capacity");
        CargoCapacity = cargoCapacity;
    }

    public override string TypeName => "Cargo plane";

    public override double MaxCargo => CargoCapacity;

    public override double FuelConsumption(double distance)
    {
        double cargoFactor = 1 + CurrentCargo / 1000 * ConsumptionIncreasePerTonne;
        return base.FuelConsumption(distance) * cargoFactor;
    }
}
