using System.Diagnostics;

namespace ValueTaskExample.Examples;

/// <summary>
/// 📊 Performance Comparison: Task vs ValueTask
/// Allocation overhead'i gösterir
/// </summary>
public static class PerformanceComparison
{
    public static async Task RunAsync()
    {
        Console.WriteLine("📊 PERFORMANCE: Task vs ValueTask");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Allocation overhead karşılaştırması\n");

        const int iterations = 100_000;

        // Warm-up
        await RunTaskTest(1000);
        await RunValueTaskTest(1000);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Task test
        Console.WriteLine($"→ Task<int> test ({iterations:N0} iterations)...");
        var taskMemBefore = GC.GetTotalMemory(false);
        var taskSw = Stopwatch.StartNew();
        await RunTaskTest(iterations);
        taskSw.Stop();
        var taskMemAfter = GC.GetTotalMemory(false);
        var taskAlloc = taskMemAfter - taskMemBefore;

        Console.WriteLine($"  Time: {taskSw.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Memory: {taskAlloc / 1024.0:N2} KB allocated\n");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // ValueTask test
        Console.WriteLine($"→ ValueTask<int> test ({iterations:N0} iterations)...");
        var vtMemBefore = GC.GetTotalMemory(false);
        var vtSw = Stopwatch.StartNew();
        await RunValueTaskTest(iterations);
        vtSw.Stop();
        var vtMemAfter = GC.GetTotalMemory(false);
        var vtAlloc = vtMemAfter - vtMemBefore;

        Console.WriteLine($"  Time: {vtSw.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Memory: {vtAlloc / 1024.0:N2} KB allocated\n");

        // Comparison
        var speedup = (double)taskSw.ElapsedMilliseconds / vtSw.ElapsedMilliseconds;
        var memSaving = taskAlloc - vtAlloc;

        Console.WriteLine("📊 Sonuçlar:");
        Console.WriteLine($"  Hız farkı: ValueTask {speedup:F2}x daha hızlı");
        Console.WriteLine($"  Bellek tasarrufu: {memSaving / 1024.0:N2} KB");
        Console.WriteLine($"  Cache hit scenario: ValueTask çok daha verimli");
    }

    private static async Task RunTaskTest(int count)
    {
        var cache = new TaskCache();
        for (int i = 0; i < count; i++)
        {
            // Cache hit senaryosu (hep aynı key)
            await cache.GetAsync("key");
        }
    }

    private static async Task RunValueTaskTest(int count)
    {
        var cache = new ValueTaskCache();
        for (int i = 0; i < count; i++)
        {
            await cache.GetAsync("key");
        }
    }

    private class TaskCache
    {
        private readonly Dictionary<string, int> _data = new() { ["key"] = 42 };

        public Task<int> GetAsync(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                return Task.FromResult(value); // Allocation!
            }
            return Task.FromResult(0);
        }
    }

    private class ValueTaskCache
    {
        private readonly Dictionary<string, int> _data = new() { ["key"] = 42 };

        public ValueTask<int> GetAsync(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                return new ValueTask<int>(value); // 0 allocation
            }
            return new ValueTask<int>(0);
        }
    }
}
