namespace BackpressureExample.Examples;

/// <summary>
/// ❌ YANLIŞ: Sync over async anti-pattern
/// .Result veya .Wait() ile async metodu senkron bekleme
/// Sonuç: Thread pool starvation, deadlock riski
/// </summary>
public static class BadSyncOverAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Sync Over Async");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Bu kod thread pool'u bloke eder!\n");

        var startTime = DateTime.Now;
        
        // ❌ YANLIŞ: .Result ile senkron bekleme
        try
        {
            Console.WriteLine("→ 3 concurrent request (sync blocking)...");
            
            // Her biri thread'i bloklar
            var result1 = FetchDataAsync(1).Result; // Thread bloke!
            var result2 = FetchDataAsync(2).Result; // Thread bloke!
            var result3 = FetchDataAsync(3).Result; // Thread bloke!
            
            Console.WriteLine($"✓ Sonuçlar: {result1}, {result2}, {result3}");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Hata: {ex.InnerException?.Message}");
        }

        var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
        Console.WriteLine($"\n⏱️  Süre: {elapsed:N0} ms");
        Console.WriteLine("\n⚠️  Sorunlar:");
        Console.WriteLine("  - Thread pool thread'leri boşa bloke edildi");
        Console.WriteLine("  - Yük altında starvation yaratır");
        Console.WriteLine("  - UI context'te deadlock riski");
        Console.WriteLine("  - Exception handling karmaşık (AggregateException)");

        await Task.CompletedTask; // Async signature için
    }

    private static async Task<string> FetchDataAsync(int id)
    {
        // Simulate I/O
        await Task.Delay(500);
        return $"Data{id}";
    }
}
