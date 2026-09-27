using OOPLAB1.Fuel;
using OOPLAB1.UI;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;
using OOPLAB1.Vehicles.Air.Balloons;
using OOPLAB1.Vehicles.Air.Helicopters;
using OOPLAB1.Vehicles.Air.Planes;

namespace OOPLAB1.Tests;

// Aircraft: the flight trapezoid, wind, reserve rules and the limits of every kind.
// The expected numbers are the ones the console program printed for the demo aircraft.
public static class AirVehicleTests
{
    [Test]
    public static void Cessna_Tailwind_MatchesTheProgram()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        FlightPlan plan = cessna.PlanFlight(300, 1000, wind: 25);
        Assert.Near(251, plan.GroundSpeed, "ground speed", 1e-9);
        Assert.Near(18.8, plan.ClimbDistance, "climb, km");
        Assert.Near(253.3, plan.CruiseDistance, "cruise, km");
        Assert.Near(27.9, plan.DescentDistance, "descent, km");
        Assert.Near(43.9, plan.FuelBurned, "fuel, L");
        Assert.Near(48.3, plan.FuelRequired, "fuel with 10 % reserve, L");
        Assert.Equal("1 h 12 min", ConsolePrinter.FormatHours(plan.FlightHours), "flight time");
    }

    [Test]
    public static void Cessna_Headwind_CostsMoreFuelAndTime()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        FlightPlan plan = cessna.PlanFlight(300, 1000, wind: -40);
        Assert.Near(59.0, plan.FuelBurned, "fuel, L");
        Assert.Near(64.9, plan.FuelRequired, "fuel with reserve, L");
        Assert.Equal("1 h 37 min", ConsolePrinter.FormatHours(plan.FlightHours), "flight time");
    }

    [Test]
    public static void LightAirplane_MayNotFlyAbove3000m()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        Assert.Near(3000, cessna.AltitudeLimit, "allowed altitude", 1e-9);
        Assert.Throws<ArgumentException>(() => cessna.PlanFlight(300, 3500));
    }

    [Test]
    public static void ShortFlight_TooHighAltitude_IsRejected()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        var error = Assert.Throws<VehicleException>(() => cessna.PlanFlight(20, 2900));
        Assert.Contains("too short", error.Message);
        Assert.True(cessna.MaxAltitudeFor(20) < 2900, "the limit for 20 km is lower");
    }

    [Test]
    public static void Fly_BurnsFuelAndAddsThePath()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        FlightPlan plan = cessna.Fly(190, 2900);
        Assert.Near(87.5, cessna.FuelLevel, "120 L - 32.5 L");
        Assert.Near(150000 + plan.PathLength, cessna.Mileage, "mileage grows by the path", 1e-9);
    }

    [Test]
    public static void Fly_WithoutEnoughFuel_IsCancelled()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        Assert.Throws<VehicleException>(() => cessna.Fly(1000, 1000));
        Assert.Near(120, cessna.FuelLevel, "fuel stays", 1e-9);
        Assert.Near(150000, cessna.Mileage, "mileage stays", 1e-9);
    }

    [Test]
    public static void PassengerAirplane_ReserveGrowsWithPassengers()
    {
        var a220 = Demo.Get<PassengerAirplane>("YL-PAX");
        Assert.Near(0.10, a220.FuelReserve, "empty", 1e-9);
        a220.BoardPassengers(149);
        Assert.Near(0.15, a220.FuelReserve, "149 passengers", 1e-9);
        FlightPlan plan = a220.PlanFlight(1680, 11000, wind: 15);
        Assert.Near(5206.1, plan.FuelBurned, "fuel, L");
        Assert.Near(5987.1, plan.FuelRequired, "fuel with 15 % reserve, L");
    }

    [Test]
    public static void PassengerAirplane_ReserveStopsAt40Percent()
    {
        var jumbo = new PassengerAirplane("YL-BIG", "Boeing", "747", 100000, 200000, 0, 1200, FuelType.Kerosene,
                                          2010, 13000, 10, 10, 900, seats: 900);
        jumbo.BoardPassengers(700);
        Assert.Near(0.40, jumbo.FuelReserve, "700 passengers", 1e-9);
    }

    [Test]
    public static void CargoAirplane_CargoRaisesTheConsumption()
    {
        var boeing = Demo.Get<CargoAirplane>("YL-CRG");
        boeing.LoadCargo(23000);
        Assert.Near(455.1, boeing.FuelConsumption(100), "370 x 1.23 L/100 km", 1e-9);
        FlightPlan plan = boeing.PlanFlight(1680, 11000, wind: -50);
        Assert.Near(8714.8, plan.FuelBurned, "fuel, L");
        Assert.Near(9586.3, plan.FuelRequired, "fuel with 10 % reserve, L");
    }

    [Test]
    public static void Balloon_FliesWithTheWindAndNeedsNoReserve()
    {
        var balloon = Demo.Get<Balloon>("YL-BAL");
        Assert.Near(30, balloon.GroundSpeed(30), "ground speed = wind", 1e-9);
        FlightPlan typical = balloon.PlanFlight(15, 500, wind: 15);
        Assert.Near(62.6, typical.FuelBurned, "fuel at the typical wind, L");
        Assert.Near(typical.FuelBurned, typical.FuelRequired, "no reserve", 1e-9);
        Assert.Near(31.6, balloon.PlanFlight(15, 500, wind: 30).FuelBurned, "double wind, half the fuel");
    }

    [Test]
    public static void Balloon_WithoutWind_CannotMove()
    {
        var balloon = Demo.Get<Balloon>("YL-BAL");
        Assert.Throws<VehicleException>(() => balloon.PlanFlight(15, 500, wind: 0));
    }

    [Test]
    public static void Airship_OwnSpeedPlusWind()
    {
        var airship = Demo.Get<Airship>("YL-ZEP");
        FlightPlan plan = airship.PlanFlight(100, 500, wind: -30);
        Assert.Near(120.8, plan.FuelBurned, "fuel against 30 km/h, L");
        Assert.Near(2.0, plan.FlightHours, "100 km at 50 km/h", 0.001);
        Assert.Throws<VehicleException>(() => airship.PlanFlight(100, 500, wind: -100));
    }

    [Test]
    public static void Helicopter_ChosenAngleAndVerticalLanding()
    {
        var r44 = Demo.Get<Helicopter>("YL-HEL");
        r44.SetClimbAngle(30);
        FlightPlan plan = r44.PlanFlight(100, 1000);
        Assert.Near(1.732, plan.ClimbDistance, "1 km / tan 30 deg", 0.001);
        Assert.Near(0, plan.DescentDistance, "vertical landing", 1e-9);
        Assert.Near(32.2, plan.FuelRequired, "fuel with reserve, L");
        Assert.Throws<ArgumentException>(() => r44.SetClimbAngle(31));
    }

    [Test]
    public static void CargoHelicopter_SlingLoadLimitsTheAngle()
    {
        var h215 = Demo.Get<CargoHelicopter>("YL-HCL");
        h215.AttachSlingLoad(4000);
        Assert.Near(15, h215.AllowedClimbAngle, "angle with a sling load", 1e-9);
        Assert.Throws<ArgumentException>(() => h215.SetClimbAngle(20));
        h215.SetClimbAngle(15);
        Assert.Near(524.9, h215.PlanFlight(150, 1500, wind: 15).FuelRequired, "fuel with reserve, L");
        h215.DetachSlingLoad();
        Assert.Near(30, h215.AllowedClimbAngle, "angle without the load", 1e-9);
    }

    [Test]
    public static void OnlyAirplanesCanTaxi()
    {
        var cessna = Demo.Get<LightAirplane>("YL-ABC");
        cessna.Drive(5);
        Assert.Near(119.2, cessna.FuelLevel, "5 km of taxiing at 16 L/100 km", 1e-9);

        var r44 = Demo.Get<Helicopter>("YL-HEL");
        Assert.Throws<VehicleException>(() => r44.Drive(5));
    }
}
