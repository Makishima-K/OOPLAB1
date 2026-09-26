using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Water.Boats;

// Motor boat: the plain water vehicle - everything comes from WaterVehicle.
public class MotorBoat : WaterVehicle
{
    public MotorBoat(string registrationNumber, string brand, string model,
                     double fuelLevel, double tankCapacity, double mileage,
                     double fuelConsumptionRate, FuelType fuelType, int developYear,
                     double cruiseSpeed, double draft)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear, cruiseSpeed, draft)
    {
    }

    public override string TypeName => "Motor boat";
}
