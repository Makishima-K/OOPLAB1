using OOPLAB1.Fuel;

namespace OOPLAB1.Vehicles.Water.Ships;

// Passenger liner. It keeps a 15 % fuel reserve for the sea. It carries one lifeboat for
// every started 50 passengers; each lifeboat adds weight and +0.25 % consumption
// (2000 passengers -> 40 lifeboats -> +10 %).
public class PassengerLiner : WaterVehicle
{
    public const int MaxSeats = 10000;
    public const int PassengersPerLifeboat = 50;
    public const double ConsumptionIncreasePerLifeboat = 0.0025;
    public const double LinerFuelReserve = 0.15;

    public int Seats { get; }

    public PassengerLiner(string registrationNumber, string brand, string model,
                          double fuelLevel, double tankCapacity, double mileage,
                          double fuelConsumptionRate, FuelType fuelType, int developYear,
                          double cruiseSpeed, double draft, int seats)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear, cruiseSpeed, draft)
    {
        if (seats < 1 || seats > MaxSeats)
            throw new ArgumentException($"Number of seats must be from 1 to {MaxSeats}.");
        Seats = seats;
    }

    public override string TypeName => "Passenger liner";

    public override int MaxPassengers => Seats;

    public override double FuelReserve => LinerFuelReserve;

    public int Lifeboats => (int)Math.Ceiling(Passengers / (double)PassengersPerLifeboat);

    public override double FuelConsumption(double distance)
    {
        double lifeboatsFactor = 1 + Lifeboats * ConsumptionIncreasePerLifeboat;
        return base.FuelConsumption(distance) * lifeboatsFactor;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $"\n  Lifeboats: {Lifeboats} (one per {PassengersPerLifeboat} passengers)";
    }
}
