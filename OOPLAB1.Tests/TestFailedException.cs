namespace OOPLAB1.Tests;

// Thrown by the checks in Assert when a test fails.
public sealed class TestFailedException(string message) : Exception(message);
