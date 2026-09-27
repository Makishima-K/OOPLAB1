using OOPLAB1.Fuel;
using OOPLAB1.UI;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air.Planes;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;

namespace OOPLAB1.Tests;

// Fuel prices, the default wind, the fleet and the time format.
public static class SettingsAndFleetTests
{
    [Test]
    public static void FuelPrices_DefaultsAndUnits()
    {
        var prices = new FuelPrices();
        Assert.Near(1.65, prices.GetPrice(FuelType.Petrol), "petrol", 1e-9);
        Assert.Near(1.55, prices.GetPrice(FuelType.Diesel), "diesel", 1e-9);
        Assert.Near(0.85, prices.GetPrice(FuelType.Gas), "gas", 1e-9);
        Assert.Near(1.30, prices.GetPrice(FuelType.Kerosene), "kerosene", 1e-9);
        Assert.Equal("0.25 EUR/kWh", prices.Describe(FuelType.Electric), "electricity");
    }

    [Test]
    public static void FuelPrices_CanBeChangedButNotNegative()
    {
        var prices = new FuelPrices();
        prices.SetPrice(FuelType.Petrol, 1.80);
        Assert.Equal("1.80 EUR/L", prices.Describe(FuelType.Petrol), "new petrol price");
        Assert.Throws<ArgumentException>(() => prices.SetPrice(FuelType.Petrol, -1));
    }

    [Test]
    public static void Wind_DefaultIs15AndLimitsAre0To150()
    {
        var wind = new Wind();
        Assert.Near(15, wind.Speed, "default", 1e-9);
        wind.SetSpeed(0);
        Assert.Equal("0 km/h", wind.Describe(), "calm");
        Assert.Throws<ArgumentException>(() => wind.SetSpeed(151));
    }

    [Test]
    public static void Fleet_HasTheFifteenDemoVehicles()
    {
        Assert.Equal(15, Demo.NewFleet().Count, "demo vehicles");
    }

    [Test]
    public static void Fleet_RejectsADuplicateRegistrationNumber()
    {
        Fleet fleet = Demo.NewFleet();
        var copy = new Car("ab-1234", "Opel", "Astra", 10, 50, 0, 6, FuelType.Petrol, 2020, 4, 5,
                           SteeringSide.Left, DateOnly.FromDateTime(DateTime.Today));
        Assert.Throws<VehicleException>(() => fleet.Add(copy));
        Assert.Equal(15, fleet.Count, "vehicles");
    }

    [Test]
    public static void Fleet_FindIgnoresCaseAndSpaces()
    {
        Vehicle? found = Demo.NewFleet().Find(" yl-abc ");
        Assert.True(found is LightAirplane, "the Cessna is found by \" yl-abc \"");
    }

    [Test]
    public static void FormatHours_LikeTheConsole()
    {
        Assert.Equal("30 min", ConsolePrinter.FormatHours(0.5), "0.5 h");
        Assert.Equal("1 h 12 min", ConsolePrinter.FormatHours(1.2), "1.2 h");
        Assert.Equal("2 h 45 min", ConsolePrinter.FormatHours(2.75), "2.75 h");
        Assert.Equal("3 h", ConsolePrinter.FormatHours(3), "3 h");
    }
}
