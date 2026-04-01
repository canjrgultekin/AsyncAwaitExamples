using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitExample.Helpers;

namespace AsyncAwaitExample.Demos;

/// <summary>
/// DEMO 4: Deadlock Anti-Pattern
///
/// Anahtar kavram: .Result veya .Wait() kullanmak SynchronizationContext
/// olan ortamlarda (ASP.NET Classic, WinForms, WPF) deadlock'a neden olur.
/// Console uygulamasında SynchronizationContext olmadığından deadlock olmaz
/// ama pattern'i anlamak kritik. Bunu ASP.NET Core'da deneme!
/// </summary>
public static class DeadlockDemo
{
    public static async Task RunAsync()
    {
        ConsoleHelper.WriteHeader("DEMO 4: Deadlock Anti-Pattern");

        ConsoleHelper.WriteWarning("NOT: Console app'te SynchronizationContext yok, deadlock olmaz.");
        ConsoleHelper.WriteWarning("Ama ASP.NET Classic, WinForms, WPF'te bu pattern DEADLOCK yapar!");
        ConsoleHelper.WriteExplanation("Bu demo neden olduğunu anlatır, production'da yapma!");

        // ── DEADLOCK NEDEN OLUR? ─────────────────────────────────────────────
        ConsoleHelper.WriteSection("Deadlock Nasıl Oluşur? (Kavramsal Açıklama)");

        ConsoleHelper.WriteInfo("Senaryo (ASP.NET Classic / WinForms):");
        ConsoleHelper.WriteInfo("");
        ConsoleHelper.WriteInfo("  1. Main thread (SynchronizationContext'li) async metod çağırır");
        ConsoleHelper.WriteInfo("  2. async metot await'e çarpar → thread serbest kalır");
        ConsoleHelper.WriteInfo("  3. .Result ile BLOKE EDER → thread dondu, SyncContext bekliyor");
        ConsoleHelper.WriteInfo("  4. await tamamlandı, continuation AYNI thread'e post edilmek ister");
        ConsoleHelper.WriteInfo("  5. DEADLOCK: Thread bloke, continuation thread'e giremez, iki taraf bekler!");
        ConsoleHelper.WriteSeparator();

        // ── YANLIŞ PATTERN (Gösterim amaçlı) ───────────────────────────────
        ConsoleHelper.WriteSection("YANLIŞ Pattern — .Result / .Wait() kullanımı");

        ConsoleHelper.WriteExplanation("Bu console'da deadlock YAPMAZ ama production ASP.NET'te yapar:");
        ConsoleHelper.WriteSeparator();

        try
        {
            ConsoleHelper.WriteThread("ANTI-PATTERN", ".GetAwaiter().GetResult() çağrılıyor...");

            // Console'da çalışır ama SynchronizationContext'li ortamda DEADLOCK
            var result = GetDataAsync().GetAwaiter().GetResult();

            ConsoleHelper.WriteSuccess($"Console'da çalıştı: {result}");
            ConsoleHelper.WriteWarning("AMA ASP.NET Classic'te bu satır ASLA dönmezdi (deadlock)!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Hata: {ex.Message}");
        }

        // ── DOĞRU PATTERN ────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("DOĞRU Pattern — async all the way");

        ConsoleHelper.WriteInfo("Kural basit: async'i async ile çağır, zincirleri kırma.");
        ConsoleHelper.WriteInfo("Bir noktada async kullanmak zorundaysan → her katman async olmalı.");
        ConsoleHelper.WriteSeparator();

        ConsoleHelper.WriteThread("CORRECT", "await ile çağrılıyor...");
        var correctResult = await GetDataAsync();
        ConsoleHelper.WriteSuccess($"Doğru yaklaşım: {correctResult}");

        // ── ConfigureAwait AÇIKLAMASI ─────────────────────────────────────────
        ConsoleHelper.WriteSection("ConfigureAwait(false) — Library kodlarında şart");

        ConsoleHelper.WriteExplanation("Library yazıyorsan ConfigureAwait(false) kullan:");
        ConsoleHelper.WriteExplanation("Continuation, orijinal SynchronizationContext'e dönmez → deadlock riski azalır");
        ConsoleHelper.WriteSeparator();

        // Library tarzı kullanım
        var libResult = await GetDataWithConfigureAwaitAsync();
        ConsoleHelper.WriteSuccess($"Library pattern (ConfigureAwait(false)): {libResult}");

        ConsoleHelper.WriteInfo("");
        ConsoleHelper.WriteInfo("Özet kural:");
        ConsoleHelper.WriteInfo("  • App kodu (Controller, ViewModel): ConfigureAwait(false) GEREKMİYOR");
        ConsoleHelper.WriteInfo("  • Library/NuGet kodu: ConfigureAwait(false) ŞART");
        ConsoleHelper.WriteInfo("  • ASP.NET Core: SynchronizationContext yok → deadlock riski yok");

        // ── SONUÇ ────────────────────────────────────────────────────────────
        ConsoleHelper.WriteSection("Öğrendiklerin");
        ConsoleHelper.WriteInfo("• .Result / .Wait() = sync-over-async = deadlock riski.");
        ConsoleHelper.WriteInfo("• 'async all the way' prensibi: her katmanda async/await kullan.");
        ConsoleHelper.WriteInfo("• Library yazıyorsan: ConfigureAwait(false) ekle.");
        ConsoleHelper.WriteInfo("• ASP.NET Core'da deadlock riski düşük ama yine de .Result yapma.");
        ConsoleHelper.WriteInfo("• async void kullanma (EventHandler dışında) → exception yakalanamaz!");

        ConsoleHelper.WaitForKey();
    }

    private static async Task<string> GetDataAsync()
    {
        await Task.Delay(300);
        return "async data hazır";
    }

    private static async Task<string> GetDataWithConfigureAwaitAsync()
    {
        // ConfigureAwait(false): continuation orijinal context'e dönmeye ÇALIŞMAZ
        await Task.Delay(200).ConfigureAwait(false);
        return "library pattern, ConfigureAwait(false) ile";
    }
}
