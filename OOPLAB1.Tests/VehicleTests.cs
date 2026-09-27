using OOPLAB1.Fuel;
using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;
using OOPLAB1.Vehicles.Water.Boats;

namespace OOPLAB1.Tests;

// The base task of variant 6: registration number, brand, model, fuel level, mileage,
// Drive and Refuel - checked on a car, because Vehicle itself is abstract.
public static class VehicleTests
{
    private static readonly DateOnly NextYear = DateOnly.FromDateTime(DateTime.Today).AddYears(1);

    private static Car NewCar(string number = "ab-1234", string brand = "Toyota", double fuel = 30,
                              double tank = 50, int year = 2015, int seats = 5) =>
        new(number, brand, "Corolla", fuel, tank, mileage: 15000, fuelConsumptionRate: 6, FuelType.Petrol,
            year, numberOfDoors: 4, seats, SteeringSide.Left, NextYear);

    [Test]
    public static void Constructor_StoresTheFiveRequiredProperties()
    {
        Car car = NewCar();
        Assert.Equal("AB-1234", car.RegistrationNumber, "registration number");
        Assert.Equal("Toyota", car.Brand, "brand");
        Assert.Equal("Corolla", car.Model, "model");
        Assert.Near(30, car.FuelLevel, "fuel level", 1e-9);
        Assert.Near(15000, car.Mileage, "mileage", 1e-9);
    }

    [Test]
    public static void RegistrationNumber_IsTrimmedAndUpperCase()
    {
        Assert.Equal("LV-2024", Vehicle.NormalizeRegistrationNumber("  lv-2024 "), "number");
    }

    [Test]
    public static void RegistrationNumber_WrongLengthOrSymbols_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => Vehicle.NormalizeRegistrationNumber("AB 12"));
        Assert.Throws<ArgumentException>(() => Vehicle.NormalizeRegistrationNumber("A"));
        Assert.Throws<ArgumentException>(() => Vehicle.NormalizeRegistrationNumber("ABCDEFGHIJK"));
    }

    [Test]
    public static void Constructor_WrongData_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => NewCar(brand: "  "));
        Assert.Throws<ArgumentException>(() => NewCar(fuel: 60, tank: 50));
        Assert.Throws<ArgumentException>(() => NewCar(year: 1885));
        Assert.Throws<ArgumentException>(() => NewCar(seats: 10));
    }

    [Test]
    public static void Drive_BurnsFuelAndAddsMileage()
    {
        Car car = NewCar();
        car.Drive(100);
        Assert.Near(24, car.FuelLevel, "fuel after 100 km at 6 L/100 km", 1e-9);
        Assert.Near(15100, car.Mileage, "mileage", 1e-9);
    }

    [Test]
    public static void Drive_WithoutEnoughFuel_ThrowsAndChangesNothing()
    {
        Car car = NewCar(fuel: 3);
        var error = Assert.Throws<VehicleException>(() => car.Drive(100));
        Assert.Contains("Not enough fuel", error.Message);
        Assert.Near(3, car.FuelLevel, "fuel", 1e-9);
        Assert.Near(15000, car.Mileage, "mileage", 1e-9);
    }

    [Test]
    public static void Drive_NotPositiveDistance_IsRejected()
    {
        Car car = NewCar();
        Assert.Throws<ArgumentException>(() => car.Drive(-5));
        Assert.Throws<ArgumentException>(() => car.Drive(double.NaN));
    }

    [Test]
    public static void Refuel_FillsTheTankAndReturnsTheCost()
    {
        Car car = NewCar();
        double cost = car.Refuel(10, 1.65);
        Assert.Near(16.5, cost, "cost", 1e-9);
        Assert.Near(40, car.FuelLevel, "fuel", 1e-9);
    }

    [Test]
    public static void Refuel_MoreThanTheFreeSpace_IsRejected()
    {
        Car car = NewCar();
        Assert.Throws<VehicleException>(() => car.Refuel(25, 1.65));
        Assert.Near(30, car.FuelLevel, "fuel stays", 1e-9);
    }

    [Test]
    public static void Passengers_AddTwoPercentEach_AndSeatsAreLimited()
    {
        Car car = NewCar();
        Assert.Equal(4, car.MaxPassengers, "passengers of a 5-seat car");
        car.BoardPassengers(4);
        Assert.Near(6.48, car.FuelConsumption(100), "L/100 km with 4 passengers", 1e-9);
        Assert.Throws<VehicleException>(() => car.BoardPassengers(1));
        car.DropOffPassengers(1);
        Assert.Near(6.36, car.FuelConsumption(100), "L/100 km with 3 passengers", 1e-9);
    }

    [Test]
    public static void Tax_DependsOnTheYearOfMake()
    {
        Assert.Near(30, NewCar(year: 1998).Tax, "tax before 2000", 1e-9);
        Assert.Near(20, NewCar(year: 2005).Tax, "tax 2000-2009", 1e-9);
        Assert.Near(10, NewCar(year: 2015).Tax, "tax 2010-2019", 1e-9);
        Assert.Near(0, NewCar(year: 2022).Tax, "tax from 2020", 1e-9);
    }

    [Test]
    public static void Car_WithElectricFuel_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => new Car("EV-1", "Nissan", "Leaf", 0, 40, 0, 15, FuelType.Electric,
                                                       2020, 4, 5, SteeringSide.Left, NextYear));
    }

    [Test]
    public static void Vessel_CannotDriveOnTheGround()
    {
        var boat = Demo.Get<MotorBoat>("LV-1001");
        Assert.False(boat.CanDrive, "a boat can drive");
        Assert.Throws<VehicleException>(() => boat.Drive(10));
    }
}
