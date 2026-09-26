using OOPLAB1.Vehicles;

namespace OOPLAB1;

// Conventional fuel prices: EUR per litre, electricity EUR per kWh.
// Every fuel type starts with a default price; the user can change any of them.
public class FuelPrices
{
    // Every FuelType must have a price here.
    private readonly Dictionary<FuelType, double> _prices = new()
    {
        [FuelType.Petrol] = 1.65,
        [FuelType.Diesel] = 1.55,
        [FuelType.Gas] = 0.85,
        [FuelType.Kerosene] = 1.30,
        [FuelType.Electric] = 0.25,
    };

    public double GetPrice(FuelType fuelType) => _prices[fuelType];

    public void SetPrice(FuelType fuelType, double price)
    {
        if (!double.IsFinite(price) || price < 0)
            throw new ArgumentException("Price must be zero or greater.");
        _prices[fuelType] = price;
    }

    // Electricity is sold per kWh, other fuels per litre.
    public static string UnitOf(FuelType fuelType) => fuelType == FuelType.Electric ? "kWh" : "L";

    // e.g. "1.65 EUR/L"
    public string Describe(FuelType fuelType) => $"{GetPrice(fuelType):F2} EUR/{UnitOf(fuelType)}";
}
