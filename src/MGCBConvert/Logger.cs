namespace MGCBConvert;

public static class Logger
{
    private static bool _verboseEnabled = false;

    public static void SetVerbose(bool enabled) => _verboseEnabled = enabled;

    public static void LogVerbose(string message)
    {
        if (_verboseEnabled)
        {
            Console.WriteLine($"[VERBOSE] {message}");
        }
    }

    public static void LogInfo(string message) => Console.WriteLine(message);
    public static void LogError(string message) => Console.Error.WriteLine($"[ERROR] {message}");
}
