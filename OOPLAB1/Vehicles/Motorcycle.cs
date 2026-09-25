namespace OOPLAB1.Vehicles;

// Motorcycle. One pillion passenger, one more with a sidecar.
// A sidecar adds 15 % to the consumption, every passenger adds 5 %.
public class Motorcycle : Vehicle
{
    public const double SidecarConsumptionFactor = 1.15;
    public const double PassengerConsumptionIncrease = 0.05;

    public bool HasSidecar { get; private set; }

    public Motorcycle(string registrationNumber, string brand, string model,
                      double fuelLevel, double tankCapacity, double mileage,
                      double fuelConsumptionRate, FuelType fuelType, int developYear,
                      bool hasSidecar)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, EnsureNotElectric(fuelType), developYear)
    {
        HasSidecar = hasSidecar;
    }

    public override string TypeName => "Motorcycle";

    public override int MaxPassengers => HasSidecar ? 2 : 1;

    protected override double ConsumptionIncreasePerPassenger => PassengerConsumptionIncrease;

    public void AttachSidecar()
    {
        if (HasSidecar)
            throw new VehicleException("A sidecar is already attached.");

        HasSidecar = true;
    }

    public void DetachSidecar()
    {
        if (!HasSidecar)
            throw new VehicleException("There is no sidecar to detach.");
        if (Passengers > 1)
            throw new VehicleException("The passenger in the sidecar must get off first.");

        HasSidecar = false;
    }

    public override double FuelConsumption(double distance)
    {
        double sidecarFactor = HasSidecar ? SidecarConsumptionFactor : 1;
        return base.FuelConsumption(distance) * sidecarFactor;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Sidecar: {(HasSidecar ? "yes" : "no")}";
    }
}
