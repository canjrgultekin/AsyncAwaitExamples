namespace ValueTaskExample.Examples;

/// <summary>
/// ✅ DOĞRU: Cache scenario için ValueTask kullanımı
/// Cache hit: Senkron ValueTask (0 allocation)
/// Cache miss: Async path (Task allocation)
/// </summary>
public static class GoodCacheValueTask
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Cache Pattern with ValueTask");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Cache hit: 0 allocation, Cache miss: Task allocation\n");

        var cache = new SmartCache();

        // İlk 10 çağrı (3 unique key)
        for (int i = 0; i < 10; i++)
        {
            var key = $"key{i % 3}";
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = await cache.GetAsync(key);
            sw.Stop();

            Console.WriteLine($"[{i + 1,2}] Key: {key}, Value: {result,3}, " +
                            $"Time: {sw.ElapsedMilliseconds,3}ms");
        }

        Console.WriteLine("\n✅ Pattern:");
        Console.WriteLine("  - Cache hit: ValueTask senkron (0 alloc)");
        Console.WriteLine("  - Cache miss: Task async (allocation)");
        Console.WriteLine("  - Hot path optimize edilmiş");
    }

    private class SmartCache
    {
        private readonly Dictionary<string, int> _data = new();

        public ValueTask<int> GetAsync(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                // ✅ Cache hit: Senkron ValueTask
                return new ValueTask<int>(value); // 0 allocation
            }

            // Cache miss: Async path
            return new ValueTask<int>(FetchFromDbAsync(key));
        }

        private async Task<int> FetchFromDbAsync(string key)
        {
            await Task.Delay(100); // DB simulation
            var value = key.Length * 10;
            _data[key] = value;
            return value;
        }
    }
}
