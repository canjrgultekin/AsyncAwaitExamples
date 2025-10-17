using System.Runtime.CompilerServices;

namespace AsyncEnumerableExample.Examples;

/// <summary>
/// ✅ DOĞRU: IAsyncEnumerable ile streaming
/// Memory efficient, lazy loading
/// </summary>
public static class GoodStreamData
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Stream with IAsyncEnumerable");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Lazy loading: Sadece current item memory'de\n");

        Console.WriteLine("→ Streaming 10,000 records...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var memBefore = GC.GetTotalMemory(false);

        int processed = 0;
        // ✅ await foreach: Her seferinde bir item
        await foreach (var item in StreamDataAsync())
        {
            // Process (simulated)
            processed++;
            
            if (processed % 2000 == 0)
                Console.WriteLine($"  → Processed {processed:N0} records");
        }

        var memAfter = GC.GetTotalMemory(false);
        var memUsed = (memAfter - memBefore) / 1024.0;

        sw.Stop();
        Console.WriteLine($"\n✓ {processed:N0} record işlendi");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Memory: {memUsed:N2} KB (sabit!)\n");

        Console.WriteLine("✅ Avantajlar:");
        Console.WriteLine("  - Memory kullanımı sabit (~current batch size)");
        Console.WriteLine("  - İlk item hemen işlenmeye başlar (low latency)");
        Console.WriteLine("  - Milyon+ record için güvenli");
        Console.WriteLine("  - Backpressure doğal (consumer hızı kontrol eder)");
    }

    private static async IAsyncEnumerable<DataItem> StreamDataAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < 10_000; i++)
        {
            // Simulate DB fetch
            if (i % 100 == 0)
                await Task.Delay(10, ct);
            
            yield return new DataItem(i, $"Data{i}");
        }
    }

    private record DataItem(int Id, string Value);
}
