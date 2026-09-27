using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Ground;

// Automobile: a ground vehicle with a steering wheel - a car or a truck. A motorcycle has
// handlebars, so it inherits GroundVehicle directly. The side of the wheel is only an
// attribute: it is set when the vehicle is created and shown in the details.
public abstract class Automobile : GroundVehicle
{
    public SteeringSide SteeringSide { get; }

    protected Automobile(string registrationNumber, string brand, string model,
                         double fuelLevel, double tankCapacity, double mileage,
                         double fuelConsumptionRate, FuelType fuelType, int developYear,
                         SteeringSide steeringSide, DateOnly inspectionValidUntil)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear, inspectionValidUntil)
    {
        if (!Enum.IsDefined(steeringSide))
            throw new ArgumentException("Unknown steering side.");
        SteeringSide = steeringSide;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Steering wheel: {SteeringSide.ToString().ToLowerInvariant()}";
    }
}
