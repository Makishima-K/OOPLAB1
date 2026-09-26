using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Air.Balloons;

// Hot-air balloon. It burns gas (propane) to heat the air and climb. It has no engine and
// flies with the wind of the flight, so its ground speed is the wind speed.
// The burner uses fuel by time; FuelConsumptionRate is given for a typical wind
// (CruiseSpeed) - with a stronger wind the balloon flies faster and uses less fuel per km.
// Like other aircraft it cannot drive, and it needs no fuel reserve: even without fuel
// the balloon descends safely.
public class Balloon : AirVehicle
{
    public Balloon(string registrationNumber, string brand, string model,
                   double fuelLevel, double tankCapacity, double mileage,
                   double fuelConsumptionRate, int developYear,
                   double maxAltitude, double climbRate, double descentRate, double typicalWind)
        : this(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, FuelType.Gas, developYear,
               maxAltitude, climbRate, descentRate, typicalWind)
    {
    }

    // For Airship: an airship is a balloon with engines, which run on another fuel.
    protected Balloon(string registrationNumber, string brand, string model,
                      double fuelLevel, double tankCapacity, double mileage,
                      double fuelConsumptionRate, FuelType fuelType, int developYear,
                      double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override string TypeName => "Balloon";

    public override double FuelReserve => 0;

    // No engine: the balloon moves exactly with the wind.
    public override double GroundSpeed(double wind) => wind;

    protected override string SpeedInfo => $"Typical wind: {CruiseSpeed:F0} km/h (fuel rate is given for it)";
}
