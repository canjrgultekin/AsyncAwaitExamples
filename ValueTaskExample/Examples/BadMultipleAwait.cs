namespace ValueTaskExample.Examples;

/// <summary>
/// ❌ YANLIŞ: ValueTask'ı birden fazla await etme
/// ValueTask tek seferlik (single-use), ikinci await undefined behavior
/// </summary>
public static class BadMultipleAwait
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Multiple Await ValueTask");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("ValueTask'ı birden fazla await etme!\n");

        var vt = GetValueTaskAsync();

        Console.WriteLine("→ İlk await...");
        var result1 = await vt; // ✓ OK
        Console.WriteLine($"  Sonuç 1: {result1}");

        Console.WriteLine("→ İkinci await (YANLIŞ!)...");
        try
        {
            var result2 = await vt; // ❌ UNDEFINED BEHAVIOR!
            Console.WriteLine($"  Sonuç 2: {result2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Hata: {ex.GetType().Name}");
        }

        Console.WriteLine("\n⚠️  Sorunlar:");
        Console.WriteLine("  - ValueTask tek kullanımlık (single-use)");
        Console.WriteLine("  - İkinci await: undefined behavior");
        Console.WriteLine("  - Stale data okuyabilir");
        Console.WriteLine("  - Exception fırlatabilir");
        Console.WriteLine("\n✅ Çözüm: .AsTask() ile Task'a çevir");
    }

    private static async ValueTask<int> GetValueTaskAsync()
    {
        await Task.Delay(100);
        return 42;
    }
}
