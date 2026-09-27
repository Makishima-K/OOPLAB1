using OOPLAB1.UI;

namespace OOPLAB1.Tests;

// Console input: wrong answers are explained and asked again, Enter keeps a default value,
// the end of the input stops the program calmly. The console is replaced by strings.
public static class InputReaderTests
{
    [Test]
    public static void ReadDouble_AcceptsACommaAfterAMistake()
    {
        var (value, output) = Typed(() => InputReader.ReadDouble("Distance (km): ", 0, 10), "abc", "2,5");
        Assert.Near(2.5, value, "value", 1e-9);
        Assert.Contains("'abc' is not a number.", output);
    }

    [Test]
    public static void ReadDouble_OutOfRange_AsksAgain()
    {
        var (value, output) = Typed(() => InputReader.ReadDouble("Fuel level (L, 0-50): ", 0, 50), "60", "NaN", "30");
        Assert.Near(30, value, "value", 1e-9);
        Assert.Contains("The value must be at most 50.", output);
        Assert.Contains("'NaN' is not a number.", output);
    }

    [Test]
    public static void ReadInt_RejectsOtherNumbersAndText()
    {
        var (value, output) = Typed(() => InputReader.ReadInt("Your choice: ", 0, 8), "9", "x", "3");
        Assert.Equal(3, value, "choice");
        Assert.Contains("Enter a number from 0 to 8.", output);
        Assert.Contains("'x' is not a whole number.", output);
    }

    [Test]
    public static void ReadDoubleOrDefault_EnterKeepsTheDefault()
    {
        var (enter, _) = Typed(() => InputReader.ReadDoubleOrDefault("Wind, Enter = +15: ", -150, 150, 15), "");
        Assert.Near(15, enter, "Enter", 1e-9);
        var (typed, _) = Typed(() => InputReader.ReadDoubleOrDefault("Wind, Enter = +15: ", -150, 150, 15), "-40");
        Assert.Near(-40, typed, "typed", 1e-9);
        Assert.Throws<ArgumentOutOfRangeException>(() => InputReader.ReadDoubleOrDefault("x: ", 0, 10, 20));
    }

    [Test]
    public static void ReadDate_AcceptsTwoFormats()
    {
        var min = new DateOnly(2020, 1, 1);
        var max = new DateOnly(2030, 12, 31);
        var (date, output) = Typed(() => InputReader.ReadDate("Valid until: ", min, max), "2027-13-01", "2035-01-01", "31.12.2027");
        Assert.Equal(new DateOnly(2027, 12, 31), date, "date");
        Assert.Contains("'2027-13-01' is not a date", output);
        Assert.Contains("Enter a date from 2020-01-01 to 2030-12-31.", output);
    }

    [Test]
    public static void ReadYesNo_IgnoresCase()
    {
        Assert.True(Typed(() => InputReader.ReadYesNo("Take off?"), "YES").Result, "YES");
        var (answer, output) = Typed(() => InputReader.ReadYesNo("Take off?"), "maybe", "n");
        Assert.False(answer, "n");
        Assert.Contains("Please answer y or n.", output);
    }

    [Test]
    public static void Choose_ZeroCancels()
    {
        string[] options = { "Fly", "Drive on the ground (taxi)" };
        Assert.Equal(-1, Typed(() => InputReader.Choose("Fly or drive?", options), "0").Result, "0 = cancel");
        Assert.Equal(1, Typed(() => InputReader.Choose("Fly or drive?", options), "2").Result, "second option");
    }

    [Test]
    public static void EndOfInput_Throws()
    {
        // No lines at all: the input ends before the answer
        Assert.Throws<EndOfStreamException>(() => Typed(() => InputReader.ReadLine("Distance (km): ")));
    }

    // Runs the reader with the typed lines and returns its result and everything it printed.
    private static (T Result, string Output) Typed<T>(Func<T> read, params string[] lines)
    {
        TextReader keyboard = Console.In;
        TextWriter screen = Console.Out;
        var output = new StringWriter();
        try
        {
            string input = lines.Length == 0 ? "" : string.Join("\n", lines) + "\n";
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);
            return (read(), output.ToString());
        }
        finally
        {
            Console.SetIn(keyboard);
            Console.SetOut(screen);
        }
    }
}
