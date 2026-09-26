using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Water.Boats;

// Sailboat with an engine. The sails add half of the average wind speed of the voyage to
// the engine speed. The engine still works as usual, so a faster boat spends less fuel
// per km (see WaterVehicle.FuelConsumption).
public class Sailboat : WaterVehicle
{
    public const double SailFactor = 0.5;

    public double WindSpeed { get; private set; }   // km/h, average wind for the next voyage

    public Sailboat(string registrationNumber, string brand, string model,
                    double fuelLevel, double tankCapacity, double mileage,
                    double fuelConsumptionRate, FuelType fuelType, int developYear,
                    double engineSpeed, double draft)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear, engineSpeed, draft)
    {
    }

    public override string TypeName => "Sailboat";

    // Sets the average wind for the next voyage (0 = calm, only the engine works).
    public void SetWind(double windSpeed)
    {
        EnsureInRange(windSpeed, 0, Wind.MaxSpeed, "Wind speed");
        WindSpeed = windSpeed;
    }

    public override double SpeedThroughWater => CruiseSpeed + SailFactor * WindSpeed;

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Wind for the sails: {WindSpeed:F0} km/h (+{SailFactor * WindSpeed:F1} km/h)";
    }
}
