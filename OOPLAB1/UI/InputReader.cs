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
            if (TryParseDouble(ReadLine(prompt), min, max, out double value))
                return value;
        }
    }

    // Like ReadDouble, but just Enter keeps the default value.
    public static double ReadDoubleOrDefault(string prompt, double min, double max, double defaultValue)
    {
        if (defaultValue < min || defaultValue > max)
            throw new ArgumentOutOfRangeException(nameof(defaultValue), "The default value is out of the limits.");
        while (true)
        {
            string text = ReadLine(prompt);
            if (text.Length == 0)
                return defaultValue;
            if (TryParseDouble(text, min, max, out double value))
                return value;
        }
    }

    // Checks a typed number; prints what is wrong and returns false if it does not fit.
    private static bool TryParseDouble(string text, double min, double max, out double value)
    {
        text = text.Replace(',', '.');
        bool isNumber = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        if (!isNumber || !double.IsFinite(value))
            ConsolePrinter.Warning($"'{text}' is not a number.");
        else if (value < min - Tolerance)
            ConsolePrinter.Warning($"The value must be at least {min:0.##}.");
        else if (value > max + Tolerance)
            ConsolePrinter.Warning($"The value must be at most {max:0.##}.");
        else
        {
            value = Math.Clamp(value, min, max);
            return true;
        }
        return false;
    }

    private static readonly string[] DateFormats = { "yyyy-MM-dd", "dd.MM.yyyy" };

    // Both "2027-05-31" and "31.05.2027" are accepted.
    public static DateOnly ReadDate(string prompt, DateOnly min, DateOnly max)
    {
        while (true)
        {
            string text = ReadLine(prompt);
            if (!DateOnly.TryParseExact(text, DateFormats, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateOnly date))
                ConsolePrinter.Warning($"'{text}' is not a date (yyyy-mm-dd).");
            else if (date < min || date > max)
                ConsolePrinter.Warning($"Enter a date from {min:yyyy-MM-dd} to {max:yyyy-MM-dd}.");
            else
                return date;
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
        int width = options.Count.ToString(CultureInfo.InvariantCulture).Length;   // " 9." and "10." line up
        for (int i = 0; i < options.Count; i++)
            Console.WriteLine($"  {(i + 1).ToString(CultureInfo.InvariantCulture).PadLeft(width)}. {options[i]}");
        if (allowCancel)
            Console.WriteLine($"  {"0".PadLeft(width)}. Cancel");

        int choice = ReadInt("Your choice: ", allowCancel ? 0 : 1, options.Count);
        return choice - 1;
    }
}
