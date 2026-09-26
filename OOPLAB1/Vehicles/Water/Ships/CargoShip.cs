using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Water.Ships;

// Cargo ship. Cargo does not weaken the current - the current carries the ship with the
// water whatever it weighs. Cargo does two other things:
//  * the ship sinks deeper: the draft grows from the empty draft to LoadedDraft at full load,
//    so a loaded ship may not pass a shallow route;
//  * the ship is slower (up to 20 % at full load), so the current matters more, and every km
//    costs more fuel (see WaterVehicle.FuelConsumption).
public class CargoShip : WaterVehicle
{
    public const double SlowdownAtFullLoad = 0.2;

    public double CargoCapacity { get; }   // kg
    public double LoadedDraft { get; }     // m, at full load

    public CargoShip(string registrationNumber, string brand, string model,
                     double fuelLevel, double tankCapacity, double mileage,
                     double fuelConsumptionRate, FuelType fuelType, int developYear,
                     double cruiseSpeed, double emptyDraft, double loadedDraft, double cargoCapacity)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear, cruiseSpeed, emptyDraft)
    {
        EnsurePositive(cargoCapacity, "Cargo capacity");
        if (!double.IsFinite(loadedDraft) || loadedDraft < emptyDraft)
            throw new ArgumentException("Loaded draft cannot be less than the empty draft.");
        CargoCapacity = cargoCapacity;
        LoadedDraft = loadedDraft;
    }

    public override string TypeName => "Cargo ship";

    public override double MaxCargo => CargoCapacity;

    public double EmptyDraft => base.Draft;

    private double LoadShare => CurrentCargo / CargoCapacity;

    public override double Draft => EmptyDraft + (LoadedDraft - EmptyDraft) * LoadShare;

    public override double SpeedThroughWater => CruiseSpeed * (1 - SlowdownAtFullLoad * LoadShare);

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Draft empty / full: {EmptyDraft:F1} / {LoadedDraft:F1} m";
    }
}
