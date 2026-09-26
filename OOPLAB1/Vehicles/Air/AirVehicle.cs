using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Air;

// Base class of aircraft. A flight is a trapezoid: two right triangles and a line between them.
//
//             ______________________________
//            /|           cruise           |\
//    climb  / | altitude                   | \  descent
//   _______/__|____________________________|__\_______
//          |<------------- distance -------------->|
//
// Left triangle: the aircraft rises with ClimbRate (m/s) and at the same time moves forward
// with its ground speed, so its horizontal leg is ground speed * climb time. Then it keeps one
// altitude. Right triangle: the same with DescentRate.
// Wind: ground speed = CruiseSpeed + average wind of the route (+ tailwind, - headwind).
// Engines burn fuel by time, so a headwind makes every km more expensive, a tailwind cheaper.
// Fuel: climb x2, cruise x1, descent x0.5 of the normal consumption, plus a reserve
// (10 %, subclasses may change it). Without enough fuel the flight is cancelled.
// Aircraft cannot drive on the ground - only an Airplane can taxi.
public abstract class AirVehicle : Vehicle
{
    public const double MinAltitude = 100;         // m, lower is not a flight
    public const double ClimbFuelFactor = 2;       // engines work at full power
    public const double DescentFuelFactor = 0.5;
    public const double DefaultFuelReserve = 0.1;  // +10 % for safety

    public double MaxAltitude { get; }   // m, what the aircraft can technically reach
    public double ClimbRate { get; }     // m/s
    public double DescentRate { get; }   // m/s
    public double CruiseSpeed { get; }   // km/h, speed through the air

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

    public override bool CanDrive => false;

    // Extra fuel that must be in the tank before the flight: 0.1 = +10 %.
    public virtual double FuelReserve => DefaultFuelReserve;

    // The highest allowed altitude; rules can make it lower than MaxAltitude.
    public virtual double AltitudeLimit => MaxAltitude;

    // Speed over the ground with the average wind of the route (+ tailwind, - headwind).
    public virtual double GroundSpeed(double wind) => CruiseSpeed + wind;

    // Calculates the flight without making it: the three parts, the time and the fuel.
    public FlightPlan PlanFlight(double distance, double altitude, double wind = 0)
    {
        EnsurePositive(distance, "Flight distance");
        EnsureInRange(altitude, MinAltitude, AltitudeLimit, "Altitude");
        double groundSpeed = CheckedGroundSpeed(wind);

        double climbDistance = ClimbDistance(altitude, groundSpeed);
        double descentDistance = DescentDistance(altitude, groundSpeed);
        double cruiseDistance = distance - climbDistance - descentDistance;
        if (cruiseDistance < -Tolerance)
            throw new VehicleException(
                $"{distance:0.#} km is too short to climb to {altitude:F0} m and descend again " +
                $"(at most {MaxAltitudeFor(distance, wind):F0} m).");
        cruiseDistance = Math.Max(0, cruiseDistance);

        // The aircraft really flies along the hypotenuses of the triangles.
        double altitudeKm = altitude / 1000;
        double climbPath = Math.Sqrt(climbDistance * climbDistance + altitudeKm * altitudeKm);
        double descentPath = Math.Sqrt(descentDistance * descentDistance + altitudeKm * altitudeKm);

        // Engines burn fuel by time: the wind changes the time, so it changes the fuel.
        double windFactor = CruiseSpeed / groundSpeed;
        double fuel = (FuelConsumption(climbPath) * ClimbFuelFactor
                     + FuelConsumption(cruiseDistance)
                     + FuelConsumption(descentPath) * DescentFuelFactor) * windFactor;
        double hours = SlopeHours(altitude, ClimbRate, climbDistance, groundSpeed)
                     + cruiseDistance / groundSpeed
                     + SlopeHours(altitude, DescentRate, descentDistance, groundSpeed);

        return new FlightPlan(distance, altitude, wind, groundSpeed,
                              climbDistance, cruiseDistance, descentDistance,
                              climbPath + cruiseDistance + descentPath, hours,
                              fuel, fuel * (1 + FuelReserve));
    }

    // The highest altitude for this distance: the aircraft must have time to descend.
    public double MaxAltitudeFor(double distance, double wind = 0)
    {
        EnsurePositive(distance, "Flight distance");
        double groundSpeed = CheckedGroundSpeed(wind);
        double kmPerMetre = ClimbDistance(1, groundSpeed) + DescentDistance(1, groundSpeed);   // both triangles grow with the altitude
        return kmPerMetre > 0 ? Math.Min(AltitudeLimit, distance / kmPerMetre) : AltitudeLimit;
    }

    public bool HasEnoughFuel(FlightPlan plan) => plan.FuelRequired <= FuelLevel + Tolerance;

    // Makes the flight, or cancels it if the fuel (with the reserve) is not enough.
    public FlightPlan Fly(double distance, double altitude, double wind = 0)
    {
        FlightPlan plan = PlanFlight(distance, altitude, wind);
        if (!HasEnoughFuel(plan))
            throw new VehicleException(
                $"Flight cancelled: {plan.FuelRequired:F1} L needed " +
                $"(with {FuelReserve * 100:0}% reserve), only {FuelLevel:F1} L in the tank.");

        BurnFuel(plan.FuelBurned);
        AddMileage(plan.PathLength);
        return plan;
    }

    // Horizontal leg of the left triangle, km: how far the aircraft moves over the ground
    // while climbing.
    protected virtual double ClimbDistance(double altitude, double groundSpeed)
    {
        double climbSeconds = altitude / ClimbRate;
        return groundSpeed * climbSeconds / 3600;
    }

    // Horizontal leg of the right triangle, km.
    protected virtual double DescentDistance(double altitude, double groundSpeed)
    {
        double descentSeconds = altitude / DescentRate;
        return groundSpeed * descentSeconds / 3600;
    }

    private double CheckedGroundSpeed(double wind)
    {
        if (!double.IsFinite(wind))
            throw new ArgumentException("Wind must be a number.");
        double groundSpeed = GroundSpeed(wind);
        if (groundSpeed <= 0)
            throw new VehicleException(
                $"{TypeName} {RegistrationNumber} cannot move forward: ground speed {groundSpeed:F0} km/h.");
        return groundSpeed;
    }

    // A slope takes as long as the slower of two movements: changing the altitude with the
    // vertical speed, or covering the horizontal leg with the ground speed.
    private static double SlopeHours(double altitude, double verticalSpeed,
                                     double horizontalDistance, double groundSpeed)
    {
        double verticalHours = altitude / verticalSpeed / 3600;
        double horizontalHours = horizontalDistance / groundSpeed;
        return Math.Max(verticalHours, horizontalHours);
    }

    // Line about the speed in GetInfo (a balloon shows the wind instead).
    protected virtual string SpeedInfo => $"Cruise speed: {CruiseSpeed:F0} km/h";

    public override string GetInfo()
    {
        string altitude = AltitudeLimit < MaxAltitude
            ? $"{MaxAltitude:F0} m, allowed {AltitudeLimit:F0} m"
            : $"{MaxAltitude:F0} m";
        return base.GetInfo() +
               $"\n  Max altitude: {altitude}" +
               $"\n  Climb / descent rate: {ClimbRate:0.#} / {DescentRate:0.#} m/s" +
               $"\n  {SpeedInfo}" +
               $"\n  Fuel reserve: {FuelReserve * 100:0}%";
    }
}
