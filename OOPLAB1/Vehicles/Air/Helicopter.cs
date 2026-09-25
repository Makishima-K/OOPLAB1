namespace OOPLAB1.Vehicles.Air;

// SKETCH: helicopter - not finished and not in the menu yet.
// Now it flies like an airplane (the algorithm in AirVehicle), but a helicopter takes off
// vertically: the left triangle turns into a vertical line.
// TODO:
//  * override ClimbDistance(altitude) => 0 (vertical take-off);
//  * Hover(minutes) - stays in one place and burns fuel;
//  * add ("Helicopter", CreateHelicopter) to UI/VehicleCreator.
public class Helicopter : AirVehicle
{
    public Helicopter(string registrationNumber, string brand, string model,
                      double fuelLevel, double tankCapacity, double mileage,
                      double fuelConsumptionRate, FuelType fuelType, int developYear,
                      double maxAltitude, double climbRate, double descentRate, double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
    }

    public override string TypeName => "Helicopter";
}
