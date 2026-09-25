using OOPLAB1.Vehicles;
using OOPLAB1.Vehicles.Air;

namespace OOPLAB1;

// Ready-made vehicles to try the menu without typing everything in.
public static class DemoData
{
    public static void AddTo(Fleet fleet)
    {
        fleet.Add(new Car("AB-1234", "Toyota", "Corolla",
            fuelLevel: 32, tankCapacity: 50, mileage: 15000, fuelConsumptionRate: 6,
            fuelType: FuelType.Petrol, developYear: 2015, numberOfDoors: 4, seats: 5));

        fleet.Add(new ElectricCar("EV-2022", "Tesla", "Model 3",
            batteryCharge: 45, batteryCapacity: 75, mileage: 8000, energyConsumptionRate: 15,
            developYear: 2022, numberOfDoors: 4, seats: 5));

        fleet.Add(new Truck("TR-7700", "Iveco", "Daily",
            fuelLevel: 60, tankCapacity: 90, mileage: 120000, fuelConsumptionRate: 12,
            developYear: 2008, cargoCapacity: 2000));

        fleet.Add(new Motorcycle("MC-500", "Honda", "CB500",
            fuelLevel: 10, tankCapacity: 17, mileage: 32000, fuelConsumptionRate: 4.5,
            fuelType: FuelType.Petrol, developYear: 1998, hasSidecar: false));

        // Cessna 172: about 36 L/h at 226 km/h = 16 L/100 km, climbs ~3.7 m/s
        fleet.Add(new Airplane("YL-ABC", "Cessna", "172",
            fuelLevel: 120, tankCapacity: 212, mileage: 150000, fuelConsumptionRate: 16,
            fuelType: FuelType.Petrol, developYear: 2010,
            maxAltitude: 4100, climbRate: 3.7, descentRate: 2.5, cruiseSpeed: 226));
    }
}
