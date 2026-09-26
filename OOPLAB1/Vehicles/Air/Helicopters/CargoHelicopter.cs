using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Air.Helicopters;

// Cargo helicopter: carries cargo inside and can hang a load on a sling under the body.
// Every 100 kg of cargo, inside and on the sling, adds 1 % to the consumption. With a load
// on the sling it may climb at most 15 degrees, otherwise the load starts to swing.
public class CargoHelicopter : Helicopter
{
    public const double ConsumptionIncreasePer100Kg = 0.01;
    public const double SlingClimbAngleLimit = 15;   // degrees

    public double CargoCapacity { get; }              // kg, inside
    public double MaxSlingLoad { get; }               // kg
    public double SlingLoad { get; private set; }     // kg, 0 = nothing on the sling

    public CargoHelicopter(string registrationNumber, string brand, string model,
                           double fuelLevel, double tankCapacity, double mileage,
                           double fuelConsumptionRate, FuelType fuelType, int developYear,
                           double maxAltitude, double climbRate, double descentRate, double cruiseSpeed,
                           double maxClimbAngle, double cargoCapacity, double maxSlingLoad)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed, maxClimbAngle)
    {
        EnsurePositive(cargoCapacity, "Cargo capacity");
        EnsurePositive(maxSlingLoad, "Maximum sling load");
        CargoCapacity = cargoCapacity;
        MaxSlingLoad = maxSlingLoad;
    }

    public override string TypeName => "Cargo helicopter";

    public override double MaxCargo => CargoCapacity;

    public bool HasSlingLoad => SlingLoad > 0;

    public override double AllowedClimbAngle =>
        HasSlingLoad ? Math.Min(MaxClimbAngle, SlingClimbAngleLimit) : MaxClimbAngle;

    public void AttachSlingLoad(double weight)
    {
        if (HasSlingLoad)
            throw new VehicleException("A load is already on the sling.");
        EnsurePositive(weight, "Sling load");
        if (weight > MaxSlingLoad + Tolerance)
            throw new VehicleException($"The sling holds at most {MaxSlingLoad:F0} kg.");

        SlingLoad = weight;
    }

    public void DetachSlingLoad()
    {
        if (!HasSlingLoad)
            throw new VehicleException("There is no load on the sling.");

        SlingLoad = 0;
    }

    public override double FuelConsumption(double distance)
    {
        double loadFactor = 1 + (CurrentCargo + SlingLoad) / 100 * ConsumptionIncreasePer100Kg;
        return base.FuelConsumption(distance) * loadFactor;
    }

    public override string GetInfo()
    {
        string sling = HasSlingLoad ? $"{SlingLoad:F0} kg (max {MaxSlingLoad:F0} kg)" : $"empty (max {MaxSlingLoad:F0} kg)";
        return base.GetInfo() + $"\n  Sling: {sling}";
    }
}
