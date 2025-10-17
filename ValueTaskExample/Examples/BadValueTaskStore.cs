namespace ValueTaskExample.Examples;

/// <summary>
/// ❌ YANLIŞ: ValueTask'ı field'da saklamak
/// ValueTask ephemeral (geçici), state'i değişebilir
/// </summary>
public static class BadValueTaskStore
{
    // ❌ ValueTask field'da tutma!
    private static ValueTask<int> _cachedValueTask;

    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Store ValueTask in Field");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("ValueTask'ı field/property'de saklamak tehlikeli!\n");

        Console.WriteLine("→ ValueTask oluşturuluyor ve field'da saklanıyor...");
        _cachedValueTask = GetValueTaskAsync();

        Console.WriteLine("→ İlk kullanım...");
        var result1 = await _cachedValueTask;
        Console.WriteLine($"  Sonuç 1: {result1}");

        await Task.Delay(500);

        Console.WriteLine("→ Daha sonra aynı field'ı kullanma (YANLIŞ!)...");
        try
        {
            var result2 = await _cachedValueTask; // ❌ Stale state!
            Console.WriteLine($"  Sonuç 2: {result2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Hata: {ex.Message}");
        }

        Console.WriteLine("\n⚠️  Sorunlar:");
        Console.WriteLine("  - ValueTask state'i reuse edilebilir");
        Console.WriteLine("  - Field'dan okunan ValueTask stale olabilir");
        Console.WriteLine("  - Unpredictable behavior");
        Console.WriteLine("\n✅ Çözüm: Task kullan veya her seferinde yeni ValueTask");
    }

    private static async ValueTask<int> GetValueTaskAsync()
    {
        await Task.Delay(100);
        return 42;
    }
}
