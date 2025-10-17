namespace CpuBoundExample.Examples;

/// <summary>
/// ✅ DOĞRU: CPU-bound işi Task.Run ile background'a atmak
/// Pool thread'lerini serbest bırakır
/// </summary>
public static class GoodCpuBoundTaskRun
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: CPU-Bound with Task.Run");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Task.Run ile CPU işini dedicated thread'e at\n");

        Console.WriteLine("→ 3 concurrent CPU-bound task (Task.Run ile)...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        // ✅ DOĞRU: Task.Run ile CPU-bound işleri background'a at
        var tasks = new[]
        {
            Task.Run(() => HeavyComputation(1)),
            Task.Run(() => HeavyComputation(2)),
            Task.Run(() => HeavyComputation(3))
        };

        var results = await Task.WhenAll(tasks);
        sw.Stop();

        Console.WriteLine($"✓ Sonuçlar: {string.Join(", ", results)}");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms (paralel)\n");

        Console.WriteLine("✅ Avantajlar:");
        Console.WriteLine("  - Ana thread (veya request thread) serbest");
        Console.WriteLine("  - CPU işleri paralel çalışır");
        Console.WriteLine("  - I/O işleri için pool thread'leri mevcut");
        Console.WriteLine("\n💡 Bonus: Çok uzun işler için TaskCreationOptions.LongRunning");
    }

    private static long HeavyComputation(int id)
    {
        Console.WriteLine($"  → Task {id} başladı (dedicated thread)");
        
        long sum = 0;
        for (int i = 0; i < 100_000_000; i++)
        {
            sum += i;
        }

        Console.WriteLine($"  ✓ Task {id} bitti");
        return sum;
    }
}
