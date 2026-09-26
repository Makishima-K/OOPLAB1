using OOPLAB1.Fuel;
using OOPLAB1.Vehicles.Air.Balloons;
using OOPLAB1.Vehicles.Air.Helicopters;
using OOPLAB1.Vehicles.Air.Planes;
using OOPLAB1.Vehicles.Ground;
using OOPLAB1.Vehicles.Ground.Cars;
using OOPLAB1.Vehicles.Water.Boats;
using OOPLAB1.Vehicles.Water.Ships;

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
        fleet.Add(new LightAirplane("YL-ABC", "Cessna", "172",
            fuelLevel: 120, tankCapacity: 212, mileage: 150000, fuelConsumptionRate: 16,
            fuelType: FuelType.Petrol, developYear: 2010,
            maxAltitude: 4100, climbRate: 3.7, descentRate: 2.5, cruiseSpeed: 226));

        // Airbus A220-300: about 2500 L/h at 830 km/h = 300 L/100 km
        fleet.Add(new PassengerAirplane("YL-PAX", "Airbus", "A220-300",
            fuelLevel: 15000, tankCapacity: 21500, mileage: 3_000_000, fuelConsumptionRate: 300,
            fuelType: FuelType.Kerosene, developYear: 2020,
            maxAltitude: 12500, climbRate: 12, descentRate: 10, cruiseSpeed: 830, seats: 149));

        fleet.Add(new CargoAirplane("YL-CRG", "Boeing", "737-800BCF",
            fuelLevel: 20000, tankCapacity: 26000, mileage: 5_000_000, fuelConsumptionRate: 370,
            fuelType: FuelType.Kerosene, developYear: 2008,
            maxAltitude: 12500, climbRate: 10, descentRate: 10, cruiseSpeed: 800, cargoCapacity: 23000));

        // The burner uses about 60 L of propane per hour; with a 15 km/h wind = 400 L/100 km
        fleet.Add(new Balloon("YL-BAL", "Cameron", "Z-105",
            fuelLevel: 120, tankCapacity: 160, mileage: 2000, fuelConsumptionRate: 400,
            developYear: 2018, maxAltitude: 3000, climbRate: 2.5, descentRate: 2, typicalWind: 15));

        // Zeppelin NT: three petrol engines, about 60 L/h at 80 km/h = 75 L/100 km
        fleet.Add(new Airship("YL-ZEP", "Zeppelin", "NT",
            fuelLevel: 500, tankCapacity: 800, mileage: 40000, fuelConsumptionRate: 75,
            developYear: 2015, maxAltitude: 2600, climbRate: 3, descentRate: 2, cruiseSpeed: 80));

        // Robinson R44: about 57 L/h at 200 km/h = 28.5 L/100 km; slow vertical landing
        fleet.Add(new Helicopter("YL-HEL", "Robinson", "R44",
            fuelLevel: 150, tankCapacity: 180, mileage: 50000, fuelConsumptionRate: 28.5,
            fuelType: FuelType.Petrol, developYear: 2015,
            maxAltitude: 4200, climbRate: 5, descentRate: 2.5, cruiseSpeed: 200, maxClimbAngle: 30));

        // Airbus H215: about 600 L/h of jet fuel at 260 km/h = 230 L/100 km
        fleet.Add(new CargoHelicopter("YL-HCL", "Airbus", "H215",
            fuelLevel: 2000, tankCapacity: 2600, mileage: 300000, fuelConsumptionRate: 230,
            fuelType: FuelType.Kerosene, developYear: 2012,
            maxAltitude: 6000, climbRate: 7, descentRate: 2.5, cruiseSpeed: 260, maxClimbAngle: 30,
            cargoCapacity: 2500, maxSlingLoad: 4500));

        fleet.Add(new MotorBoat("LV-1001", "Buster", "XL",
            fuelLevel: 100, tankCapacity: 150, mileage: 1200, fuelConsumptionRate: 67,
            fuelType: FuelType.Petrol, developYear: 2019, cruiseSpeed: 45, draft: 0.4));

        // Coaster: 3850 t of cargo, about 6000 L of diesel per day at 22 km/h = 1140 L/100 km
        fleet.Add(new CargoShip("LV-CARGO1", "Damen", "Combi Coaster 3850",
            fuelLevel: 150_000, tankCapacity: 200_000, mileage: 800_000, fuelConsumptionRate: 1140,
            fuelType: FuelType.Diesel, developYear: 2016, cruiseSpeed: 22,
            emptyDraft: 3.0, loadedDraft: 5.5, cargoCapacity: 3_850_000));

        fleet.Add(new PassengerLiner("LV-LINER1", "Meyer Werft", "MW-2000",
            fuelLevel: 1_000_000, tankCapacity: 1_500_000, mileage: 2_000_000, fuelConsumptionRate: 9000,
            fuelType: FuelType.Diesel, developYear: 2008, cruiseSpeed: 40, draft: 6.8, seats: 2000));

        // Sailing yacht with a small diesel engine: about 4 L/h at 12 km/h = 33 L/100 km
        fleet.Add(new Sailboat("LV-SAIL1", "Bavaria", "C42",
            fuelLevel: 150, tankCapacity: 210, mileage: 5000, fuelConsumptionRate: 33,
            fuelType: FuelType.Diesel, developYear: 2021, engineSpeed: 12, draft: 2.1));
    }
}
