# Async/Await Deep Dive - Yanlış vs Doğru Kod Örnekleri

**.NET 9 & C# 13 - Production-Ready Kod Koleksiyonu**

Bu repo, **"Async/Await'in Karanlık Tarafı"** makalesindeki tüm kod örneklerini **interaktif menülerle** içerir. Her proje, yaygın hataları ve doğru kullanımları **yan yana** gösterir.

## 🎯 Ne İçeriyor?

4 bağımsız proje, her biri 5 örnek:

1. **BackpressureExample** - Sync over async, deadlock, backpressure
2. **ValueTaskExample** - ValueTask kullanım hataları, performance comparison
3. **CpuBoundExample** - CPU vs I/O bound, Task.Run, Parallel.For
4. **AsyncEnumerableExample** - Streaming, batch processing, cancellation

**Toplam 20 çalışır örnek!**

## 🚀 Hızlı Başlangıç

### Gereksinimler
- .NET 9 SDK ([İndir](https://dotnet.microsoft.com/download/dotnet/9.0))
- Visual Studio 2022 / VS Code / Rider (isteğe bağlı)

### Çalıştırma (Console)
```bash
cd BackpressureExample
dotnet run
```

### Çalıştırma (Visual Studio)
1. **AsyncAwaitExamples.sln** dosyasını aç
2. Solution Explorer'da bir proje seç
3. F5 veya Ctrl+F5 ile çalıştır

### Hepsini Aynı Anda (VS)
1. Solution'a sağ tık → **Set Startup Projects**
2. **Multiple startup projects** seç
3. Dört projeyi de **Start** yap
4. F5 → 4 console window birden açılır

## 📚 Proje Detayları

### 1. BackpressureExample

**Konular:** Thread pool starvation, deadlock, sync over async

**Örnekler:**
- ❌ Sync Over Async (.Result kullanımı)
- ✅ Full Async Chain (Task.WhenAll)
- ❌ Deadlock Senaryosu (UI context simulation)
- ✅ Async Event Handler (doğru pattern)
- ✅ Production Backpressure (Channel + RateLimiter)

**Öğretiler:**
- `.Result` ve `.Wait()` neden tehlikeli
- Deadlock nasıl oluşur (WPF/WinForms)
- Backpressure ve rate-limiting
- HttpClient pooling best practices

---

### 2. ValueTaskExample

**Konular:** Allocation optimization, cache patterns

**Örnekler:**
- ❌ Multiple Await ValueTask (undefined behavior)
- ✅ Single Await or AsTask() (doğru kullanım)
- ❌ Store ValueTask in Field (tehlikeli)
- ✅ Cache Pattern (senkron completion optimization)
- 📊 Performance Comparison (Task vs ValueTask)

**Öğretiler:**
- ValueTask tek kullanımlık
- Ne zaman ValueTask, ne zaman Task
- Hot path optimization
- 100K iterasyonda %50+ hız farkı

---

### 3. CpuBoundExample

**Konular:** CPU vs I/O bound, parallelism, Task.Run

**Örnekler:**
- ❌ CPU-Bound on ThreadPool (thread kilitleme)
- ✅ CPU-Bound with Task.Run (doğru pattern)
- ❌ Fake Async (Task.Run + Thread.Sleep)
- ✅ True Async I/O (non-blocking)
- ✅ Parallel.For Best Practices (multi-core)

**Öğretiler:**
- CPU-bound işler Task.Run ile
- Thread.Sleep vs Task.Delay farkı
- Parallel.For thread-local state optimization
- PLINQ alternatifi

---

### 4. AsyncEnumerableExample

**Konular:** Streaming, lazy loading, memory efficiency

**Örnekler:**
- ❌ Load All to Memory (OOM riski)
- ✅ Stream with IAsyncEnumerable (lazy loading)
- ❌ Sync Blocking in Stream (Thread.Sleep hatası)
- ✅ Batch Processing (network optimization)
- ✅ Cancellation Support (graceful shutdown)

**Öğretiler:**
- IAsyncEnumerable ile streaming
- Memory kullanımı sabit (10K item → ~10 KB)
- Batch processing + streaming kombinasyonu
- [EnumeratorCancellation] attribute

---

## 🔍 Örnek Çalıştırma

Her proje **interaktif menü** ile çalışır:

```
═══════════════════════════════════════════════
  BackpressureExample - Yanlış vs Doğru
═══════════════════════════════════════════════

Çalıştırmak istediğiniz örneği seçin:
1 - Sync Over Async (YANLIŞ)
2 - Full Async Chain (DOĞRU)
3 - Deadlock Senaryosu (YANLIŞ - WPF/WinForms benzeri)
4 - Async Event Handler (DOĞRU)
5 - Backpressure + Rate Limit (PRODUCTION)
0 - Çıkış

Seçim: _
```

Her örnek:
- ❌ YANLIŞ veya ✅ DOĞRU olarak işaretli
- Çalışan kod + açıklamalar
- Sorunlar ve çözümler listesi
- Performance metrikleri (bazılarında)

## 📖 Makale

Bu örneklerin detaylı açıklamaları için **"Async/Await'in Karanlık Tarafı: Kod Ezberciliğinden Kurumsal Mimariye"** makalesini okuyun.

## 🛠️ Proje Yapısı

```
AsyncAwaitExamples/
├── AsyncAwaitExamples.sln          ← Visual Studio solution
├── README.md                        ← Bu dosya
│
├── BackpressureExample/
│   ├── Program.cs                   ← İnteraktif menü
│   ├── BackpressureExample.csproj
│   └── Examples/
│       ├── BadSyncOverAsync.cs      ← ❌ Yanlış örnek
│       ├── GoodFullAsync.cs         ← ✅ Doğru örnek
│       └── ...
│
├── ValueTaskExample/
│   ├── Program.cs
│   ├── ValueTaskExample.csproj
│   └── Examples/
│       ├── BadMultipleAwait.cs
│       ├── GoodSingleAwait.cs
│       └── ...
│
├── CpuBoundExample/
│   ├── Program.cs
│   ├── CpuBoundExample.csproj
│   └── Examples/
│       ├── BadCpuBoundOnPool.cs
│       ├── GoodCpuBoundTaskRun.cs
│       └── ...
│
└── AsyncEnumerableExample/
    ├── Program.cs
    ├── AsyncEnumerableExample.csproj
    └── Examples/
        ├── BadLoadAllToMemory.cs
        ├── GoodStreamData.cs
        └── ...
```

## 🧪 Test Etme

Her örnek **bağımsız çalışır**, hiçbir dış servise bağımlılık yok (Production örneği hariç - httpbin.org kullanır).

## 💡 Öğrenme Yolu

**Önerilen sıra:**
1. **BackpressureExample** → async/await temel hataları
2. **CpuBoundExample** → CPU vs I/O ayrımı
3. **ValueTaskExample** → Performance optimization
4. **AsyncEnumerableExample** → Streaming patterns

## 🐛 Sorun Giderme

**Build hatası alıyorsan:**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Runtime hatası alıyorsan:**
- .NET 9 SDK kurulu mu? → `dotnet --version`
- Proje klasöründe misin? → `cd BackpressureExample`

## 📚 Kaynaklar

- [Microsoft Docs - Async/Await Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [Stephen Toub - Understanding ValueTask](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)
- [Thread Pool Starvation Debugging](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)

## 📄 Lisans

Bu örnekler eğitim amaçlıdır. İstediğin gibi kullanabilirsin.

---

**Son Güncelleme:** Ekim 2025  
**Hedef:** .NET 9, C# 13  
**Build Status:** ✅ Tüm projeler hatasız build olur

💬 **Geri bildirim:** Bu örnekleri geliştirmek için önerilerin varsa, issue aç veya PR gönder!
