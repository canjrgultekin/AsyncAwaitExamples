namespace BackpressureExample.Examples;

/// <summary>
/// ❌ YANLIŞ: Deadlock senaryosu (WPF/WinForms benzeri)
/// SynchronizationContext varken .Result kullanımı
/// NOT: Console app'te context yok, bu yüzden simulate ediyoruz
/// </summary>
public static class BadDeadlock
{
    public static void Run()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Deadlock (Simulated UI Context)");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("WPF/WinForms'ta bu kod deadlock yapar!\n");

        Console.WriteLine("⚠️  NOT: Console app'te SynchronizationContext yok,");
        Console.WriteLine("    gerçek deadlock olmaz ama pattern'i gösteriyor.\n");

        // UI thread'i simüle et (Button_Click gibi)
        SimulateUIThreadBlocking();
    }

    private static void SimulateUIThreadBlocking()
    {
        Console.WriteLine("→ 'Button_Click' event handler çağrıldı...");
        Console.WriteLine("→ GetDataAsync().Result çağrılıyor (UI thread bloke)...\n");

        try
        {
            // ❌ Bu WPF/WinForms'ta deadlock yaratır:
            // 1. .Result, UI thread'i bloklar
            // 2. GetDataAsync, await sonrası UI thread'e dönmeye çalışır
            // 3. Ama UI thread bloklu (.Result bekliyor)
            // 4. Deadlock!
            
            var data = GetDataAsync().Result;
            Console.WriteLine($"✓ Veri: {data}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Hata: {ex.Message}");
        }

        Console.WriteLine("\n⚠️  WPF/WinForms'ta olsaydı:");
        Console.WriteLine("  - UI donardı (freeze)");
        Console.WriteLine("  - Continuation UI thread'e dönemez");
        Console.WriteLine("  - Sonsuz deadlock");
        Console.WriteLine("\n✅ Çözüm: async void Button_Click + await kullan");
    }

    private static async Task<string> GetDataAsync()
    {
        await Task.Delay(100);
        // Gerçek UI context'te, burası UI thread'e post edilir
        // Ama UI thread bloklu → deadlock
        return "Data";
    }
}
