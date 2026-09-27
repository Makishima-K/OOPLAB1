namespace OOPLAB1.Tests;

// Checks used by the tests. Each one throws TestFailedException with a clear message.
public static class Assert
{
    public static void Equal<T>(T expected, T actual, string what)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new TestFailedException($"{what}: expected {expected}, got {actual}");
    }

    // Doubles are compared with a tolerance: the program prints them rounded to 0.1.
    public static void Near(double expected, double actual, string what, double tolerance = 0.05)
    {
        if (!(Math.Abs(expected - actual) <= tolerance))
            throw new TestFailedException($"{what}: expected {expected} ± {tolerance}, got {actual}");
    }

    public static void True(bool condition, string what)
    {
        if (!condition)
            throw new TestFailedException($"expected: {what}");
    }

    public static void False(bool condition, string what)
    {
        if (condition)
            throw new TestFailedException($"not expected: {what}");
    }

    public static void Contains(string expectedPart, string text)
    {
        if (!text.Contains(expectedPart, StringComparison.Ordinal))
            throw new TestFailedException($"expected the text to contain \"{expectedPart}\", got \"{text}\"");
    }

    // Runs the action and returns the exception of the expected type; fails if there is none.
    public static TException Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException e)
        {
            return e;
        }
        catch (Exception e)
        {
            throw new TestFailedException($"expected {typeof(TException).Name}, got {e.GetType().Name}: {e.Message}");
        }
        throw new TestFailedException($"expected {typeof(TException).Name}, but nothing was thrown");
    }
}
