namespace OOPLAB1.UI;

// Coloured messages in the console.
public static class ConsolePrinter
{
    public static void Header(string text) => WriteLine(text, ConsoleColor.Cyan);

    public static void Success(string text) => WriteLine(text, ConsoleColor.Green);

    public static void Warning(string text) => WriteLine(text, ConsoleColor.Yellow);

    public static void Error(string text) => WriteLine("Error: " + text, ConsoleColor.Red);

    // 2.75 -> "2 h 45 min", 0.5 -> "30 min", 3 -> "3 h"
    public static string FormatHours(double hours)
    {
        int minutes = (int)Math.Round(hours * 60);
        if (minutes < 60)
            return $"{minutes} min";
        return minutes % 60 == 0 ? $"{minutes / 60} h" : $"{minutes / 60} h {minutes % 60} min";
    }

    private static void WriteLine(string text, ConsoleColor color)
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previous;
    }
}
