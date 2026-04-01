using System;
using System.Threading.Tasks;
using AsyncAwaitExample.Demos;
using AsyncAwaitExample.Helpers;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = "Async/Await Eğitim Simülatörü — .NET 10";

await ShowMenuAsync();

static async Task ShowMenuAsync()
{
    while (true)
    {
        Console.Clear();

        ConsoleHelper.WriteHeader("Async / Await Eğitim Simülatörü  —  .NET 10");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  Bu uygulama .NET async/await kavramlarını interaktif demo'larla öğretir.");
        Console.WriteLine("  Her demo kendi bölümündeki konuyu thread ID'leri ve timing ile somutlaştırır.");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  ┌─────────────────────────────────────────────────────────────┐");
        Console.WriteLine("  │                       DEMO MENÜSÜ                          │");
        Console.WriteLine("  ├─────────────────────────────────────────────────────────────┤");
        Console.WriteLine("  │  [1]  Sync vs Async — Thread Bloklama Farkı                 │");
        Console.WriteLine("  │  [2]  Sequential vs Parallel — Task.WhenAll Gücü            │");
        Console.WriteLine("  │  [3]  CancellationToken — İptal Mekanizması                 │");
        Console.WriteLine("  │  [4]  Deadlock Anti-Pattern — .Result / .Wait() Tehlikesi   │");
        Console.WriteLine("  │  [5]  Exception Handling — Async'te Hata Yönetimi           │");
        Console.WriteLine("  │  [6]  ValueTask vs Task — Allocation Optimizasyonu          │");
        Console.WriteLine("  ├─────────────────────────────────────────────────────────────┤");
        Console.WriteLine("  │  [A]  Tüm Demo'ları Sırayla Çalıştır                       │");
        Console.WriteLine("  │  [Q]  Çıkış                                                 │");
        Console.WriteLine("  └─────────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("  Seçiminiz: ");

        var key = Console.ReadLine()?.Trim().ToUpperInvariant();

        switch (key)
        {
            case "1":
                await SyncVsAsyncDemo.RunAsync();
                break;
            case "2":
                await SequentialVsParallelDemo.RunAsync();
                break;
            case "3":
                await CancellationDemo.RunAsync();
                break;
            case "4":
                await DeadlockDemo.RunAsync();
                break;
            case "5":
                await ExceptionHandlingDemo.RunAsync();
                break;
            case "6":
                await ValueTaskDemo.RunAsync();
                break;
            case "A":
                await RunAllDemosAsync();
                break;
            case "Q":
                Console.Clear();
                ConsoleHelper.WriteSuccess("Görüşmek üzere! Async all the way. 🚀");
                Console.WriteLine();
                return;
            default:
                ConsoleHelper.WriteWarning("Geçersiz seçim. Lütfen 1-6, A veya Q girin.");
                await Task.Delay(1000);
                break;
        }
    }
}

static async Task RunAllDemosAsync()
{
    ConsoleHelper.WriteHeader("TÜM DEMO'LAR — Sıralı Çalıştırma");
    ConsoleHelper.WriteExplanation("Her demo tamamlandıktan sonra Enter'a basarak devam edebilirsin.");
    ConsoleHelper.WaitForKey();

    await SyncVsAsyncDemo.RunAsync();
    await SequentialVsParallelDemo.RunAsync();
    await CancellationDemo.RunAsync();
    await DeadlockDemo.RunAsync();
    await ExceptionHandlingDemo.RunAsync();
    await ValueTaskDemo.RunAsync();

    ConsoleHelper.WriteSuccess("Tüm demo'lar tamamlandı! Artık async/await'i çok daha iyi anlıyorsun.");
    ConsoleHelper.WaitForKey();
}
