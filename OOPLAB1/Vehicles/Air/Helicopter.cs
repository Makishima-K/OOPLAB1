namespace OOPLAB1.Vehicles.Air;

// Helicopter. It climbs along a slope like an airplane, but much steeper: the pilot chooses
// the climb angle for every flight, up to MaxClimbAngle. It lands vertically, because
// landing on a slope is not safe - the right triangle of the flight becomes a vertical line.
// Like other aircraft it cannot drive.
public class Helicopter : AirVehicle
{
    public const double MinClimbAngle = 1;   // degrees
    public const double VerticalAngle = 90;  // degrees

    public double MaxClimbAngle { get; }             // degrees
    public double ClimbAngle { get; private set; }   // degrees, used in the next flight

    public Helicopter(string registrationNumber, string brand, string model,
                      double fuelLevel, double tankCapacity, double mileage,
                      double fuelConsumptionRate, FuelType fuelType, int developYear,
                      double maxAltitude, double climbRate, double descentRate, double cruiseSpeed,
                      double maxClimbAngle)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
        EnsureInRange(maxClimbAngle, MinClimbAngle, VerticalAngle, "Maximum climb angle");
        MaxClimbAngle = maxClimbAngle;
        ClimbAngle = maxClimbAngle;
    }

    public override string TypeName => "Helicopter";

    // Chooses the climb angle for the next flight.
    public void SetClimbAngle(double angle)
    {
        EnsureInRange(angle, MinClimbAngle, MaxClimbAngle, "Climb angle");
        ClimbAngle = angle;
    }

    // Horizontal leg of the climb: the steeper the angle, the shorter it is.
    protected override double ClimbDistance(double altitude)
    {
        double radians = ClimbAngle * Math.PI / 180;
        return altitude / 1000 / Math.Tan(radians);
    }

    // Vertical landing: the descent has no horizontal part.
    protected override double DescentDistance(double altitude) => 0;

    public override string GetInfo()
    {
        return base.GetInfo() +
               $"\n  Climb angle: {ClimbAngle:0.#} deg (max {MaxClimbAngle:0.#} deg)" +
               "\n  Landing: vertical";
    }
}
