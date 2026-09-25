using System.Globalization;

namespace OOPLAB1.UI;

// Reads values from the console and asks again until the input is valid.
public static class InputReader
{
    private const double Tolerance = 1e-9;

    public static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();
        if (line == null)
            throw new EndOfStreamException("The input has ended.");
        if (Console.IsInputRedirected)
            Console.WriteLine(line);   // show the answer when the input comes from a file
        return line.Trim();
    }

    public static string ReadText(string prompt)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (text.Length > 0)
                return text;
            ConsolePrinter.Warning("Please enter a value.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                ConsolePrinter.Warning($"'{text}' is not a whole number.");
            else if (value < min || value > max)
                ConsolePrinter.Warning($"Enter a number from {min} to {max}.");
            else
                return value;
        }
    }

    // Both "2.5" and "2,5" are accepted.
    public static double ReadDouble(string prompt, double min, double max = double.MaxValue)
    {
        while (true)
        {
            string text = ReadLine(prompt).Replace(',', '.');
            bool isNumber = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture,
                                            out double value);
            if (!isNumber || !double.IsFinite(value))
                ConsolePrinter.Warning($"'{text}' is not a number.");
            else if (value < min - Tolerance)
                ConsolePrinter.Warning($"The value must be at least {min:0.##}.");
            else if (value > max + Tolerance)
                ConsolePrinter.Warning($"The value must be at most {max:0.##}.");
            else
                return Math.Clamp(value, min, max);
        }
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            string answer = ReadLine($"{prompt} (y/n): ").ToLowerInvariant();
            if (answer is "y" or "yes")
                return true;
            if (answer is "n" or "no")
                return false;
            ConsolePrinter.Warning("Please answer y or n.");
        }
    }

    // Prints a numbered list and returns the index of the chosen option (-1 = cancel).
    public static int Choose(string title, IReadOnlyList<string> options, bool allowCancel = true)
    {
        Console.WriteLine(title);
        for (int i = 0; i < options.Count; i++)
            Console.WriteLine($"  {i + 1}. {options[i]}");
        if (allowCancel)
            Console.WriteLine("  0. Cancel");

        int choice = ReadInt("Your choice: ", allowCancel ? 0 : 1, options.Count);
        return choice - 1;
    }
}
