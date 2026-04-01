using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Helpers;

/// <summary>
/// Gerçek hayattaki I/O operasyonlarını simüle eder.
/// Database sorgusu, HTTP isteği, dosya okuma gibi düşün.
/// </summary>
public static class FakeWorkSimulator
{
    /// <summary>
    /// SYNC versiyon: Thread'i tamamen bloke eder.
    /// Tıpkı Thread.Sleep gibi - bu thread hiçbir iş yapamaz, bekler.
    /// </summary>
    public static string FetchDataSync(string source, int delayMs)
    {
        ConsoleHelper.WriteThread(source, $"SYNC başladı → Thread bloke ediliyor ({delayMs}ms)...");
        Thread.Sleep(delayMs); // Thread tamamen donuyor!
        ConsoleHelper.WriteThread(source, $"SYNC bitti → Veri hazır");
        return $"[{source}] → Sonuç";
    }

    /// <summary>
    /// ASYNC versiyon: Thread'i bloke etmez, I/O beklerken ThreadPool başka iş yapabilir.
    /// </summary>
    public static async Task<string> FetchDataAsync(string source, int delayMs, CancellationToken ct = default)
    {
        ConsoleHelper.WriteThread(source, $"ASYNC başladı → Thread serbest kalıyor ({delayMs}ms I/O bekleniyor)...");
        await Task.Delay(delayMs, ct); // Thread bloke DEĞİL, devam eder
        ConsoleHelper.WriteThread(source, $"ASYNC bitti → Continuation farklı thread'de çalışabilir");
        return $"[{source}] → Sonuç";
    }

    /// <summary>
    /// CancellationToken destekli uzun süren iş simülasyonu.
    /// </summary>
    public static async Task<string> LongRunningWorkAsync(string name, int totalMs, CancellationToken ct)
    {
        int steps = 10;
        int stepMs = totalMs / steps;

        for (int i = 1; i <= steps; i++)
        {
            ct.ThrowIfCancellationRequested(); // Her adımda cancel kontrolü
            await Task.Delay(stepMs, ct);
            ConsoleHelper.WriteThread(name, $"Adım {i}/{steps} tamamlandı");
        }

        return $"[{name}] tamamlandı";
    }

    /// <summary>
    /// Hata fırlatacak şekilde tasarlanmış async iş.
    /// </summary>
    public static async Task<string> FailingWorkAsync(string name, int delayMs, bool shouldFail)
    {
        await Task.Delay(delayMs);

        if (shouldFail)
            throw new InvalidOperationException($"'{name}' servisi başarısız oldu! (Simüle edilmiş hata)");

        return $"[{name}] → Başarılı";
    }

    /// <summary>
    /// ValueTask demo için: cache'de varsa sync döner, yoksa async fetch yapar.
    /// </summary>
    public static async ValueTask<string> GetWithCacheAsync(string key, bool isCached, int delayMs)
    {
        if (isCached)
        {
            // Cache hit: async overhead yok, direkt dön (ValueTask burada parlıyor)
            ConsoleHelper.WriteThread(key, "Cache HIT → Async overhead YOK, direkt dönüş");
            return $"[CACHE] {key} = cached_value_42";
        }

        // Cache miss: gerçek async I/O
        ConsoleHelper.WriteThread(key, "Cache MISS → Async I/O başlatılıyor...");
        await Task.Delay(delayMs);
        ConsoleHelper.WriteThread(key, "Cache MISS → Veri çekildi, cache'e yazıldı");
        return $"[DB] {key} = fresh_value_99";
    }
}
