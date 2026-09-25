namespace OOPLAB1.Vehicles;

// Thrown when an operation is impossible in the current state of a vehicle:
// not enough fuel, the tank is full, the cargo does not fit, and so on.
public class VehicleException : Exception
{
    public VehicleException(string message) : base(message)
    {
    }
}
