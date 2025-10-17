namespace AsyncEnumerableExample.Examples;

/// <summary>
/// ❌ YANLIŞ: Tüm veriyi memory'e yüklemek
/// Büyük veri setlerinde OOM riski
/// </summary>
public static class BadLoadAllToMemory
{
    public static async Task RunAsync()
    {
        Console.WriteLine("❌ YANLIŞ ÖRNEK: Load All to Memory");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Tüm veri memory'e yüklenir (OOM riski)\n");

        Console.WriteLine("→ Simulating 10,000 records...");
        Console.WriteLine("  (Gerçekte 1M+ record olabilir)\n");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var memBefore = GC.GetTotalMemory(false);

        // ❌ YANLIŞ: Tüm veriyi çek, sonra işle
        var allData = await LoadAllDataAsync();
        
        var memAfter = GC.GetTotalMemory(false);
        var memUsed = (memAfter - memBefore) / 1024.0;

        Console.WriteLine($"✓ {allData.Count:N0} record memory'e yüklendi");
        Console.WriteLine($"  Memory: {memUsed:N2} KB\n");

        // İşleme
        int processed = 0;
        foreach (var item in allData)
        {
            // Process
            processed++;
        }

        sw.Stop();
        Console.WriteLine($"✓ {processed:N0} record işlendi");
        Console.WriteLine($"⏱️  Süre: {sw.ElapsedMilliseconds:N0} ms\n");

        Console.WriteLine("⚠️  Sorunlar:");
        Console.WriteLine("  - Tüm veri memory'de (1M record = ~GBler)");
        Console.WriteLine("  - İlk record'u işlemek için tüm veri beklemeli");
        Console.WriteLine("  - OOM exception riski");
        Console.WriteLine("  - Yavaş başlangıç (TTFB - Time To First Byte)");
        Console.WriteLine("\n✅ Çözüm: IAsyncEnumerable ile streaming");
    }

    private static async Task<List<DataItem>> LoadAllDataAsync()
    {
        var list = new List<DataItem>();
        for (int i = 0; i < 10_000; i++)
        {
            // Simulate DB fetch (batch)
            if (i % 1000 == 0)
                await Task.Delay(10);
            
            list.Add(new DataItem(i, $"Data{i}"));
        }
        return list;
    }

    private record DataItem(int Id, string Value);
}
