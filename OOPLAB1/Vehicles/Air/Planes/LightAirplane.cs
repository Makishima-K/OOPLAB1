using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Air.Planes;

// Light private airplane (like a Cessna 172). It has no pressurized cabin, so it may not
// fly higher than 3000 m, even if it could technically climb higher (MaxAltitude).
public class LightAirplane : Airplane
{
    public const double UnpressurizedCeiling = 3000;   // m

    public LightAirplane(string registrationNumber, string brand, string model,
                         double fuelLevel, double tankCapacity, double mileage,
                         double fuelConsumptionRate, FuelType fuelType, int developYear,
                         double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override string TypeName => "Light plane";

    public override double AltitudeLimit => Math.Min(MaxAltitude, UnpressurizedCeiling);
}
