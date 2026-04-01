using System;

namespace AsyncAwaitExample.Helpers;

public static class ConsoleHelper
{
    private static readonly object _lock = new();

    public static void WriteHeader(string title)
    {
        lock (_lock)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {title.PadRight(56)}║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
    }

    public static void WriteSection(string title)
    {
        lock (_lock)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ─── {title} ───");
            Console.ResetColor();
        }
    }

    public static void WriteInfo(string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }
    }

    public static void WriteThread(string label, string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"  [Thread:{Environment.CurrentManagedThreadId:D2}] ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"[{label}] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }
    }

    public static void WriteSuccess(string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {message}");
            Console.ResetColor();
        }
    }

    public static void WriteError(string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {message}");
            Console.ResetColor();
        }
    }

    public static void WriteWarning(string message)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  ⚠ {message}");
            Console.ResetColor();
        }
    }

    public static void WriteTiming(string label, long ms)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  ⏱ {label}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{ms} ms");
            Console.ResetColor();
        }
    }

    public static void WriteExplanation(string explanation)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  💡 {explanation}");
            Console.ResetColor();
        }
    }

    public static void WriteSeparator()
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('─', 56));
            Console.ResetColor();
        }
    }

    public static void WaitForKey()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  [Enter'a basarak devam et]");
        Console.ResetColor();
        Console.ReadLine();
    }
}
