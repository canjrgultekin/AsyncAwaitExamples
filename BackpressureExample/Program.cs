using BackpressureExample.Examples;

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  BackpressureExample - Yanlış vs Doğru");
Console.WriteLine("═══════════════════════════════════════════════\n");

while (true)
{
    Console.WriteLine("Çalıştırmak istediğiniz örneği seçin:");
    Console.WriteLine("1 - Sync Over Async (YANLIŞ)");
    Console.WriteLine("2 - Full Async Chain (DOĞRU)");
    Console.WriteLine("3 - Deadlock Senaryosu (YANLIŞ - WPF/WinForms benzeri)");
    Console.WriteLine("4 - Async Event Handler (DOĞRU)");
    Console.WriteLine("5 - Backpressure + Rate Limit (PRODUCTION)");
    Console.WriteLine("0 - Çıkış\n");
    Console.Write("Seçim: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                await BadSyncOverAsync.RunAsync();
                break;
            case "2":
                await GoodFullAsync.RunAsync();
                break;
            case "3":
                BadDeadlock.Run();
                break;
            case "4":
                await GoodAsyncEventHandler.RunAsync();
                break;
            case "5":
                await ProductionBackpressure.RunAsync();
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
