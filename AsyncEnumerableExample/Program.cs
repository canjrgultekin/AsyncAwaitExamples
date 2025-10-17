using AsyncEnumerableExample.Examples;

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  AsyncEnumerableExample - Yanlış vs Doğru");
Console.WriteLine("═══════════════════════════════════════════════\n");

while (true)
{
    Console.WriteLine("Çalıştırmak istediğiniz örneği seçin:");
    Console.WriteLine("1 - Load All to Memory (YANLIŞ)");
    Console.WriteLine("2 - Stream with IAsyncEnumerable (DOĞRU)");
    Console.WriteLine("3 - Sync Blocking in Async Stream (YANLIŞ)");
    Console.WriteLine("4 - Batch Processing (DOĞRU)");
    Console.WriteLine("5 - Cancellation Support (BEST PRACTICE)");
    Console.WriteLine("0 - Çıkış\n");
    Console.Write("Seçim: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                await BadLoadAllToMemory.RunAsync();
                break;
            case "2":
                await GoodStreamData.RunAsync();
                break;
            case "3":
                await BadSyncBlockingInStream.RunAsync();
                break;
            case "4":
                await GoodBatchProcessing.RunAsync();
                break;
            case "5":
                await BestPracticeCancellation.RunAsync();
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
