namespace OOPLAB1.Vehicles.Air;

// Airplane: flies like every aircraft (the algorithm is in AirVehicle) and, unlike other
// aircraft, can also drive on the ground on its wheels (taxiing).
// Kinds: PassengerAirplane, CargoAirplane, LightAirplane.
public abstract class Airplane : AirVehicle
{
    protected Airplane(string registrationNumber, string brand, string model,
                       double fuelLevel, double tankCapacity, double mileage,
                       double fuelConsumptionRate, FuelType fuelType, int developYear,
                       double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override bool CanDrive => true;
}
