using System.Net;
using System.Net.Http.Json;
using System.Threading.Channels;
using System.Threading.RateLimiting;

namespace BackpressureExample.Examples;

/// <summary>
/// ✅ PRODUCTION-READY: Backpressure + Rate-Limiting + HttpClient Pooling
/// Gerçek dünya senaryosu: Yüksek yük altında dış API'ye güvenli çağrı
/// </summary>
public static class ProductionBackpressure
{
    public static async Task RunAsync()
    {
        Console.WriteLine("✅ PRODUCTION ÖRNEK: Backpressure + Rate-Limit");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine("Bounded queue + concurrency limit + connection pooling\n");

        var cts = new CancellationTokenSource();
        Console.WriteLine("⌨️  'q' tuşuna basarak durdurun...\n");

        // Background'da 'q' dinle
        _ = Task.Run(() =>
        {
            if (Console.ReadKey(true).KeyChar == 'q')
                cts.Cancel();
        });

        await RunProcessorAsync(cts.Token);
    }

    private static async Task RunProcessorAsync(CancellationToken ct)
    {
        // Bounded channel: Max 100 item in queue
        var channel = Channel.CreateBounded<Job>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        // Rate limiter: Max 20 concurrent
        var limiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions
        {
            PermitLimit = 20,
            QueueLimit = 50,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });

        // HttpClient (singleton pattern)
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            MaxConnectionsPerServer = 64,
            AutomaticDecompression = DecompressionMethods.GZip
        };
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://httpbin.org"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        // Producer
        var producerTask = Task.Run(async () =>
        {
            for (int i = 0; i < 50 && !ct.IsCancellationRequested; i++)
            {
                var job = new Job(Guid.NewGuid(), i);
                await channel.Writer.WriteAsync(job, ct);
                Console.WriteLine($"→ Produced: Job {i}");
                await Task.Delay(50, ct);
            }
            channel.Writer.Complete();
        }, ct);

        // Consumers (4 paralel)
        var consumerTasks = Enumerable.Range(0, 4).Select(async workerId =>
        {
            await foreach (var job in channel.Reader.ReadAllAsync(ct))
            {
                using var lease = await limiter.AcquireAsync(1, ct);
                if (!lease.IsAcquired) continue;

                try
                {
                    var response = await client.PostAsJsonAsync("/anything", job, ct);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"  ✓ Worker {workerId}: Job {job.Id} completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Worker {workerId}: {ex.Message}");
                }
            }
        });

        await Task.WhenAll(producerTask, Task.WhenAll(consumerTasks));

        Console.WriteLine("\n✅ Pattern'ler:");
        Console.WriteLine("  - Bounded channel: Memory koruması");
        Console.WriteLine("  - Rate limiter: Dış servis koruması");
        Console.WriteLine("  - Connection pooling: Socket reuse");
        Console.WriteLine("  - Paralel consumers: Throughput optimization");

        client.Dispose();
        limiter.Dispose();
    }

    private record Job(Guid Id, int Payload);
}
