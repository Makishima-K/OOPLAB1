namespace OOPLAB1.Vehicles;

// Electric car: it has its own battery instead of a fuel tank.
// Battery in kWh, consumption rate (FuelConsumptionRate) in kWh/100 km, charger power in kW.
// The inherited FuelLevel and TankCapacity stay 0 - there is no fuel tank.
public class ElectricCar : Car
{
    public const double DefaultChargerPower = 11;   // kW, a usual home charger

    public double BatteryCapacity { get; }
    public double BatteryCharge { get; private set; }

    public ElectricCar(string registrationNumber, string brand, string model,
                       double batteryCharge, double batteryCapacity, double mileage,
                       double energyConsumptionRate, int developYear,
                       int numberOfDoors, int seats)
        : base(registrationNumber, brand, model, mileage, energyConsumptionRate,
               developYear, numberOfDoors, seats)
    {
        EnsurePositive(batteryCapacity, "Battery capacity");
        EnsureInRange(batteryCharge, 0, batteryCapacity, "Battery charge");
        BatteryCapacity = batteryCapacity;
        BatteryCharge = batteryCharge;
    }

    public override string TypeName => "Electric car";

    public override string FuelUnit => "kWh";

    public double FreeBatteryCapacity => BatteryCapacity - BatteryCharge;

    public double ChargePercent => BatteryCharge / BatteryCapacity * 100;

    public override string EnergyStatus =>
        $"Battery: {BatteryCharge:F1} / {BatteryCapacity:F1} kWh ({ChargePercent:F0}%)";

    public override double Range => BatteryCharge / FuelConsumption(100) * 100;

    // Driving uses the battery, not the fuel tank.
    public override void Drive(double distance)
    {
        EnsurePositive(distance, "Distance");
        double energyNeeded = FuelConsumption(distance);
        if (energyNeeded > BatteryCharge + Tolerance)
            throw new VehicleException(
                $"Not enough charge: {energyNeeded:F1} kWh needed, {BatteryCharge:F1} kWh " +
                $"in the battery (enough for {Range:F0} km).");

        BatteryCharge = Math.Max(0, BatteryCharge - energyNeeded);
        AddMileage(distance);
    }

    // An electric car cannot be filled with fuel: "refuelling" means charging it with a
    // usual charger until the requested energy is added. Returns the price.
    public override double Refuel(double amount, double pricePerUnit)
    {
        EnsurePositive(amount, "Amount of energy");
        double hoursNeeded = amount / DefaultChargerPower;
        return Charge(amount, DefaultChargerPower, hoursNeeded, pricePerUnit).Cost;
    }

    // Charges `amount` kWh with a charger of `chargerPower` kW, but not longer than
    // `availableHours`. If there is not enough time, only a part of the energy is added.
    public (double Added, double Hours, double Cost) Charge(double amount, double chargerPower,
                                                             double availableHours, double pricePerKWh)
    {
        EnsurePositive(amount, "Amount of energy");
        EnsurePositive(chargerPower, "Charger power");
        EnsurePositive(availableHours, "Available time");
        EnsureNotNegative(pricePerKWh, "Price");
        if (amount > FreeBatteryCapacity + Tolerance)
            throw new VehicleException($"Only {FreeBatteryCapacity:F1} kWh fits into the battery.");

        double added = Math.Min(amount, chargerPower * availableHours);
        BatteryCharge = Math.Min(BatteryCapacity, BatteryCharge + added);
        return (added, added / chargerPower, added * pricePerKWh);
    }

    // Hours needed to charge the battery to 100 % with the given charger.
    public double TimeToFullCharge(double chargerPower)
    {
        EnsurePositive(chargerPower, "Charger power");
        return FreeBatteryCapacity / chargerPower;
    }

    // Price of charging the battery to 100 %.
    public double CalculateChargingCost(double pricePerKWh)
    {
        EnsureNotNegative(pricePerKWh, "Price");
        return FreeBatteryCapacity * pricePerKWh;
    }
}
