using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Air.Balloons;

// Airship: a balloon with engines. Like a balloon it is lighter than air, so it needs no
// fuel reserve and cannot drive. Unlike a balloon it has its own speed: the wind only adds
// to it or takes from it. The engines run on petrol.
public class Airship : Balloon
{
    public Airship(string registrationNumber, string brand, string model,
                   double fuelLevel, double tankCapacity, double mileage,
                   double fuelConsumptionRate, int developYear,
                   double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, FuelType.Petrol, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override string TypeName => "Airship";

    // Own speed of the engines plus the wind.
    public override double GroundSpeed(double wind) => CruiseSpeed + wind;

    protected override string SpeedInfo => $"Cruise speed (engines): {CruiseSpeed:F0} km/h";
}
