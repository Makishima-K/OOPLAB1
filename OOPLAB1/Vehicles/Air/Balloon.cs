namespace OOPLAB1.Vehicles.Air;

// Hot-air balloon. It burns gas (propane) to heat the air and climb. It has no engine and
// flies with the wind, so its CruiseSpeed is the wind speed. Like other aircraft it cannot
// drive, and it needs no fuel reserve: even without fuel the balloon descends safely.
public class Balloon : AirVehicle
{
    public Balloon(string registrationNumber, string brand, string model,
                   double fuelLevel, double tankCapacity, double mileage,
                   double fuelConsumptionRate, int developYear,
                   double maxAltitude, double climbRate, double descentRate, double windSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, FuelType.Gas, developYear,
               maxAltitude, climbRate, descentRate, windSpeed)
    {
    }

    public override string TypeName => "Balloon";

    public override double FuelReserve => 0;

    protected override string SpeedInfo => $"Wind speed: {CruiseSpeed:F0} km/h";
}
