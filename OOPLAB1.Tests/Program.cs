// Quick tests of OOPLAB1. No test library is used: a test is a public static method marked
// with [Test]; the runner finds all of them, runs each one and prints PASS or FAIL.
// Run from the solution folder:  dotnet run --project OOPLAB1.Tests
// The exit code is 1 if at least one test fails.

using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace OOPLAB1.Tests;

public static class Program
{
    public static int Main()
    {
        // The same number format as the program itself: "2.5", not "2,5"
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var clock = Stopwatch.StartNew();
        int passed = 0;
        int failed = 0;
        var testClasses = typeof(Program).Assembly.GetTypes()
            .Where(type => type.GetMethods().Any(IsTest))
            .OrderBy(type => type.Name);

        foreach (Type testClass in testClasses)
        {
            Console.WriteLine(testClass.Name);
            // MetadataToken keeps the order in which the tests are written
            foreach (MethodInfo test in testClass.GetMethods().Where(IsTest).OrderBy(m => m.MetadataToken))
            {
                string? error = Run(test);
                if (error == null)
                {
                    passed++;
                    Write("  PASS  ", ConsoleColor.Green);
                    Console.WriteLine(test.Name);
                }
                else
                {
                    failed++;
                    Write("  FAIL  ", ConsoleColor.Red);
                    Console.WriteLine($"{test.Name}\n        {error}");
                }
            }
        }

        Console.WriteLine();
        Write($"{passed + failed} tests: {passed} passed, {failed} failed",
              failed == 0 ? ConsoleColor.Green : ConsoleColor.Red);
        Console.WriteLine($" ({clock.Elapsed.TotalSeconds:F2} s)");
        return failed == 0 ? 0 : 1;
    }

    private static bool IsTest(MethodInfo method) => method.IsStatic && method.IsDefined(typeof(TestAttribute));

    // Returns null when the test passes, otherwise what went wrong.
    private static string? Run(MethodInfo test)
    {
        try
        {
            test.Invoke(null, null);
            return null;
        }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            Exception reason = e.InnerException;
            return reason is TestFailedException ? reason.Message : $"{reason.GetType().Name}: {reason.Message}";
        }
    }

    private static void Write(string text, ConsoleColor color)
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = previous;
    }
}
