namespace OOPLAB1.Vehicles;

// Diesel truck. Cargo increases the consumption: +1 L/100 km for every 500 kg,
// proportionally (250 kg -> +0.5, 1000 kg -> +2, 2000 kg -> +4 L/100 km).
// A trailer gives extra capacity and adds 5 L/100 km.
public class Truck : Vehicle
{
    public const double ExtraConsumptionPer500Kg = 1;    // L/100 km
    public const double TrailerExtraConsumption = 5;     // L/100 km

    public double CargoCapacity { get; }                 // kg, the truck itself
    public double TrailerCapacity { get; private set; }  // kg, 0 = no trailer
    public double CurrentCargo { get; private set; }     // kg

    public Truck(string registrationNumber, string brand, string model,
                 double fuelLevel, double tankCapacity, double mileage,
                 double fuelConsumptionRate, int developYear, double cargoCapacity)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, FuelType.Diesel, developYear)
    {
        EnsurePositive(cargoCapacity, "Cargo capacity");
        CargoCapacity = cargoCapacity;
    }

    public override string TypeName => "Truck";

    public bool HasTrailer => TrailerCapacity > 0;

    public double TotalCapacity => CargoCapacity + TrailerCapacity;

    public double FreeCapacity => TotalCapacity - CurrentCargo;

    public double LoadPercent => CurrentCargo / TotalCapacity * 100;

    public void LoadCargo(double weight)
    {
        EnsurePositive(weight, "Cargo weight");
        if (weight > FreeCapacity + Tolerance)
            throw new VehicleException(
                $"Cannot load {weight:F0} kg: only {FreeCapacity:F0} kg of free capacity.");

        CurrentCargo = Math.Min(TotalCapacity, CurrentCargo + weight);
    }

    public void UnloadCargo(double weight)
    {
        EnsurePositive(weight, "Cargo weight");
        if (weight > CurrentCargo + Tolerance)
            throw new VehicleException(
                $"Cannot unload {weight:F0} kg: only {CurrentCargo:F0} kg is loaded.");

        CurrentCargo = Math.Max(0, CurrentCargo - weight);
    }

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

    public override double FuelConsumption(double distance)
    {
        double extraPer100Km = CurrentCargo / 500 * ExtraConsumptionPer500Kg;
        if (HasTrailer)
            extraPer100Km += TrailerExtraConsumption;
        return base.FuelConsumption(distance) + distance / 100 * extraPer100Km;
    }

    public override string GetInfo()
    {
        string trailer = HasTrailer ? $"attached, {TrailerCapacity:F0} kg" : "none";
        return base.GetInfo() +
               $"\n  Cargo: {CurrentCargo:F0} / {TotalCapacity:F0} kg ({LoadPercent:F0}%)" +
               $"\n  Trailer: {trailer}";
    }
}
