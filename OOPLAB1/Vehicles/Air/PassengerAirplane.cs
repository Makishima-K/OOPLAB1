namespace OOPLAB1.Vehicles.Air;

// Passenger airplane: the more people on board, the bigger the fuel reserve for safety.
// Up to 50 passengers 10 %, then +5 % for every started 100 passengers, at most 40 %:
// 51-150 -> 15 %, 151-250 -> 20 %, ..., 551 and more -> 40 %.
public class PassengerAirplane : Airplane
{
    public const int MaxSeats = 900;
    public const int PassengersWithBaseReserve = 50;
    public const int PassengersPerReserveStep = 100;
    public const double ReserveStep = 0.05;
    public const double MaxFuelReserve = 0.4;   // without a limit the reserve could grow endlessly

    public int Seats { get; }

    public PassengerAirplane(string registrationNumber, string brand, string model,
                             double fuelLevel, double tankCapacity, double mileage,
                             double fuelConsumptionRate, FuelType fuelType, int developYear,
                             double maxAltitude, double climbRate, double descentRate,
                             double cruiseSpeed, int seats)
        : base(registrationNumber, brand, model, fuelLevel, tankCapacity, mileage,
               fuelConsumptionRate, fuelType, developYear,
               maxAltitude, climbRate, descentRate, cruiseSpeed)
    {
        if (seats < 1 || seats > MaxSeats)
            throw new ArgumentException($"Number of seats must be from 1 to {MaxSeats}.");
        Seats = seats;
    }

    public override string TypeName => "Passenger plane";

    public override int MaxPassengers => Seats;

    public override double FuelReserve
    {
        get
        {
            int extraPassengers = Math.Max(0, Passengers - PassengersWithBaseReserve);
            int steps = (int)Math.Ceiling(extraPassengers / (double)PassengersPerReserveStep);
            return Math.Min(MaxFuelReserve, DefaultFuelReserve + steps * ReserveStep);
        }
    }
}
