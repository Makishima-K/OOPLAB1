namespace OOPLAB1.Vehicles.Air;

// Airplane: climbs and descends along slopes and keeps one altitude between them
// (the flight algorithm is in AirVehicle).
public class Airplane : AirVehicle
{
    public Airplane(string registrationNumber, string brand, string model,
                    double fuelLevel, double tankCapacity, double mileage,
                    double fuelConsumptionRate, FuelType fuelType, int developYear,
                    double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override string TypeName => "Airplane";
}
