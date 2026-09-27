using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Ground;

// Base class of ground transport: cars, trucks and motorcycles. What they all have:
//  * a technical inspection valid until a date. An expired one is only a warning - the
//    vehicle can still drive. Passing it again gives the interval of the class;
//  * the road of the next trip: city or highway and the average gradient. The road gives the
//    speed limit and so the travel time (Latvia: 50 km/h in towns, 90 km/h outside, trucks
//    80 km/h). The gradient changes the consumption.
// Drive itself is not changed: it asks FuelConsumption, and FuelConsumption knows the road.
// An airplane taxiing on the ground is not a ground vehicle, so it keeps the plain Drive.
public abstract class GroundVehicle : Vehicle
{
    public const double CitySpeedLimit = 50;              // km/h
    public const double DefaultHighwaySpeedLimit = 90;    // km/h
    public const double MaxGradient = 10;                 // %, steeper roads are rare
    public const double UphillIncreasePerPercent = 0.10;  // +10 % consumption per 1 % uphill
    public const double DownhillSavingPerPercent = 0.05;  // -5 % per 1 % downhill: the engine still runs
    public const int DefaultInspectionYears = 2;
    public const int MaxInspectionYears = 4;              // a new car has its first inspection after 4 years

    public DateOnly InspectionValidUntil { get; private set; }
    public RoadType Road { get; private set; } = RoadType.Highway;
    public double Gradient { get; private set; }          // %, average for the trip: + uphill, - downhill

    protected GroundVehicle(string registrationNumber, string brand, string model,
                            double fuelLevel, double tankCapacity, double mileage,
                            double fuelConsumptionRate, FuelType fuelType, int developYear,
                            DateOnly inspectionValidUntil)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear)
    {
        var earliest = new DateOnly(developYear, 1, 1);
        DateOnly latest = Today.AddYears(MaxInspectionYears);
        if (inspectionValidUntil < earliest || inspectionValidUntil > latest)
            throw new ArgumentException(
                $"Inspection date must be from {earliest:yyyy-MM-dd} to {latest:yyyy-MM-dd}.");
        InspectionValidUntil = inspectionValidUntil;
    }

    // How long a new inspection is valid; a truck is checked every year.
    protected virtual int InspectionYears => DefaultInspectionYears;

    public bool InspectionExpired => Today > InspectionValidUntil;

    // Passes the inspection today and returns the new date.
    public DateOnly PassInspection()
    {
        InspectionValidUntil = Today.AddYears(InspectionYears);
        return InspectionValidUntil;
    }

    public virtual double HighwaySpeedLimit => DefaultHighwaySpeedLimit;

    public double SpeedLimitOn(RoadType road) => road == RoadType.City ? CitySpeedLimit : HighwaySpeedLimit;

    public double SpeedLimit => SpeedLimitOn(Road);

    // Chooses the road for the next trip (like the climb angle of a helicopter).
    public void SetRoad(RoadType road, double gradient)
    {
        if (!Enum.IsDefined(road))
            throw new ArgumentException("Unknown road type.");
        EnsureInRange(gradient, -MaxGradient, MaxGradient, "Gradient");
        Road = road;
        Gradient = gradient;
    }

    // Hours to drive the distance at the speed limit of the road.
    public double TravelHours(double distance)
    {
        EnsurePositive(distance, "Distance");
        return distance / SpeedLimit;
    }

    // Change of the consumption for every 1 % of gradient; subclasses may change it.
    protected virtual double UphillIncrease => UphillIncreasePerPercent;
    protected virtual double DownhillSaving => DownhillSavingPerPercent;

    // 1 on a flat road, more uphill, less downhill.
    public double GradientFactor => Gradient >= 0
        ? 1 + UphillIncrease * Gradient
        : 1 + DownhillSaving * Gradient;

    // Consumption on the road of the trip: first on a flat road (every class adds its own load
    // there), then the gradient. Sealed, so no subclass can skip the gradient.
    public sealed override double FuelConsumption(double distance)
    {
        return FlatRoadConsumption(distance) * GradientFactor;
    }

    protected virtual double FlatRoadConsumption(double distance) => base.FuelConsumption(distance);

    public static string RoadName(RoadType road) => road == RoadType.City ? "city" : "highway";

    public override string GetInfo()
    {
        string inspection = InspectionExpired
            ? $"EXPIRED on {InspectionValidUntil:yyyy-MM-dd}"
            : $"valid until {InspectionValidUntil:yyyy-MM-dd}";
        string interval = InspectionYears == 1 ? "every year" : $"every {InspectionYears} years";
        return base.GetInfo() +
               $"\n  Technical inspection: {inspection} ({interval})" +
               $"\n  Speed limit: city {CitySpeedLimit:0} km/h, highway {HighwaySpeedLimit:0} km/h" +
               $"\n  Road of the next trip: {RoadName(Road)}, gradient {Gradient:+0.#;-0.#;0} %";
    }

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
