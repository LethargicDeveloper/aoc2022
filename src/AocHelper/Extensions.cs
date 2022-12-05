using System.Runtime.CompilerServices;

namespace AocHelper;

public static class Extensions
{
    public static void Log(this object obj)
        => Console.WriteLine(obj);

    public static void Log(this object obj, string message)
        => Console.WriteLine($"{message}: {obj}");

    public static IEnumerable<string> SplitLines(this string str)
        => str.Split("\r\n");
}