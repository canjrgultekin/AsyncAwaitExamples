using System;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 5: Async Exception Handling
///
/// Anahtar kavram: async metodlarda exception'lar Task içinde wrap'lenir.
/// await ile yakalanabilir. Task.WhenAll'da birden fazla exception olabilir.
/// async void exception'ları catch edilemez → asla kullanma.
/// </summary>
public static class ExceptionHandlingDemo
{
    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 5: Async Exception Handling");

        // ── TEK ASYNC EXCEPTION ───────────────────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo A: Tek async metodda exception");
        ConsoleHelper.WriteExplanation("Exception, Task içinde saklanır. await edildiğinde fırlatılır.");
        ConsoleHelper.WriteSeparator();

        try
        {
            var result = await FakeWorkSimulator.FailingWorkAsync("SingleService", 400, shouldFail: true);
            ConsoleHelper.WriteSuccess(result);
        }
        catch (InvalidOperationException ex)
        {
            ConsoleHelper.WriteError($"Yakalandı: {ex.Message}");
            ConsoleHelper.WriteExplanation("Normal try/catch gibi çalışır. await bunu sağlar.");
        }

        // ── TASK.WHENALL + ÇOKLU EXCEPTION ──────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo B: Task.WhenAll — birden fazla exception");
        ConsoleHelper.WriteExplanation("WhenAll, tüm task'ların bitmesini bekler (hatalı olanlar dahil).");
        ConsoleHelper.WriteExplanation("Doğrudan await ile sadece ilk exception görünür!");
        ConsoleHelper.WriteSeparator();

        var t1 = FakeWorkSimulator.FailingWorkAsync("ServiceA", 300, shouldFail: true);
        var t2 = FakeWorkSimulator.FailingWorkAsync("ServiceB", 500, shouldFail: false);
        var t3 = FakeWorkSimulator.FailingWorkAsync("ServiceC", 200, shouldFail: true);

        var allTasks = Task.WhenAll(t1, t2, t3);

        try
        {
            await allTasks;
        }
        catch
        {
            // allTasks.Exception içinde TÜM exception'lar var (AggregateException)
            if (allTasks.Exception is not null)
            {
                ConsoleHelper.WriteError($"Toplam {allTasks.Exception.InnerExceptions.Count} hata oluştu:");
                foreach (var inner in allTasks.Exception.InnerExceptions)
                    ConsoleHelper.WriteError($"  → {inner.Message}");
            }

            ConsoleHelper.WriteExplanation("İpucu: allTasks.Exception.InnerExceptions ile tüm hatalara eriş.");
        }

        // ── FIRE AND FORGET (TEHLİKELİ!) ─────────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo C: Fire and Forget — Exception Yutulur!");
        ConsoleHelper.WriteExplanation("Tehlikeli pattern: Task'ı await etmezsen exception kaybolur!");
        ConsoleHelper.WriteSeparator();

        // Bu pattern exception'ı yutar → production'da asla yapma
        ConsoleHelper.WriteWarning("Fire-and-forget başlatıldı (exception yutulacak)...");

        // Task'ı await etmeden bırakıyoruz
        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            ConsoleHelper.WriteError("Bu exception hiç görülmeyecek → TaskScheduler.UnobservedTaskException!");
            throw new InvalidOperationException("Bu exception kaybolacak!");
        });

        await Task.Delay(500); // Biraz bekle

        ConsoleHelper.WriteWarning("Exception yutudu. Hiçbir şey olmamış gibi devam etti. Bul bakalım.");
        ConsoleHelper.WriteExplanation("Çözüm: Gerçek fire-and-forget için IBackgroundJobClient veya Channel kullan.");

        // ── DOĞRU FIRE AND FORGET ─────────────────────────────────────────────
        ConsoleHelper.WriteSection("Senaryo D: Güvenli background iş kalıbı");
        ConsoleHelper.WriteExplanation("Hataları loglayan extension method ile fire-and-forget yap:");
        ConsoleHelper.WriteSeparator();

        FireAndForgetSafe(BackgroundJobAsync("SafeJob"), "SafeJob");
        await Task.Delay(700); // Arka planda bitmesini bekle
        ConsoleHelper.WriteSuccess("Background iş güvenli şekilde tamamlandı, exception loglandı.");

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• await ile exception normal try/catch ile yakalanır.");
        ConsoleHelper.WriteInfo("• Task.WhenAll'da tüm exception'lar için allTasks.Exception kullan.");
        ConsoleHelper.WriteInfo("• await etmediğin Task → exception kaybolur. Asla bırakma.");
        ConsoleHelper.WriteInfo("• async void sadece EventHandler için. Exception yakalanamaz!");
        ConsoleHelper.WriteInfo("• Fire-and-forget: hataları loglayan wrapper kullan.");

        ConsoleHelper.WaitForKey();
    }

    private static async Task BackgroundJobAsync(string name)
    {
        await Task.Delay(500);
        ConsoleHelper.WriteThread(name, "Background iş tamamlandı.");
    }

    /// <summary>
    /// Güvenli fire-and-forget: exception'ları yakalar ve loglar.
    /// Production'da ILogger inject et.
    /// </summary>
    private static void FireAndForgetSafe(Task task, string jobName)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
                ConsoleHelper.WriteError($"[{jobName}] Background hata: {t.Exception?.GetBaseException().Message}");
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
