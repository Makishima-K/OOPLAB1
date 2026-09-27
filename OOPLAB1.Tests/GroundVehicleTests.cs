using OOPLAB1.Fuel;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;

namespace OOPLAB1.Tests;

// Ground vehicles: gradient, speed limits and travel time, technical inspection, steering wheel,
// truck with a trailer and motorcycle with a sidecar.
public static class GroundVehicleTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    [Test]
    public static void Gradient_Uphill_CostsTenPercentPerPercent()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        toyota.SetRoad(RoadType.Highway, 3);
        Assert.Near(7.80, toyota.FuelConsumption(100), "L/100 km at +3 %", 1e-9);
    }

    [Test]
    public static void Gradient_Downhill_SavesFivePercentPerPercent()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        toyota.SetRoad(RoadType.Highway, -4);
        Assert.Near(4.80, toyota.FuelConsumption(100), "L/100 km at -4 %", 1e-9);
    }

    [Test]
    public static void ElectricCar_Downhill_RecoversMoreEnergy()
    {
        var tesla = Demo.Get<ElectricCar>("EV-2022");
        tesla.SetRoad(RoadType.Highway, -5);
        Assert.Near(9.00, tesla.FuelConsumption(100), "kWh/100 km at -5 %", 1e-9);
        Assert.Near(500, tesla.Range, "range, km", 1e-6);
    }

    [Test]
    public static void LoadedTruck_FeelsTheHillMore()
    {
        var iveco = Demo.Get<Truck>("TR-7700");
        iveco.AttachTrailer(3000);
        iveco.LoadCargo(4000);
        Assert.Near(25.00, iveco.FuelConsumption(100), "flat road with 4 t and a trailer", 1e-9);
        iveco.SetRoad(RoadType.Highway, 2);
        Assert.Near(34.00, iveco.FuelConsumption(100), "+2 % with 4 t", 1e-9);
    }

    [Test]
    public static void Gradient_OutOfRange_IsRejected()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        Assert.Throws<ArgumentException>(() => toyota.SetRoad(RoadType.Highway, 10.5));
        Assert.Throws<ArgumentException>(() => toyota.SetRoad(RoadType.City, -11));
    }

    [Test]
    public static void TravelTime_UsesTheLatvianSpeedLimits()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        toyota.SetRoad(RoadType.City, 0);
        Assert.Near(0.6, toyota.TravelHours(30), "30 km in town at 50 km/h", 1e-9);
        toyota.SetRoad(RoadType.Highway, 0);
        Assert.Near(1.0, toyota.TravelHours(90), "90 km on the highway at 90 km/h", 1e-9);

        var iveco = Demo.Get<Truck>("TR-7700");
        Assert.Near(80, iveco.HighwaySpeedLimit, "truck limit", 1e-9);
        Assert.Near(1.0, iveco.TravelHours(80), "80 km for a truck", 1e-9);
    }

    [Test]
    public static void Drive_UsesTheRoadOfTheTrip()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        toyota.SetRoad(RoadType.Highway, 3);
        toyota.Drive(120);
        Assert.Near(22.64, toyota.FuelLevel, "32 L - 9.36 L", 1e-9);
        Assert.Near(15120, toyota.Mileage, "mileage", 1e-9);
    }

    [Test]
    public static void ExpiredInspection_IsOnlyAWarning()
    {
        var honda = Demo.Get<Motorcycle>("MC-500");
        Assert.True(honda.InspectionExpired, "the demo Honda's inspection has expired");
        honda.Drive(20);
        Assert.Near(9.1, honda.FuelLevel, "it still drove 20 km", 1e-9);
    }

    [Test]
    public static void PassInspection_TwoYears_TruckOneYear()
    {
        var honda = Demo.Get<Motorcycle>("MC-500");
        Assert.Equal(Today.AddYears(2), honda.PassInspection(), "motorcycle");
        Assert.False(honda.InspectionExpired, "expired after passing");

        var iveco = Demo.Get<Truck>("TR-7700");
        Assert.Equal(Today.AddYears(1), iveco.PassInspection(), "truck");
    }

    [Test]
    public static void InspectionDate_OutsideTheLimits_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => NewCar(new DateOnly(2014, 12, 31)));   // before the year of make
        Assert.Throws<ArgumentException>(() => NewCar(Today.AddYears(5)));           // more than 4 years ahead
        NewCar(Today.AddYears(4));
    }

    [Test]
    public static void SteeringWheel_OnlyAutomobilesHaveIt()
    {
        var toyota = Demo.Get<Car>("AB-1234");
        Assert.Equal(SteeringSide.Right, toyota.SteeringSide, "the demo Toyota");
        Assert.True(typeof(Automobile).IsAssignableFrom(typeof(Truck)), "a truck is an Automobile");
        Assert.False(typeof(Automobile).IsAssignableFrom(typeof(Motorcycle)), "a motorcycle is an Automobile");
        Assert.True(typeof(GroundVehicle).IsAssignableFrom(typeof(Motorcycle)), "a motorcycle is a GroundVehicle");
    }

    [Test]
    public static void Truck_CargoAndTrailerAddToConsumption()
    {
        var iveco = Demo.Get<Truck>("TR-7700");
        iveco.LoadCargo(1500);
        Assert.Near(15, iveco.FuelConsumption(100), "+1 L/100 km per 500 kg", 1e-9);
        iveco.AttachTrailer(3000);
        Assert.Near(20, iveco.FuelConsumption(100), "+5 L/100 km for the trailer", 1e-9);
        Assert.Near(5000, iveco.MaxCargo, "capacity with the trailer", 1e-9);
    }

    [Test]
    public static void Truck_TrailerDetachesOnlyWhenTheCargoFits()
    {
        var iveco = Demo.Get<Truck>("TR-7700");
        iveco.AttachTrailer(3000);
        iveco.LoadCargo(4000);
        Assert.Throws<VehicleException>(() => iveco.DetachTrailer());
        iveco.UnloadCargo(2000);
        iveco.DetachTrailer();
        Assert.Near(2000, iveco.MaxCargo, "capacity without the trailer", 1e-9);
    }

    [Test]
    public static void Motorcycle_SidecarAndPassengers()
    {
        var honda = Demo.Get<Motorcycle>("MC-500");
        honda.BoardPassengers(1);
        Assert.Throws<VehicleException>(() => honda.BoardPassengers(1));
        honda.AttachSidecar();
        honda.BoardPassengers(1);
        Assert.Near(5.6925, honda.FuelConsumption(100), "4.5 x 1.10 x 1.15", 1e-9);
        Assert.Throws<VehicleException>(() => honda.DetachSidecar());
    }

    private static Car NewCar(DateOnly inspection) =>
        new("LV-3000", "Opel", "Astra", 40, 52, 90000, 5.8, FuelType.Petrol, 2015, 5, 5,
            SteeringSide.Left, inspection);
}
