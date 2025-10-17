namespace BackpressureExample.Examples;

/// <summary>
/// ✅ DOĞRU: Tam async zincir
/// await ile tüm zincir async, thread bloke etmez
/// </summary>
public static class GoodFullAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Full Async Chain");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Thread'ler bloke edilmez, async I/O kullanılır\n");

        var startTime = DateTime.Now;
        
        Console.WriteLine("→ 3 concurrent request (async, paralel)...");
        
        // ✅ DOĞRU: Task.WhenAll ile paralel async
        var tasks = new[]
        {
            FetchDataAsync(1),
            FetchDataAsync(2),
            FetchDataAsync(3)
        };

        var results = await Task.WhenAll(tasks);
        
        Console.WriteLine($"✓ Sonuçlar: {string.Join(", ", results)}");

        var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
        Console.WriteLine($"\n⏱️  Süre: {elapsed:N0} ms (paralel, ~500ms)");
        Console.WriteLine("\n✅ Avantajlar:");
        Console.WriteLine("  - Thread'ler bloke olmadı");
        Console.WriteLine("  - Paralel çalıştı (3 task aynı anda)");
        Console.WriteLine("  - Thread pool verimli kullanıldı");
        Console.WriteLine("  - Exception handling temiz");
    }

    private static async Task<string> FetchDataAsync(int id)
    {
        await Task.Delay(500);
        return $"Data{id}";
    }
}
