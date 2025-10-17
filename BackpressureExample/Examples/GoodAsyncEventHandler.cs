namespace BackpressureExample.Examples;

/// <summary>
/// ✅ DOĞRU: Async event handler pattern
/// async void event handler + await kullanımı
/// </summary>
public static class GoodAsyncEventHandler
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Async Event Handler");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("UI event handler'larda async void kullanımı\n");

        // UI event'i simüle et
        await SimulateButtonClick();
    }

    // ✅ DOĞRU: async void sadece event handler'larda
    private static async Task SimulateButtonClick()
    {
        Console.WriteLine("→ 'Button_Click' event handler (async void)");
        Console.WriteLine("→ await ile async çağrı yapılıyor...\n");

        try
        {
            // ✅ await kullanımı
            var data = await GetDataAsync();
            
            Console.WriteLine($"✓ Veri alındı: {data}");
            Console.WriteLine("✓ UI güncellendi (simulated)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Hata yakalandı: {ex.Message}");
        }

        Console.WriteLine("\n✅ Avantajlar:");
        Console.WriteLine("  - UI thread bloke olmadı");
        Console.WriteLine("  - Deadlock riski yok");
        Console.WriteLine("  - Exception düzgün handle edildi");
        Console.WriteLine("\n⚠️  NOT: async void SADECE event handler'larda!");
        Console.WriteLine("  Diğer yerlerde async Task kullan.");
    }

    private static async Task<string> GetDataAsync()
    {
        await Task.Delay(500);
        return "User Data";
    }
}
