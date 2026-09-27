using OOPLAB1.Vehicles;

namespace OOPLAB1.Tests;

// Fresh demo vehicles for every test: the same objects as in the program's demo data, so the
// numbers in the tests are the numbers that were checked by hand in the console.
public static class Demo
{
    public static Fleet NewFleet()
    {
        var fleet = new Fleet();
        DemoData.AddTo(fleet);
        return fleet;
    }

    public static T Get<T>(string registrationNumber) where T : Vehicle
    {
        Vehicle? vehicle = NewFleet().Find(registrationNumber);
        return vehicle as T ?? throw new TestFailedException($"no demo {typeof(T).Name} {registrationNumber}");
    }
}
