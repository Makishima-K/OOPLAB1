using OOPLAB1.Vehicles;

namespace OOPLAB1;

// All vehicles of the program. Registration numbers in the fleet are unique.
public class Fleet
{
    private readonly List<Vehicle> _vehicles = new();

    // Read-only view: vehicles can be added or removed only through the fleet.
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    public int Count => _vehicles.Count;

    public void Add(Vehicle vehicle)
    {
        if (Contains(vehicle.RegistrationNumber))
            throw new VehicleException(
                $"A vehicle with registration number {vehicle.RegistrationNumber} already exists.");
        _vehicles.Add(vehicle);
    }

    public bool Remove(Vehicle vehicle) => _vehicles.Remove(vehicle);

    public bool Contains(string registrationNumber) => Find(registrationNumber) != null;

    public Vehicle? Find(string registrationNumber)
    {
        string key = registrationNumber.Trim();
        return _vehicles.Find(vehicle =>
            string.Equals(vehicle.RegistrationNumber, key, StringComparison.OrdinalIgnoreCase));
    }
}
