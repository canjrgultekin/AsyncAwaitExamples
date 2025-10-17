using System.Runtime.CompilerServices;

namespace AsyncEnumerableExample.Examples;

/// <summary>
/// ✅ DOĞRU: Batch processing ile network round-trip azaltma
/// Streaming + batching kombinasyonu
/// </summary>
public static class GoodBatchProcessing
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Batch Processing");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Streaming + batching: Best of both worlds\n");

        Console.WriteLine("→ Streaming 1,000 records in batches of 100...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int totalProcessed = 0;
        int batchCount = 0;

        await foreach (var batch in StreamBatchesAsync())
        {
            batchCount++;
            totalProcessed += batch.Count;
            Console.WriteLine($"  → Batch {batchCount}: {batch.Count} items (Total: {totalProcessed})");
            
            // Simulate batch processing
            await Task.Delay(50);
        }

        sw.Stop();
        Console.WriteLine($"\n✓ {totalProcessed:N0} record işlendi ({batchCount} batch)");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms\n");

        Console.WriteLine("✅ Avantajlar:");
        Console.WriteLine("  - Network round-trip azaltma (1000 call yerine 10)");
        Console.WriteLine("  - Memory verimli (sadece current batch)");
        Console.WriteLine("  - Throughput optimization");
        Console.WriteLine("\n💡 Use case: DB pagination, API cursor-based fetch");
    }

    private static async IAsyncEnumerable<List<DataItem>> StreamBatchesAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        const int totalItems = 1000;
        const int batchSize = 100;
        int offset = 0;

        while (offset < totalItems && !ct.IsCancellationRequested)
        {
            // Simulate DB fetch (batch)
            await Task.Delay(100, ct);

            var batch = new List<DataItem>();
            for (int i = 0; i < batchSize && offset < totalItems; i++, offset++)
            {
                batch.Add(new DataItem(offset, $"Data{offset}"));
            }

            yield return batch;
        }
    }

    private record DataItem(int Id, string Value);
}
