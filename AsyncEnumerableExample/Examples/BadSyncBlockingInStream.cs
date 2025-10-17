using System.Runtime.CompilerServices;

namespace AsyncEnumerableExample.Examples;

/// <summary>
/// ❌ YANLIŞ: IAsyncEnumerable içinde senkron blocking
/// Thread pool'u gereksiz bloklar
/// </summary>
public static class BadSyncBlockingInStream
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Sync Blocking in Async Stream");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Async stream içinde sync blocking!\n");

        Console.WriteLine("→ Streaming 100 records (with blocking)...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int processed = 0;

        await foreach (var item in BadStreamAsync())
        {
            processed++;
            if (processed % 25 == 0)
                Console.WriteLine($"  → Processed {processed}");
        }

        sw.Stop();
        Console.WriteLine($"\n✓ {processed} record işlendi");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms\n");

        Console.WriteLine("⚠️  Sorunlar:");
        Console.WriteLine("  - Her iteration'da Thread.Sleep (100ms × 100 = 10 saniye!)");
        Console.WriteLine("  - Thread pool thread bloke");
        Console.WriteLine("  - Async yararı yok");
        Console.WriteLine("\n✅ Çözüm: await Task.Delay (non-blocking)");
    }

    private static async IAsyncEnumerable<DataItem> BadStreamAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < 100; i++)
        {
            // ❌ YANLIŞ: Thread.Sleep bloklar
            Thread.Sleep(100); // Thread bloke! 
            
            await Task.CompletedTask; // Fake async
            yield return new DataItem(i, $"Data{i}");
        }
    }

    private record DataItem(int Id, string Value);
}
