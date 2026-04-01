using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 2: Sequential Async vs Parallel Async (Task.WhenAll)
///
/// Anahtar kavram: await'leri peş peşe sıralamak işleri PARALELLEŞTIRMEZ.
/// Gerçek paralel I/O için Task.WhenAll kullanılır.
/// Bu demo çoğu junior dev'in düştüğü tuzağı gösterir.
/// </summary>
public static class SequentialVsParallelDemo
{
    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 2: Sequential vs Parallel Async");

        ConsoleHelper.WriteExplanation("5 servis çağrısı, her biri 600ms. Toplam fark dramatik olacak.");

        // ── SEQUENTIAL (YanlıŞ anlayış) ─────────────────────────────────────
        ConsoleHelper.WriteSection("SEQUENTIAL AWAIT (Çoğu junior dev bunu yapıyor)");
        ConsoleHelper.WriteInfo("Her await bir öncekinin bitmesini bekler → Sıralı çalışır.");
        ConsoleHelper.WriteInfo("async yazıyorsun ama aslında sync gibi davranıyorsun!");
        ConsoleHelper.WriteSeparator();

        var sw = Stopwatch.StartNew();

        // Bu klasik hata: async görünümlü ama sequential çalışır
        var s1 = await FakeWorkSimulator.FetchDataAsync("UserService", 600);
        var s2 = await FakeWorkSimulator.FetchDataAsync("OrderService", 600);
        var s3 = await FakeWorkSimulator.FetchDataAsync("ProductService", 600);
        var s4 = await FakeWorkSimulator.FetchDataAsync("PaymentService", 600);
        var s5 = await FakeWorkSimulator.FetchDataAsync("NotificationService", 600);

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("SEQUENTIAL toplam", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation($"5 x 600ms = ~3000ms. Hepsi sırayla bekledi.");

        // ── PARALLEL (Task.WhenAll) ──────────────────────────────────────────
        ConsoleHelper.WriteSection("PARALLEL AWAIT — Task.WhenAll (Doğru yaklaşım)");
        ConsoleHelper.WriteInfo("Tüm Task'lar AYNI ANDA başlatılır, hepsi bitince devam edilir.");
        ConsoleHelper.WriteInfo("Birbirinden bağımsız I/O çağrıları için bu doğru kalıptır.");
        ConsoleHelper.WriteSeparator();

        sw.Restart();

        // Task'ları başlat ama await'leme — hepsi aynı anda başlar
        var t1 = FakeWorkSimulator.FetchDataAsync("UserService", 600);
        var t2 = FakeWorkSimulator.FetchDataAsync("OrderService", 600);
        var t3 = FakeWorkSimulator.FetchDataAsync("ProductService", 600);
        var t4 = FakeWorkSimulator.FetchDataAsync("PaymentService", 600);
        var t5 = FakeWorkSimulator.FetchDataAsync("NotificationService", 600);

        // Hepsini await et — en uzun süren kadar bekler
        var results = await Task.WhenAll(t1, t2, t3, t4, t5);

        sw.Stop();
        ConsoleHelper.WriteSeparator();
        ConsoleHelper.WriteTiming("PARALLEL toplam", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation("~600ms! 5 iş paralel koştu. Zaman tasarrufu: ~2400ms!");

        // ── HYBRID PATTERN ──────────────────────────────────────────────────
        ConsoleHelper.WriteSection("HYBRID: Bazıları sequential, bazıları parallel");
        ConsoleHelper.WriteInfo("Bazen A'nın bitmesi B için gereklidir → sequential zorunlu.");
        ConsoleHelper.WriteInfo("Ama B ve C birbirinden bağımsızsa → parallel yapılabilir.");
        ConsoleHelper.WriteSeparator();

        sw.Restart();

        // Önce user'ı al (gerekli)
        var userId = await FakeWorkSimulator.FetchDataAsync("UserService", 400);
        ConsoleHelper.WriteInfo($"UserId alındı: {userId} → Şimdi parallel devam:");

        // User geldi, ondan sonra bağımsız olan diğerlerini parallel at
        var (orders, products) = await (
            FakeWorkSimulator.FetchDataAsync("OrderService", 500),
            FakeWorkSimulator.FetchDataAsync("ProductService", 500)
        );

        sw.Stop();
        ConsoleHelper.WriteTiming("HYBRID toplam", sw.ElapsedMilliseconds);
        ConsoleHelper.WriteExplanation("400ms (sequential) + 500ms (parallel) = ~900ms. Optimal!");

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• Peş peşe await = sequential. Task.WhenAll = parallel.");
        ConsoleHelper.WriteInfo("• Bağımsız I/O çağrıları için her zaman WhenAll düşün.");
        ConsoleHelper.WriteInfo("• Bir sonrakine bağımlıysa sequential, bağımsızsa parallel yap.");
        ConsoleHelper.WriteInfo("• Task.WhenAny: ilk biten yeterli olduğunda kullan (race pattern).");

        ConsoleHelper.WaitForKey();
    }
}
