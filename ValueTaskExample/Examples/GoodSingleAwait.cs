namespace ValueTaskExample.Examples;

/// <summary>
/// ✅ DOĞRU: ValueTask tek seferlik await veya .AsTask() ile dönüştür
/// </summary>
public static class GoodSingleAwait
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ DOĞRU ÖRNEK: Single Await or AsTask()");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("ValueTask doğru kullanımı\n");

        // Yöntem 1: Tek await (önerilen)
        Console.WriteLine("✅ Yöntem 1: Tek await");
        var vt1 = GetValueTaskAsync();
        var result1 = await vt1; // Tek kullanım
        Console.WriteLine($"  Sonuç: {result1}\n");

        // Yöntem 2: .AsTask() ile Task'a çevir (çoklu await gerekirse)
        Console.WriteLine("✅ Yöntem 2: AsTask() ile çoklu await");
        var vt2 = GetValueTaskAsync();
        var task = vt2.AsTask(); // Task'a çevir

        var result2a = await task; // İlk await
        var result2b = await task; // İkinci await - şimdi güvenli!
        Console.WriteLine($"  Sonuç 1: {result2a}");
        Console.WriteLine($"  Sonuç 2: {result2b}");

        Console.WriteLine("\n✅ Kurallar:");
        Console.WriteLine("  - ValueTask: Tek await, sonra unut");
        Console.WriteLine("  - Çoklu await gerekirse: .AsTask() kullan");
        Console.WriteLine("  - AsTask() allocation yapar ama güvenli");
    }

    private static async ValueTask<int> GetValueTaskAsync()
    {
        await Task.Delay(100);
        return 42;
    }
}
