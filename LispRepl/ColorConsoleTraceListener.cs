using System.Diagnostics;

namespace LispRepl;

internal class ColorConsoleTraceListener (ConsoleColor color, bool useStandardError = false) : ConsoleTraceListener(useStandardError)
{
    public override void WriteLine (string? message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        base.WriteLine(message);
        Console.ForegroundColor = old;
    }
}