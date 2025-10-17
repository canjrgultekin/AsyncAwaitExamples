using System.Runtime.CompilerServices;

namespace AsyncEnumerableExample.Examples;

/// <summary>
/// ✅ BEST PRACTICE: Cancellation support
/// Graceful shutdown, responsive to cancellation
/// </summary>
public static class BestPracticeCancellation
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ BEST PRACTICE: Cancellation Support");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("CancellationToken ile graceful shutdown\n");

        Console.WriteLine("→ Stream başlatılıyor (5 saniye sürecek)");
        Console.WriteLine("⌨️  'q' tuşuna basarak durdurun...\n");

        var cts = new CancellationTokenSource();

        // Background'da 'q' dinle
        _ = Task.Run(() =>
        {
            if (Console.ReadKey(true).KeyChar == 'q')
            {
                Console.WriteLine("\n→ Cancellation istendi...");
                cts.Cancel();
            }
        });

        try
        {
            int processed = 0;
            // ✅ WithCancellation ile token inject et
            await foreach (var item in StreamWithCancellationAsync()
                .WithCancellation(cts.Token))
            {
                processed++;
                Console.WriteLine($"  → Item {item.Id}: {item.Value}");
                await Task.Delay(500, cts.Token);
            }

            Console.WriteLine($"\n✓ Stream tamamlandı: {processed} item");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("✓ Stream iptal edildi (graceful)");
        }

        Console.WriteLine("\n✅ Best Practices:");
        Console.WriteLine("  - [EnumeratorCancellation] attribute kullan");
        Console.WriteLine("  - WithCancellation() ile token inject et");
        Console.WriteLine("  - OperationCanceledException handle et");
        Console.WriteLine("  - Partial result'lar valid");
    }

    // ✅ [EnumeratorCancellation] ile automatic token binding
    private static async IAsyncEnumerable<DataItem> StreamWithCancellationAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < 10; i++)
        {
            // Check cancellation
            ct.ThrowIfCancellationRequested();

            await Task.Delay(100, ct);
            yield return new DataItem(i, $"Data{i}");
        }
    }

    private record DataItem(int Id, string Value);
}
