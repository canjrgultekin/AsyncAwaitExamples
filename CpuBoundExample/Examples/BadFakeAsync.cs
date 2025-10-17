namespace CpuBoundExample.Examples;

/// <summary>
/// ❌ YANLIŞ: Fake async - Task.Run + Thread.Sleep
/// Görünüşte async ama aslında blocking
/// </summary>
public static class BadFakeAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Fake Async (Task.Run + blocking)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Thread.Sleep async yapmaz!\n");

        Console.WriteLine("→ 3 'async' task başlatılıyor...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // ❌ YANLIŞ: Task.Run içinde Thread.Sleep
        var tasks = new[]
        {
            FakeAsyncWork(1),
            FakeAsyncWork(2),
            FakeAsyncWork(3)
        };

        await Task.WhenAll(tasks);
        sw.Stop();

        Console.WriteLine($"\n⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms (paralel)\n");

        Console.WriteLine("⚠️  Sorunlar:");
        Console.WriteLine("  - Thread.Sleep = thread bloke!");
        Console.WriteLine("  - 3 thread 1 saniye boyunca hiçbir şey yapmadan bekler");
        Console.WriteLine("  - 'Async' ama aslında blocking");
        Console.WriteLine("  - Thread pool'dan 3 thread boşa harcanır");
        Console.WriteLine("\n✅ Çözüm: I/O için gerçek async (Task.Delay, HttpClient vs.)");
    }

    // ❌ Fake async: Task.Run içinde Thread.Sleep
    private static Task<string> FakeAsyncWork(int id)
    {
        return Task.Run(() =>
        {
            Console.WriteLine($"  → Task {id} başladı (thread bloke olacak)");
            Thread.Sleep(1000); // ❌ Thread bloke!
            Console.WriteLine($"  ✓ Task {id} bitti (thread 1 sn boşa harcandı)");
            return $"Result{id}";
        });
    }
}
