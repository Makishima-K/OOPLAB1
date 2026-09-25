using OOPLAB1.Vehicles;

namespace OOPLAB1.UI;

// Console menu: prints numbered items, runs the chosen one and repeats until "0".
// If an action fails (not enough fuel, wrong value...), the error is shown and the menu goes on.
public sealed class Menu
{
    private readonly string _title;
    private readonly string _exitTitle;
    private readonly Func<string>? _status;
    private readonly List<MenuItem> _items = new();

    // `status` is an optional line shown under the title, e.g. the number of vehicles.
    public Menu(string title, string exitTitle = "Back", Func<string>? status = null)
    {
        _title = title;
        _exitTitle = exitTitle;
        _status = status;
    }

    public int Count => _items.Count;

    public Menu Add(string title, Action action)
    {
        _items.Add(new MenuItem(title, action));
        return this;
    }

    public void Run()
    {
        while (true)
        {
            Print();
            int choice = InputReader.ReadInt("Your choice: ", 0, _items.Count);
            if (choice == 0)
                return;

            Console.WriteLine();
            Execute(_items[choice - 1]);
        }
    }

    private void Print()
    {
        Console.WriteLine();
        ConsolePrinter.Header($"===== {_title} =====");
        if (_status != null)
            Console.WriteLine(_status());
        for (int i = 0; i < _items.Count; i++)
            Console.WriteLine($"  {i + 1}. {_items[i].Title}");
        Console.WriteLine($"  0. {_exitTitle}");
    }

    private static void Execute(MenuItem item)
    {
        try
        {
            item.Action();
        }
        catch (VehicleException e)
        {
            ConsolePrinter.Error(e.Message);
        }
        catch (ArgumentException e)
        {
            ConsolePrinter.Error(e.Message);
        }
    }
}
