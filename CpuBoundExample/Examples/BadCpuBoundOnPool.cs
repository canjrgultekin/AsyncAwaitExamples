namespace CpuBoundExample.Examples;

/// <summary>
/// ❌ YANLIŞ: CPU-bound işi direkt async method'da çalıştırma
/// Thread pool thread'ini uzun süre meşgul tutar
/// </summary>
public static class BadCpuBoundOnPool
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: CPU-Bound on ThreadPool");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("CPU-bound işi direkt pool thread'inde çalıştırma!\n");

        Console.WriteLine("→ 3 concurrent CPU-bound task başlatılıyor...");
        Console.WriteLine("  (Her biri bir pool thread'i uzun süre kilitlecek)\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        // ❌ YANLIŞ: CPU-bound işler direkt await ediliyor
        var tasks = new[]
        {
            HeavyComputationAsync(1),
            HeavyComputationAsync(2),
            HeavyComputationAsync(3)
        };

        var results = await Task.WhenAll(tasks);
        sw.Stop();

        Console.WriteLine($"✓ Sonuçlar: {string.Join(", ", results)}");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms\n");

        Console.WriteLine("⚠️  Sorunlar:");
        Console.WriteLine("  - Pool thread'leri 3 saniye boyunca bloke");
        Console.WriteLine("  - Başka I/O işleri açlıktan ölür (starvation)");
        Console.WriteLine("  - Async ama thread yine meşgul");
        Console.WriteLine("\n✅ Çözüm: Task.Run veya dedicated thread");

        await Task.CompletedTask;
    }

    // ❌ Bu CPU-bound işi await ediyor ama yine de thread meşgul
    private static async Task<long> HeavyComputationAsync(int id)
    {
        Console.WriteLine($"  → Task {id} başladı (thread pool)");
        
        long sum = 0;
        // CPU-bound: 1 saniye süren hesaplama
        for (int i = 0; i < 100_000_000; i++)
        {
            sum += i;
        }

        await Task.CompletedTask; // Fake async
        Console.WriteLine($"  ✓ Task {id} bitti");
        return sum;
    }
}
