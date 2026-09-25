// Variant 93166 mod 20 = 6

// Class Vehicle: def __init__(self, registration_number, brand, model, fuel_level, mileage, fuel_consumption_rate)
// Class Car(Vehicle): def __init__(self, registration_number, brand, model, fuel_level, mileage, fuel_consumption_rate, number_of_doors)
// Class Truck(Vehicle): def __init__(self, registration_number, brand, model, fuel_level, mileage, fuel_consumption_rate, cargo_capacity)
// Class Motorcycle(Vehicle): def __init__(self, registration_number, brand, model, fuel_level, mileage, fuel_consumption_rate, has_sidecar)
// Class ElectricCar(Car): def __init__(self, registration_number, brand, model, fuel_level, mileage, fuel_consumption_rate, number_of_doors, battery_capacity)


// airVehicle // future










public class Vehicle
{



    public string RegistrationNumber { get; }
    public string Brand { get; }
    public string Model { get; }
    public double FuelLevel { get; set; }
    public double Mileage { get; set; }
    public double FuelConsumptionRate { get; }
    public string FuelType { get; }

    public int DevelopYear { get; } = 0;

    public double Tax { get; set; } = 0;


    public Vehicle(string registrationNumber, string brand, string model, double fuelLevel, double mileage, double fuelConsumptionRate, string fuelType, int developYear)
    {
        RegistrationNumber = registrationNumber;
        Brand = brand;
        Model = model;
        FuelLevel = fuelLevel / 2;
        Mileage = mileage;
        FuelConsumptionRate = fuelConsumptionRate;
        FuelType = fuelType;
        DevelopYear = developYear;

        if (developYear < 2000)
        {
            Tax = 30;
        }
        else if (developYear >= 2000 && developYear < 2010)
        {
            Tax = 20;
        }
        else if (developYear >= 2010 && developYear < 2020)
        {
            Tax = 10;
        }
        else
        {
            Tax = 0;
        }


    }

    public virtual double FuelConsumption(double distance)
    {
        return distance * FuelConsumptionRate;
    }


    public virtual void Drive(double distance)
    {
        double fuelNeeded = FuelConsumption(distance);

        if (FuelLevel >= fuelNeeded)
        {
            FuelLevel -= fuelNeeded;
            Mileage += distance;
        }
        else
        {
            Console.WriteLine("Not enough fuel to drive that distance.");
        }
    }


    public virtual void Refuel(double amount)
    {
        FuelLevel += amount;
    }

}
public class Car : Vehicle
{
    public Car(string registrationNumber, string brand, string model, double fuelLevel, double mileage, double fuelConsumptionRate, int numberOfDoors, string fuelType, int developYear, double batteryCharge = 0 )
        : base(registrationNumber, brand, model, fuelLevel, mileage, fuelConsumptionRate, fuelType, developYear)
    {
        NumberOfDoors = numberOfDoors;
    }
    public int NumberOfDoors { get; }



}

public class Truck : Vehicle
{
    // 500 kg => +10l/100km, 1000 kg => +20l/100km, 1500 kg => +30l/100km, 2000 kg => +40l/100km
    public double CargoCapacity { get; set; }
    public double CurrentCargo { get; set; } = 0;

    public Truck(string registrationNumber, string brand, string model, double fuelLevel, double mileage, double fuelConsumptionRate, double cargoCapacity, int developYear)
        : base(registrationNumber, brand, model, fuelLevel, mileage, fuelConsumptionRate, "Diesel", developYear)
    {
        CargoCapacity = cargoCapacity;
    }

    public void LoadCargo(double weight)
    {
        if (CurrentCargo + weight <= CargoCapacity)
        {
            CurrentCargo += weight;
        }
        else
        {
            Console.WriteLine("Cannot load cargo: exceeds capacity.");
        }
    }


    public override double FuelConsumption(double distance)
    {
        double baseConsumption = base.FuelConsumption(distance);
        double extraConsumption = 0;
        if (CurrentCargo > 0)
        {
            extraConsumption = 500 * (CurrentCargo / 500) * (distance / 100);
        }
        return baseConsumption + extraConsumption;
    }

    public override void Drive(double distance)
    {

        double fuelNeeded = FuelConsumption(distance);

        if (FuelLevel >= fuelNeeded)
        {
            FuelLevel -= fuelNeeded;
            Mileage += distance;
        }
        else
        {
            Console.WriteLine("Not enough fuel to drive that distance.");
        }

    }




}

public class Motorcycle : Vehicle
{
    public Motorcycle(string registrationNumber, string brand, string model, double fuelLevel, double mileage, double fuelConsumptionRate, bool hasSidecar, string fuelType, int developYear)
        : base(registrationNumber, brand, model, fuelLevel, mileage, fuelConsumptionRate, fuelType, developYear)
    {
        HasSidecar = hasSidecar;
    }
    public bool HasSidecar { get; }


}

public class ElectricCar : Car
{
    public double BatteryCapacity { get; }
    public double BatteryCharge { get; set; }
    public ElectricCar(string registrationNumber, string brand, string model, double fuelLevel, double mileage, double fuelConsumptionRate, int numberOfDoors, double batteryCapacity, double batteryCharge, int developYear)
        : base(registrationNumber, brand, model, fuelLevel, mileage, fuelConsumptionRate, numberOfDoors, "Electric", developYear, batteryCharge)
    {
        BatteryCapacity = batteryCapacity;
        BatteryCharge = batteryCharge;

    }

    // Method to calculate the time required to charge the battery
    // chargerPower is the power of the charger in kW



    public double timeToCharge(double chargerPower)
    {
        if (chargerPower < 0)
            throw new ArgumentException("Charger power must be positive");
        double freeSpace = BatteryCapacity - BatteryCharge;
        return freeSpace / chargerPower;
    }


    public override void Refuel(double amount)
    {

    }



    private double AddCharge(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        double freeSpace = BatteryCapacity - BatteryCharge;
        double added = Math.Min(amount, freeSpace);
        BatteryCharge += added;
        return added;
    }

    public double Refuel(double amount, double chargerPower, double haveTime)
    {
        if (chargerPower < 0)
            throw new ArgumentException("Charger power must be positive");


        double added = AddCharge(amount);
        double hours = added / chargerPower;

        if (hours > haveTime)
        {
            Console.WriteLine($"Not enough time to charge the battery. You can only charge for {haveTime} hours.");
            hours = haveTime;

        }




        return hours;
    }





}
















public static class Program
{
    public static void Main()
    {





        var vehicles = new List<Vehicle> { };


        while (true)
        {
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Add a vehicle");
            Console.WriteLine("2. Drive a vehicle");
            Console.WriteLine("3. Refuel a vehicle");
            Console.WriteLine("4. Exit");


            var choice = Console.ReadLine();


        }

    }
}
