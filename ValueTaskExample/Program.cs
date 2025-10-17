using ValueTaskExample.Examples;

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  ValueTaskExample - Yanlış vs Doğru");
Console.WriteLine("═══════════════════════════════════════════════\n");

while (true)
{
    Console.WriteLine("Çalıştırmak istediğiniz örneği seçin:");
    Console.WriteLine("1 - Multiple Await ValueTask (YANLIŞ)");
    Console.WriteLine("2 - Single Await ValueTask (DOĞRU)");
    Console.WriteLine("3 - ValueTask Store/Cache (YANLIŞ)");
    Console.WriteLine("4 - Cache with Correct ValueTask (DOĞRU)");
    Console.WriteLine("5 - Performance Comparison (Task vs ValueTask)");
    Console.WriteLine("0 - Çıkış\n");
    Console.Write("Seçim: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                await BadMultipleAwait.RunAsync();
                break;
            case "2":
                await GoodSingleAwait.RunAsync();
                break;
            case "3":
                await BadValueTaskStore.RunAsync();
                break;
            case "4":
                await GoodCacheValueTask.RunAsync();
                break;
            case "5":
                await PerformanceComparison.RunAsync();
                break;
            case "0":
                Console.WriteLine("Çıkış yapılıyor...");
                return;
            default:
                Console.WriteLine("❌ Geçersiz seçim!\n");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Hata: {ex.Message}\n");
    }

    Console.WriteLine("\n" + new string('─', 50) + "\n");
}
