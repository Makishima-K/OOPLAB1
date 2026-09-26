namespace OOPLAB1.Vehicles.Water;

// Result of WaterVehicle.PlanVoyage. Distances in km, speeds in km/h, depths in m, fuel in litres.
public sealed record VoyagePlan(
    double Distance,            // on the map, between the ports
    double Current,             // + with the current, - against
    double SpeedThroughWater,
    double GroundSpeed,         // speed through the water + current
    double WaterDistance,       // way through the water: longer against the current
    double Hours,
    double FuelBurned,          // fuel the voyage uses
    double FuelRequired,        // FuelBurned + reserve: must be on board before leaving
    double Draft,
    double RouteDepth);
