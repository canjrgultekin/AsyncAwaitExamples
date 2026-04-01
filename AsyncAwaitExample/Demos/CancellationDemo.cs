using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 3: CancellationToken - İptal Mekanizması
///
/// Anahtar kavram: Uzun süren async işleri yarıda kesmek için
/// CancellationToken kullanılır. Her async metodun bunu desteklemesi gerekir.
/// "Fire and forget" değil, kontrol altında çalışma prensibi.
/// </summary>
public static class CancellationDemo
{
    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 3: CancellationToken — İptal Mekanizması");

        // ── SENARYO 1: Timeout tabanlı iptal ────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo A: Timeout ile otomatik iptal (2 sn timeout, 5 sn iş)");
        ConsoleHelper.WriteExplanation("CancellationTokenSource'a bir timeout veriyoruz.");
        ConsoleHelper.WriteExplanation("İş 5sn sürecek ama 2sn'de otomatik iptal edilecek.");
        ConsoleHelper.WriteSeparator();

        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        try
        {
            var sw = Stopwatch.StartNew();
            var result = await FakeWorkSimulator.LongRunningWorkAsync(
                "TimeoutJob", totalMs: 5000, ct: timeoutCts.Token);
            ConsoleHelper.WriteSuccess($"Tamamlandı: {result}");
        }
        catch (OperationCanceledException)
        {
            ConsoleHelper.WriteWarning("İş iptal edildi → CancellationToken tetiklendi (timeout).");
            ConsoleHelper.WriteExplanation("OperationCanceledException beklenen bir durumdur, exception değil!");
        }

        // ── SENARYO 2: Manuel iptal ──────────────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo B: Manuel iptal (1.5sn sonra Cancel çağırıyoruz)");
        ConsoleHelper.WriteExplanation("Gerçek hayatta: kullanıcı sayfadan çıktı, HTTP request kesildi.");
        ConsoleHelper.WriteSeparator();

        using var manualCts = new CancellationTokenSource();

        // Ayrı bir task ile 1.5sn sonra cancel et
        var canceller = Task.Run(async () =>
        {
            await Task.Delay(1500);
            ConsoleHelper.WriteWarning(">>> Cancel() çağrılıyor! <<<");
            manualCts.Cancel();
        });

        try
        {
            await FakeWorkSimulator.LongRunningWorkAsync(
                "ManualCancelJob", totalMs: 5000, ct: manualCts.Token);
        }
        catch (OperationCanceledException)
        {
            ConsoleHelper.WriteWarning("İş iptal edildi → Manuel Cancel() çağrısı.");
        }

        await canceller;

        // ── SENARYO 3: Linked Token ──────────────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo C: Linked CancellationToken (birden fazla kaynak)");
        ConsoleHelper.WriteExplanation("HTTP request token'ı + timeout token'ı birleştiriyoruz.");
        ConsoleHelper.WriteExplanation("Hangisi önce tetiklenirse → iş iptal olur.");
        ConsoleHelper.WriteSeparator();

        using var requestCts = new CancellationTokenSource();   // HTTP request simülasyonu
        using var limitCts   = new CancellationTokenSource(TimeSpan.FromSeconds(3)); // Timeout

        // İkisini birleştir: biri tetiklenirse diğeri de iptal olur
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            requestCts.Token, limitCts.Token);

        // 1sn sonra "request" kesildi simülasyonu
        _ = Task.Run(async () =>
        {
            await Task.Delay(1000);
            ConsoleHelper.WriteWarning(">>> HTTP request kesildi! requestCts.Cancel() <<<");
            requestCts.Cancel();
        });

        try
        {
            await FakeWorkSimulator.LongRunningWorkAsync(
                "LinkedTokenJob", totalMs: 8000, ct: linkedCts.Token);
        }
        catch (OperationCanceledException)
        {
            ConsoleHelper.WriteWarning("İş iptal edildi → Linked token tetiklendi.");
            ConsoleHelper.WriteInfo($"requestCts.IsCancellationRequested: {requestCts.IsCancellationRequested}");
            ConsoleHelper.WriteInfo($"limitCts.IsCancellationRequested:   {limitCts.IsCancellationRequested}");
        }

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• Her async metot CancellationToken parametresi almalı.");
        ConsoleHelper.WriteInfo("• ct.ThrowIfCancellationRequested() ile ara kontrol yap.");
        ConsoleHelper.WriteInfo("• OperationCanceledException catch'e al, log'la ama yutma.");
        ConsoleHelper.WriteInfo("• Linked token: request + timeout gibi çoklu iptal kaynağı.");
        ConsoleHelper.WriteInfo("• ASP.NET Core: HttpContext.RequestAborted token'ını kullan.");

        ConsoleHelper.WaitForKey();
    }
}
