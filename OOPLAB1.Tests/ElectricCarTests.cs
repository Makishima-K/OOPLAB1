using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Ground.Cars;

namespace OOPLAB1.Tests;

// Electric car: its own battery instead of a fuel tank, charging with power, time and price.
// Demo Tesla: 45 of 75 kWh, 15 kWh/100 km.
public static class ElectricCarTests
{
    [Test]
    public static void Drive_UsesTheBattery()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        tesla.Drive(100);
        Assert.Near(30, tesla.BatteryCharge, "kWh left", 1e-9);
        Assert.Near(0, tesla.FuelLevel, "there is no fuel tank", 1e-9);
        Assert.Near(8100, tesla.Mileage, "mileage", 1e-9);
    }

    [Test]
    public static void Drive_WithoutEnoughCharge_IsRejected()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        Assert.Throws<VehicleException>(() => tesla.Drive(400));
        Assert.Near(45, tesla.BatteryCharge, "charge stays", 1e-9);
    }

    [Test]
    public static void Charge_WithTooLittleTime_ChargesOnlyWhatFits()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        var (added, hours, cost) = tesla.Charge(20, chargerPower: 11, availableHours: 1, pricePerKWh: 0.25);
        Assert.Near(11, added, "kWh added in 1 h at 11 kW", 1e-9);
        Assert.Near(1, hours, "hours", 1e-9);
        Assert.Near(2.75, cost, "EUR", 1e-9);
        Assert.Near(56, tesla.BatteryCharge, "charge", 1e-9);
    }

    [Test]
    public static void Charge_MoreThanTheFreeCapacity_IsRejected()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        Assert.Throws<VehicleException>(() => tesla.Charge(31, 11, 5, 0.25));
        Assert.Throws<ArgumentException>(() => tesla.Charge(10, 0, 5, 0.25));
    }

    [Test]
    public static void FullCharge_TimeAndCost()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        Assert.Near(30.0 / 11, tesla.TimeToFullCharge(11), "hours at 11 kW", 1e-9);
        Assert.Near(7.5, tesla.CalculateChargingCost(0.25), "EUR for 30 kWh", 1e-9);
    }

    [Test]
    public static void Refuel_MeansChargingWithAHomeCharger()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        double cost = tesla.Refuel(10, 0.25);
        Assert.Near(2.5, cost, "EUR", 1e-6);
        Assert.Near(55, tesla.BatteryCharge, "charge", 1e-6);
        Assert.Equal("kWh", tesla.FuelUnit, "unit");
    }
}
