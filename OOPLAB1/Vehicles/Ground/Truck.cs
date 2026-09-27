using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Ground;

// Diesel truck. Cargo increases the consumption: +1 L/100 km for every 500 kg,
// proportionally (250 kg -> +0.5, 1000 kg -> +2, 2000 kg -> +4 L/100 km).
// A trailer gives extra capacity and adds 5 L/100 km.
// Loading and unloading are common for all vehicles (Vehicle.LoadCargo / UnloadCargo).
// A heavy truck feels a hill more: every tonne of cargo adds 2 % more per 1 % uphill.
// Trucks may drive only 80 km/h outside towns and are inspected every year.
public class Truck : Automobile
{
    public const double ExtraConsumptionPer500Kg = 1;    // L/100 km
    public const double TrailerExtraConsumption = 5;     // L/100 km
    public const double ExtraUphillPerTonne = 0.02;      // per 1 % uphill
    public const double TruckHighwaySpeedLimit = 80;     // km/h
    public const int TruckInspectionYears = 1;

    public double CargoCapacity { get; }                 // kg, the truck itself
    public double TrailerCapacity { get; private set; }  // kg, 0 = no trailer

    public Truck(string registrationNumber, string brand, string model,
                 double fuelLevel, double tankCapacity, double mileage,
                 double fuelConsumptionRate, int developYear, double cargoCapacity,
                 SteeringSide steeringSide, DateOnly inspectionValidUntil)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, FuelType.Diesel, developYear, steeringSide, inspectionValidUntil)
    {
        EnsurePositive(cargoCapacity, "Cargo capacity");
        CargoCapacity = cargoCapacity;
    }

    public override string TypeName => "Truck";

    public override double HighwaySpeedLimit => TruckHighwaySpeedLimit;

    protected override int InspectionYears => TruckInspectionYears;

    protected override double UphillIncrease =>
        UphillIncreasePerPercent + ExtraUphillPerTonne * CurrentCargo / 1000;

    public bool HasTrailer => TrailerCapacity > 0;

    // The trailer adds its capacity to the truck's own.
    public override double MaxCargo => CargoCapacity + TrailerCapacity;

    public void AttachTrailer(double trailerCapacity)
    {
        if (HasTrailer)
            throw new VehicleException("A trailer is already attached.");
        EnsurePositive(trailerCapacity, "Trailer capacity");

        TrailerCapacity = trailerCapacity;
    }

    // The trailer can be detached only if all cargo fits into the truck itself.
    public void DetachTrailer()
    {
        if (!HasTrailer)
            throw new VehicleException("There is no trailer to detach.");
        if (CurrentCargo > CargoCapacity + Tolerance)
            throw new VehicleException(
                $"Unload at least {CurrentCargo - CargoCapacity:F0} kg first: " +
                "the cargo does not fit into the truck without the trailer.");

        TrailerCapacity = 0;
    }

    protected override double FlatRoadConsumption(double distance)
    {
        double extraPer100Km = CurrentCargo / 500 * ExtraConsumptionPer500Kg;
        if (HasTrailer)
            extraPer100Km += TrailerExtraConsumption;
        return base.FlatRoadConsumption(distance) + distance / 100 * extraPer100Km;
    }

    public override string GetInfo()
    {
        string trailer = HasTrailer ? $"attached, {TrailerCapacity:F0} kg" : "none";
        return base.GetInfo() + $"\n  Trailer: {trailer}";
    }
}
