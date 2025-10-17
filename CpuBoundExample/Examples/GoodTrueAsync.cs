namespace CpuBoundExample.Examples;

/// <summary>
/// ✅ DOĞRU: Gerçek async I/O - thread bloke etmez
/// Task.Delay, HttpClient gibi async API'ler kullanır
/// </summary>
public static class GoodTrueAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: True Async I/O");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Gerçek async: Thread bloke olmaz!\n");

        Console.WriteLine("→ 3 async I/O task başlatılıyor...\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // ✅ DOĞRU: Gerçek async I/O
        var tasks = new[]
        {
            TrueAsyncWork(1),
            TrueAsyncWork(2),
            TrueAsyncWork(3)
        };

        await Task.WhenAll(tasks);
        sw.Stop();

        Console.WriteLine($"\n⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms (paralel, ~1 saniye)\n");

        Console.WriteLine("✅ Avantajlar:");
        Console.WriteLine("  - Thread bloke olmadı!");
        Console.WriteLine("  - Task.Delay timer kullanır (IOCP)");
        Console.WriteLine("  - 3 task aynı anda çalışır ama sadece ~0 thread harcanır");
        Console.WriteLine("  - I/O completion notification ile thread pool'a döner");
        Console.WriteLine("\n💡 Gerçek async API'ler:");
        Console.WriteLine("  - Task.Delay, HttpClient.GetAsync, File.ReadAllBytesAsync");
        Console.WriteLine("  - DbContext.ToListAsync, Stream.ReadAsync vs.");
    }

    // ✅ Gerçek async: await Task.Delay (timer-based, thread bloke etmez)
    private static async Task<string> TrueAsyncWork(int id)
    {
        Console.WriteLine($"  → Task {id} başladı (thread serbest bırakılacak)");
        await Task.Delay(1000); // ✅ Non-blocking wait!
        Console.WriteLine($"  ✓ Task {id} bitti (thread idle kaldı, başka işler yaptı)");
        return $"Result{id}";
    }
}
