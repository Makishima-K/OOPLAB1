namespace OOPLAB1.Vehicles.Air;

// Base class of aircraft. A flight is a trapezoid: two right triangles and a line between them.
//
//             ______________________________
//            /|           cruise           |\
//    climb  / | altitude                   | \  descent
//   _______/__|____________________________|__\_______
//          |<------------- distance -------------->|
//
// Left triangle: the aircraft rises with ClimbRate (m/s) and at the same time flies forward
// with CruiseSpeed, so its horizontal leg is CruiseSpeed * climb time. Then it keeps one
// altitude. Right triangle: the same with DescentRate.
// Fuel: climb x2, cruise x1, descent x0.5 of the normal consumption, plus a 10 % reserve.
// Without enough fuel the flight is cancelled.
// Drive() from Vehicle means taxiing on the ground.
public abstract class AirVehicle : Vehicle
{
    public const double MinAltitude = 100;       // m, lower is not a flight
    public const double ClimbFuelFactor = 2;     // engines work at full power
    public const double DescentFuelFactor = 0.5;
    public const double FuelReserve = 0.1;       // +10 % for safety

    public double MaxAltitude { get; }   // m
    public double ClimbRate { get; }     // m/s
    public double DescentRate { get; }   // m/s
    public double CruiseSpeed { get; }   // km/h

    protected AirVehicle(string registrationNumber, string brand, string model,
                         double fuelLevel, double tankCapacity, double mileage,
                         double fuelConsumptionRate, FuelType fuelType, int developYear,
                         double maxAltitude, double climbRate, double descentRate,
                         double cruiseSpeed)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, EnsureNotElectric(fuelType), developYear)
    {
        if (!double.IsFinite(maxAltitude) || maxAltitude < MinAltitude)
            throw new ArgumentException($"Maximum altitude must be at least {MinAltitude} m.");
        EnsurePositive(climbRate, "Climb rate");
        EnsurePositive(descentRate, "Descent rate");
        EnsurePositive(cruiseSpeed, "Cruise speed");

        MaxAltitude = maxAltitude;
        ClimbRate = climbRate;
        DescentRate = descentRate;
        CruiseSpeed = cruiseSpeed;
    }

    // Calculates the flight without making it: the three parts, the time and the fuel.
    public FlightPlan PlanFlight(double distance, double altitude)
    {
        EnsurePositive(distance, "Flight distance");
        EnsureInRange(altitude, MinAltitude, MaxAltitude, "Altitude");

        double climbDistance = ClimbDistance(altitude);
        double descentDistance = DescentDistance(altitude);
        double cruiseDistance = distance - climbDistance - descentDistance;
        if (cruiseDistance < -Tolerance)
            throw new VehicleException(
                $"{distance:0.#} km is too short to climb to {altitude:F0} m and descend again " +
                $"(at most {MaxAltitudeFor(distance):F0} m).");
        cruiseDistance = Math.Max(0, cruiseDistance);

        // The aircraft really flies along the hypotenuses of the triangles.
        double altitudeKm = altitude / 1000;
        double climbPath = Math.Sqrt(climbDistance * climbDistance + altitudeKm * altitudeKm);
        double descentPath = Math.Sqrt(descentDistance * descentDistance + altitudeKm * altitudeKm);

        double fuel = FuelConsumption(climbPath) * ClimbFuelFactor
                    + FuelConsumption(cruiseDistance)
                    + FuelConsumption(descentPath) * DescentFuelFactor;
        double hours = altitude / ClimbRate / 3600
                     + cruiseDistance / CruiseSpeed
                     + altitude / DescentRate / 3600;

        return new FlightPlan(distance, altitude, climbDistance, cruiseDistance, descentDistance,
                              climbPath + cruiseDistance + descentPath, hours,
                              fuel, fuel * (1 + FuelReserve));
    }

    // The highest altitude for this distance: the aircraft must have time to descend.
    public double MaxAltitudeFor(double distance)
    {
        EnsurePositive(distance, "Flight distance");
        double kmPerMetre = ClimbDistance(1) + DescentDistance(1);   // both triangles grow with the altitude
        return kmPerMetre > 0 ? Math.Min(MaxAltitude, distance / kmPerMetre) : MaxAltitude;
    }

    public bool HasEnoughFuel(FlightPlan plan) => plan.FuelRequired <= FuelLevel + Tolerance;

    // Makes the flight, or cancels it if the fuel (with the reserve) is not enough.
    public FlightPlan Fly(double distance, double altitude)
    {
        FlightPlan plan = PlanFlight(distance, altitude);
        if (!HasEnoughFuel(plan))
            throw new VehicleException(
                $"Flight cancelled: {plan.FuelRequired:F1} L needed " +
                $"(with {FuelReserve * 100:0}% reserve), only {FuelLevel:F1} L in the tank.");

        BurnFuel(plan.FuelBurned);
        AddMileage(plan.PathLength);
        return plan;
    }

    // Horizontal leg of the left triangle, km: how far the aircraft flies forward while climbing.
    protected virtual double ClimbDistance(double altitude)
    {
        double climbSeconds = altitude / ClimbRate;
        return CruiseSpeed * climbSeconds / 3600;
    }

    // Horizontal leg of the right triangle, km.
    protected virtual double DescentDistance(double altitude)
    {
        double descentSeconds = altitude / DescentRate;
        return CruiseSpeed * descentSeconds / 3600;
    }

    public override string GetInfo()
    {
        return base.GetInfo() +
               $"\n  Max altitude: {MaxAltitude:F0} m" +
               $"\n  Climb / descent rate: {ClimbRate:0.#} / {DescentRate:0.#} m/s" +
               $"\n  Cruise speed: {CruiseSpeed:F0} km/h";
    }
}
