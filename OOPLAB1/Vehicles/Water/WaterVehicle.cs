using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Water;

// Base class of water transport. A voyage goes with or against a current:
// ground speed = speed through the water + current (+ with the current, - against).
// Engines burn fuel by time, so fuel is counted for the distance through the water:
//     water distance = distance * speed / (speed + current)
// A current against the vessel makes this way longer, a current with it makes it shorter.
// Every vessel has a draft: on a route shallower than the draft it would run aground.
// Vessels cannot drive on land.
public abstract class WaterVehicle : Vehicle
{
    public double CruiseSpeed { get; }      // km/h through the water, engines at cruise power
    public virtual double Draft { get; }    // m, how deep the hull goes into the water

    protected WaterVehicle(string registrationNumber, string brand, string model,
                           double fuelLevel, double tankCapacity, double mileage,
                           double fuelConsumptionRate, FuelType fuelType, int developYear,
                           double cruiseSpeed, double draft)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, EnsureNotElectric(fuelType), developYear)
    {
        EnsurePositive(cruiseSpeed, "Cruise speed");
        EnsurePositive(draft, "Draft");
        CruiseSpeed = cruiseSpeed;
        Draft = draft;
    }

    public override bool CanDrive => false;

    // Extra fuel that must be on board before the voyage: 0.15 = +15 %.
    public virtual double FuelReserve => 0;

    // Real speed through the water; cargo slows a ship down, sails speed a sailboat up.
    public virtual double SpeedThroughWater => CruiseSpeed;

    // The engines always work at cruise power: when the vessel is slower or faster through
    // the water, every km costs proportionally more or less fuel.
    public override double FuelConsumption(double distance)
    {
        return base.FuelConsumption(distance) * CruiseSpeed / SpeedThroughWater;
    }

    // Calculates the voyage without making it.
    public VoyagePlan PlanVoyage(double distance, double current, double routeDepth)
    {
        EnsurePositive(distance, "Voyage distance");
        EnsurePositive(routeDepth, "Route depth");
        if (!double.IsFinite(current))
            throw new ArgumentException("Current must be a number.");
        if (Draft > routeDepth + Tolerance)
            throw new VehicleException(
                $"Draft {Draft:F1} m is deeper than the route ({routeDepth:F1} m): " +
                $"{TypeName} {RegistrationNumber} would run aground.");

        double speed = SpeedThroughWater;
        double groundSpeed = speed + current;
        if (groundSpeed <= 0)
            throw new VehicleException(
                $"The current ({-current:F0} km/h against) is not weaker than the speed of " +
                $"{TypeName} {RegistrationNumber} ({speed:F1} km/h): it cannot move forward.");

        double hours = distance / groundSpeed;
        double waterDistance = speed * hours;
        double fuel = FuelConsumption(waterDistance);
        return new VoyagePlan(distance, current, speed, groundSpeed, waterDistance, hours,
                              fuel, fuel * (1 + FuelReserve), Draft, routeDepth);
    }

    public bool HasEnoughFuel(VoyagePlan plan) => plan.FuelRequired <= FuelLevel + Tolerance;

    // Makes the voyage, or cancels it if the fuel (with the reserve) is not enough.
    public VoyagePlan Sail(double distance, double current, double routeDepth)
    {
        VoyagePlan plan = PlanVoyage(distance, current, routeDepth);
        if (!HasEnoughFuel(plan))
            throw new VehicleException(
                $"Voyage cancelled: {plan.FuelRequired:F1} L needed, only {FuelLevel:F1} L on board.");

        BurnFuel(plan.FuelBurned);
        AddMileage(plan.WaterDistance);
        return plan;
    }

    public override string GetInfo()
    {
        return base.GetInfo() +
               $"\n  Speed through water: {SpeedThroughWater:F1} km/h (cruise {CruiseSpeed:F0} km/h)" +
               $"\n  Draft: {Draft:F1} m" +
               $"\n  Fuel reserve: {FuelReserve * 100:0}%";
    }
}
