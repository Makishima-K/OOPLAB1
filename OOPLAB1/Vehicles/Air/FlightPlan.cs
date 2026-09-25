namespace OOPLAB1.Vehicles.Air;

// Result of AirVehicle.PlanFlight. Distances in km, altitude in m, fuel in litres.
public sealed record FlightPlan(
    double Distance,          // between the airports, on the ground
    double Altitude,          // cruise altitude
    double ClimbDistance,     // horizontal leg of the left triangle
    double CruiseDistance,    // part flown at one altitude
    double DescentDistance,   // horizontal leg of the right triangle
    double PathLength,        // really flown: two hypotenuses + cruise
    double FlightHours,
    double FuelBurned,        // fuel the flight uses
    double FuelRequired);     // FuelBurned + reserve: must be in the tank before take-off
