// OOP laboratory work 1, variant 6 (93166 mod 20 = 6).
// Task: a Vehicle class with registration number, brand, model, fuel level and mileage
// and methods to drive and refuel. Extended with vehicle types and a console menu.

using System.Globalization;
using OOPLAB1.UI;

namespace OOPLAB1;

public static class Program
{
    public static void Main()
    {
        // '.' as the decimal separator on any Windows language
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        ConsolePrinter.Header("OOP Lab 1 - Vehicle fleet (variant 6)");
        var fleet = new Fleet();
        try
        {
            if (InputReader.ReadYesNo("Add demo vehicles?"))
                DemoData.AddTo(fleet);
            new FleetMenu(fleet).Run();
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine();
            Console.WriteLine("The input has ended.");
        }
        Console.WriteLine("Goodbye!");
    }
}
