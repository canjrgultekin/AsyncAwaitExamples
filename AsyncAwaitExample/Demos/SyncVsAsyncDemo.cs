using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 1: Sync vs Async - Temel fark nedir?
///
/// Anahtar kavram: async/await thread'i BLOKE ETMEZ.
/// Sync kod thread'i kilitlerek bekler. Async kod thread'i serbest bırakır,
/// I/O bitince continuation devam eder (büyük ihtimalle farklı thread'de).
/// </summary>
public static class SyncVsAsyncDemo
{
    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 1: Sync vs Async - Thread Bloklama Farkı");

        ConsoleHelper.WriteExplanation("3 servis çağrısı yapıyoruz: Database, Cache, ExternalAPI");
        ConsoleHelper.WriteExplanation("Her servis 800ms sürüyor. Toplam beklenen süre farkını gözlemle.");

        // ── SYNC BÖLÜM ──────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("SYNC YAKLAŞIM (Thread bloke oluyor)");

        ConsoleHelper.WriteInfo("Thread ID'lere dikkat et → Hep AYNI thread çalışıyor.");
        ConsoleHelper.WriteInfo("Thread beklerken hiçbir iş yapamıyor, ThreadPool yoruluyor.");
        ConsoleHelper.WriteSeparator();

        var sw = Stopwatch.StartNew();

        var r1 = FakeWorkSimulator.FetchDataSync("Database", 800);
        var r2 = FakeWorkSimulator.FetchDataSync("Cache", 800);
        var r3 = FakeWorkSimulator.FetchDataSync("ExternalAPI", 800);

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("SYNC toplam süre", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation("3 x 800ms = ~2400ms. Thread boyunca bloke kaldı.");

        // ── ASYNC BÖLÜM ─────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("ASYNC YAKLAŞIM (Sıralı ama thread bloke etmiyor)");

        ConsoleHelper.WriteInfo("await sırasında thread, ThreadPool'a GERİ DÖNER.");
        ConsoleHelper.WriteInfo("I/O tamamlanınca continuation devam eder (farklı thread olabilir!).");
        ConsoleHelper.WriteSeparator();

        sw.Restart();

        var a1 = await FakeWorkSimulator.FetchDataAsync("Database", 800);
        var a2 = await FakeWorkSimulator.FetchDataAsync("Cache", 800);
        var a3 = await FakeWorkSimulator.FetchDataAsync("ExternalAPI", 800);

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("ASYNC (sequential) toplam süre", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation("Süre benzer (~2400ms) AMMA thread hiç bloke olmadı!");
        ConsoleHelper.WriteExplanation("Fark: Scalability. 1000 concurrent req'te sync çöker, async çalışır.");

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• async/await HIZLANDIRMAZ, thread'i SERBEST BIRAKIR.");
        ConsoleHelper.WriteInfo("• Thread.Sleep = thread bloke. Task.Delay = thread serbest.");
        ConsoleHelper.WriteInfo("• Web uygulamalarında async → daha fazla concurrent istek karşılanır.");
        ConsoleHelper.WriteInfo("• Sadece await sıralı çalıştırmak yetmez → Demo 2'ye bak (WhenAll).");

        ConsoleHelper.WaitForKey();
    }
}
