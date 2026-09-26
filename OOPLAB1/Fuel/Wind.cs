namespace OOPLAB1.Fuel;

// Average wind speed, km/h: the free "fuel" of balloons and sails, a help or a hindrance
// for every aircraft. It starts with a default value; the user can change it like a fuel price.
// Flights and voyages offer it as the default and may use another value for one trip.
public class Wind
{
    public const double DefaultSpeed = 15;   // a gentle breeze
    public const double MaxSpeed = 150;      // a hurricane

    public double Speed { get; private set; } = DefaultSpeed;

    public void SetSpeed(double speed)
    {
        if (!double.IsFinite(speed) || speed < 0 || speed > MaxSpeed)
            throw new ArgumentException($"Wind speed must be from 0 to {MaxSpeed} km/h.");
        Speed = speed;
    }

    // e.g. "15 km/h"
    public string Describe() => $"{Speed:0.#} km/h";
}
