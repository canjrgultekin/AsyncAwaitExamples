namespace CpuBoundExample.Examples;

/// <summary>
/// ✅ BEST PRACTICES: Parallel.For ile CPU-bound işler
/// Multi-core kullanımı, partitioning, load balancing
/// </summary>
public static class ParallelBestPractices
{
    public static void Run()
    {
        Console.WriteLine("✅ BEST PRACTICES: Parallel.For");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"CPU Core Count: {Environment.ProcessorCount}\n");

        const int dataSize = 10_000_000;
        var data = Enumerable.Range(0, dataSize).ToArray();

        Console.WriteLine($"→ Processing {dataSize:N0} items...\n");

        // 1. Sequential (baseline)
        Console.WriteLine("1️⃣  Sequential (baseline):");
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        long sum1 = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum1 += data[i] * data[i];
        }
        sw1.Stop();
        Console.WriteLine($"  Sum: {sum1:N0}, Time: {sw1.ElapsedMilliseconds:N0} ms\n");

        // 2. Parallel.For (basic)
        Console.WriteLine("2️⃣  Parallel.For (basic):");
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        long sum2 = 0;
        object lockObj = new object();
        Parallel.For(0, data.Length, i =>
        {
            var result = data[i] * data[i];
            lock (lockObj)
            {
                sum2 += result;
            }
        });
        sw2.Stop();
        Console.WriteLine($"  Sum: {sum2:N0}, Time: {sw2.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Speedup: {(double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds:F2}x\n");

        // 3. Parallel.For with local state (optimized)
        Console.WriteLine("3️⃣  Parallel.For (thread-local, optimized):");
        var sw3 = System.Diagnostics.Stopwatch.StartNew();
        long sum3 = 0;
        Parallel.For(0, data.Length,
            () => 0L, // Thread-local initial value
            (i, state, localSum) =>
            {
                localSum += data[i] * data[i];
                return localSum;
            },
            localSum =>
            {
                lock (lockObj)
                {
                    sum3 += localSum;
                }
            });
        sw3.Stop();
        Console.WriteLine($"  Sum: {sum3:N0}, Time: {sw3.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Speedup: {(double)sw1.ElapsedMilliseconds / sw3.ElapsedMilliseconds:F2}x\n");

        // 4. PLINQ (alternative)
        Console.WriteLine("4️⃣  PLINQ (alternative approach):");
        var sw4 = System.Diagnostics.Stopwatch.StartNew();
        var sum4 = data.AsParallel()
            .Select(x => (long)x * x)
            .Sum();
        sw4.Stop();
        Console.WriteLine($"  Sum: {sum4:N0}, Time: {sw4.ElapsedMilliseconds:N0} ms");
        Console.WriteLine($"  Speedup: {(double)sw1.ElapsedMilliseconds / sw4.ElapsedMilliseconds:F2}x\n");

        Console.WriteLine("✅ Best Practices:");
        Console.WriteLine("  - Thread-local state: Lock contention azaltır");
        Console.WriteLine("  - Parallel overhead: Küçük işler için maliyetli");
        Console.WriteLine("  - PLINQ: Declarative, LINQ syntax");
        Console.WriteLine("  - MaxDegreeOfParallelism: CPU bound için varsayılan iyi");
    }
}
