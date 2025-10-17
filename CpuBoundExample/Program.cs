using CpuBoundExample.Examples;

Console.WriteLine("═══════════════════════════════════════════════");
Console.WriteLine("  CpuBoundExample - Yanlış vs Doğru");
Console.WriteLine("═══════════════════════════════════════════════\n");

while (true)
{
    Console.WriteLine("Çalıştırmak istediğiniz örneği seçin:");
    Console.WriteLine("1 - CPU-Bound on ThreadPool (YANLIŞ)");
    Console.WriteLine("2 - CPU-Bound with Task.Run (DOĞRU)");
    Console.WriteLine("3 - Fake Async (Task.Run + Thread.Sleep) (YANLIŞ)");
    Console.WriteLine("4 - I/O Bound True Async (DOĞRU)");
    Console.WriteLine("5 - Parallel.For Best Practices");
    Console.WriteLine("0 - Çıkış\n");
    Console.Write("Seçim: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                await BadCpuBoundOnPool.RunAsync();
                break;
            case "2":
                await GoodCpuBoundTaskRun.RunAsync();
                break;
            case "3":
                await BadFakeAsync.RunAsync();
                break;
            case "4":
                await GoodTrueAsync.RunAsync();
                break;
            case "5":
                ParallelBestPractices.Run();
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
