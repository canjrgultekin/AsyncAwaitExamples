using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 6: Task vs ValueTask
///
/// Anahtar kavram: Task her zaman heap allocation yapar.
/// ValueTask, synchronous tamamlanma yolunda allocation'dan kaçınır.
/// Cache hit gibi "çoğunlukla sync tamamlanan" senaryolarda ValueTask kazanır.
/// </summary>
public static class ValueTaskDemo
{
    // Basit in-memory cache simülasyonu
    private static readonly System.Collections.Generic.Dictionary<string, string> _cache = new();

    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 6: Task vs ValueTask — Allocation Farkı");

        ConsoleHelper.WriteExplanation("Task: her çağrıda heap'te yeni nesne allocate edilir.");
        ConsoleHelper.WriteExplanation("ValueTask: sync tamamlanırsa allocation YOK. Hot path'te önemli.");

        // ── TASK İLE CACHE HIT ────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Task<T> — Her zaman heap allocation (cache hit dahil)");
        ConsoleHelper.WriteSeparator();

        int taskAllocations = 0;
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 5; i++)
        {
            var result = await GetWithTaskAsync("user:profile", isCached: true, delayMs: 500);
            taskAllocations++;
            ConsoleHelper.WriteThread($"Task Call {i + 1}", $"Sonuç: {result}");
        }

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("Task (5x cache hit)", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation($"{taskAllocations}x Task nesnesi heap'te allocate edildi (GC baskısı).");

        // ── VALUETASK İLE CACHE HIT ───────────────────────────────────────────
        ConsoleHelper.WriteSection("ValueTask<T> — Sync tamamlanırsa sıfır allocation");
        ConsoleHelper.WriteSeparator();

        int valuetaskAllocations = 0;
        sw.Restart();

        for (int i = 0; i < 5; i++)
        {
            // Cache'e önce yaz (hit senaryosu)
            _cache["product:detail"] = "cached_product_data";

            var result = await FakeWorkSimulator.GetWithCacheAsync("product:detail", isCached: true, delayMs: 500);
            valuetaskAllocations++;
            ConsoleHelper.WriteThread($"ValueTask Call {i + 1}", $"Sonuç: {result}");
        }

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("ValueTask (5x cache hit)", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation("Cache hit = sync path → ValueTask allocation YOK. GC mutlu.");

        // ── CACHE MISS: ASYNC PATH ────────────────────────────────────────────
        ConsoleHelper.WriteSection("ValueTask — Cache MISS (async path, allocation var)");
        ConsoleHelper.WriteSeparator();

        sw.Restart();
        var missResult = await FakeWorkSimulator.GetWithCacheAsync("order:history", isCached: false, delayMs: 400);
        sw.Stop();
        ConsoleHelper.WriteTiming("ValueTask (cache miss, async)", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteInfo($"Sonuç: {missResult}");
        ConsoleHelper.WriteExplanation("Cache miss = async path. Burada Task gibi allocation var. Ama nadir durum.");

        // ── KURALLARI ANLAT ───────────────────────────────────────────────────
        ConsoleHelper.WriteSection("ValueTask Kullanım Kuralları");

        ConsoleHelper.WriteInfo("ValueTask NE ZAMAN kullanılır:");
        ConsoleHelper.WriteInfo("  ✓ Çoğunlukla senkron tamamlanan hot path metodlar");
        ConsoleHelper.WriteInfo("  ✓ Cache hit / early return senaryoları");
        ConsoleHelper.WriteInfo("  ✓ Yüksek frekanslı çağrılan metodlar (loop içi vb.)");
        ConsoleHelper.WriteInfo("  ✓ Framework / library sınır noktaları");
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteInfo("ValueTask NE ZAMAN kullanılmaz:");
        ConsoleHelper.WriteInfo("  ✗ Her zaman async tamamlanan metodlar → Task kullan");
        ConsoleHelper.WriteInfo("  ✗ Birden fazla kez await edilecekse → Task kullan");
        ConsoleHelper.WriteInfo("  ✗ .Result ile erişilecekse → Task kullan");
        ConsoleHelper.WriteInfo("  ✗ Emin değilsen → Task kullan. Premature optimization yapma.");

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• Task: basit, her yerde çalışır, her zaman allocation.");
        ConsoleHelper.WriteInfo("• ValueTask: sync path'te allocation yok, hot path optimizasyonu.");
        ConsoleHelper.WriteInfo("• Default olarak Task kullan. Profiling ile gerekliyse ValueTask geç.");
        ConsoleHelper.WriteInfo("• ValueTask'ı birden fazla await etme → tanımsız davranış!");

        ConsoleHelper.WaitForKey();
    }

    // Task kullanan versiyon — karşılaştırma için
    private static async Task<string> GetWithTaskAsync(string key, bool isCached, int delayMs)
    {
        if (isCached)
        {
            // Cache hit: yine de Task allocate eder!
            return $"[CACHE/Task] {key} = cached_value";
        }

        await Task.Delay(delayMs);
        return $"[DB/Task] {key} = fresh_value";
    }
}
