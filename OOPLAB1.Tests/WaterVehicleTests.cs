using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Water;
using OOPLAB1.Vehicles.Water.Boats;
using OOPLAB1.Vehicles.Water.Ships;

namespace OOPLAB1.Tests;

// Vessels: the current changes the way through the water, draft against the route depth,
// fuel by time, cargo ship, liner and sailboat. The numbers are the program's console output.
public static class WaterVehicleTests
{
    [Test]
    public static void MotorBoat_AgainstTheCurrent_GoesFartherThroughTheWater()
    {
        var boat = Demo.Get<MotorBoat>("LV-1001");
        VoyagePlan plan = boat.PlanVoyage(60, current: -5, routeDepth: 3);
        Assert.Near(40, plan.GroundSpeed, "45 - 5 km/h", 1e-9);
        Assert.Near(67.5, plan.WaterDistance, "way through the water, km", 1e-9);
        Assert.Near(1.5, plan.Hours, "hours", 1e-9);
        Assert.Near(45.2, plan.FuelBurned, "fuel, L");
    }

    [Test]
    public static void MotorBoat_WithTheCurrent_GoesLessThroughTheWater()
    {
        var boat = Demo.Get<MotorBoat>("LV-1001");
        VoyagePlan plan = boat.PlanVoyage(60, current: 5, routeDepth: 3);
        Assert.Near(54.0, plan.WaterDistance, "way through the water, km", 1e-9);
        Assert.Near(36.2, plan.FuelBurned, "fuel, L");
    }

    [Test]
    public static void Sail_BurnsFuelAndAddsTheWayThroughTheWater()
    {
        var boat = Demo.Get<MotorBoat>("LV-1001");
        boat.Sail(60, current: -5, routeDepth: 3);
        Assert.Near(54.775, boat.FuelLevel, "100 L - 45.225 L", 1e-9);
        Assert.Near(1267.5, boat.Mileage, "1200 + 67.5 km", 1e-9);
    }

    [Test]
    public static void CargoShip_LoadedSitsDeeperAndIsSlower()
    {
        var ship = Demo.Get<CargoShip>("LV-CARGO1");
        Assert.Near(3960.0, ship.PlanVoyage(300, -3, 10).FuelBurned, "empty: fuel, L");
        ship.LoadCargo(3_850_000);
        Assert.Near(5.5, ship.Draft, "draft at full load, m", 1e-9);
        Assert.Near(17.6, ship.SpeedThroughWater, "speed at full load, km/h", 1e-9);
        Assert.Near(5153.4, ship.PlanVoyage(300, -3, 10).FuelBurned, "loaded: fuel, L");
    }

    [Test]
    public static void CargoShip_Loaded_RunsAgroundOnAShallowRoute()
    {
        var ship = Demo.Get<CargoShip>("LV-CARGO1");
        ship.LoadCargo(3_850_000);
        var error = Assert.Throws<VehicleException>(() => ship.PlanVoyage(300, -3, routeDepth: 5));
        Assert.Contains("aground", error.Message);
    }

    [Test]
    public static void CurrentAsStrongAsTheVessel_CannotBeSailed()
    {
        var sailboat = Demo.Get<Sailboat>("LV-SAIL1");
        sailboat.SetWind(0);
        Assert.Throws<VehicleException>(() => sailboat.PlanVoyage(50, current: -12, routeDepth: 5));
    }

    [Test]
    public static void PassengerLiner_LifeboatsAndReserve()
    {
        var liner = Demo.Get<PassengerLiner>("LV-LINER1");
        liner.BoardPassengers(51);
        Assert.Equal(2, liner.Lifeboats, "lifeboats for 51 passengers");
        liner.BoardPassengers(1949);
        Assert.Equal(40, liner.Lifeboats, "lifeboats for 2000 passengers");
        VoyagePlan plan = liner.PlanVoyage(400, 0, 12);
        Assert.Near(39600, plan.FuelBurned, "fuel, L", 1e-6);
        Assert.Near(45540, plan.FuelRequired, "fuel with 15 % reserve, L", 1e-6);
    }

    [Test]
    public static void Sailboat_WindAddsHalfOfItsSpeed()
    {
        var sailboat = Demo.Get<Sailboat>("LV-SAIL1");
        sailboat.SetWind(20);
        Assert.Near(22, sailboat.SpeedThroughWater, "12 + 0.5 x 20 km/h", 1e-9);
        Assert.Near(9.0, sailboat.PlanVoyage(50, 0, 5).FuelBurned, "fuel, L");
        Assert.Throws<ArgumentException>(() => sailboat.SetWind(-1));
        Assert.Throws<ArgumentException>(() => sailboat.SetWind(151));
    }
}
