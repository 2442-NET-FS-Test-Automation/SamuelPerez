using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace DsaThreading;

public static class Program
{
	private static async Task Main(string[] args)
	{
		Console.WriteLine("Hello, World!");
		await ThreadingDemoAsync();
	}

	private static async Task ThreadingDemoAsync()
	{
		Console.WriteLine($"Main runs on thread #{Environment.CurrentManagedThreadId}");

        var workerThread = new Thread(() =>
        {
            Console.WriteLine($"Hello from Thread #{Environment.CurrentManagedThreadId}");
        });

        Console.WriteLine($"Before Start() call, isAlive = {workerThread.IsAlive}");

        workerThread.Start();
        Console.WriteLine($"During thread delegate running, isAlive = {workerThread.IsAlive}");
        workerThread.Join();

        Console.WriteLine($"After Join() call, isAlive = {workerThread.IsAlive}");

        var threads = new List<Thread>();

        for (int i = 1; i <= 5; i++)
        {
            int id = i;

            var th = new Thread(() =>
            {
                Thread.Sleep(Random.Shared.Next(5, 40));
                Console.WriteLine($"Worker {id} finished on thread #{Environment.CurrentManagedThreadId}");
            });

            threads.Add(th);
            th.Start();
        }

        foreach(Thread thread in threads){ thread.Join();}

        var counts = new ConcurrentDictionary<int, int>();

        var threadPool = new List<Thread>();

        for (int i = 1; i <= 8; i++)
        {
            int id = i;

            var th = new Thread(() =>
            {
                for (int k = 0; k < 1000; k++)
                {
                    counts.AddOrUpdate(id, 1, (_, prev) => prev + 1);
                    
                }
            });

            threads.Add(th);
            th.Start();
        }

        foreach (var th in threadPool) th.Join();
        Console.WriteLine($"Recorded {counts.Values.Sum()} increments across {counts.Count} threads");


        var done = new ConcurrentQueue<int>();

        for (int i = 0; i<5; i++)
        {
            int n = i;

            ThreadPool.QueueUserWorkItem(_ => done.Enqueue(n*n));
        }

        while (done.Count < 5) Thread.Sleep(5);

        Console.WriteLine($"Threadpool finished. {string.Join(", ", done.OrderBy(x => x))}");

        ParallelSum();

        static void ParallelSum()
        {
            int[] data = Enumerable.Range(1, 8000000).ToArray();

            var sw = Stopwatch.StartNew();
            long sequential = SumRange(data, 0, data.Length);
            sw.Stop();
            Console.WriteLine($"Sequential sum = {sequential}. {sw.ElapsedTicks} ticks, 1 thread");

            Task<long> half1 = Task.Run(() => SumRange(data, 0, data.Length / 2));
            Task<long> half2 = Task.Run(() => SumRange(data, data.Length / 2, data.Length));

            long total = half1.Result + half2.Result;
            Console.WriteLine($"Two task sum: {total}");

            long parallelTotal = 0;

            sw.Restart();

            Parallel.For(0, data.Length, 
            () => 0L, 
            (i, _, local) => local + data[i],
            local => Interlocked.Add(ref parallelTotal, local)
            );
            sw.Stop();
            Console.WriteLine($"Parallel sum = {parallelTotal}. {sw.ElapsedTicks} ticks, multi-thread");

        }

        static long SumRange(int[] a, int start, int end)
        {
            long sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += a[i];
            }
            return sum;
        }

        RaceDemo();

        static void RaceDemo()
        {
            var Bank = new Bank();
            Parallel.For(0, 100000, _ => Bank.DepositUnsafe(1));
            Console.WriteLine($"Unsafe balance = {Bank.Balance} (expected 100000)");
        }
        
        SafeDemo();

        static void SafeDemo()
        {
            var Bank = new Bank();
            Parallel.For(0, 100000, _ => Bank.DepositSafe(1));
            Console.WriteLine($"Safe balance = {Bank.Balance} (expected 100000)");
        }

        InterLockerDemo();

        static void InterLockerDemo()
        {
            long counter = 0;

            Parallel.For(0, 100000, _ => Interlocked.Increment(ref counter));
            Console.WriteLine($"InterLocked = {counter} (expected 100000)");
        }


        CancellationDemo();

        static void CancellationDemo()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            CancellationToken token = cts.Token;

            var work = Task.Run( () =>
            {
                for (long i = 0; ; i++)
                {
                    token.ThrowIfCancellationRequested();
                    if (i % 5000000 == 0) {}
                }
            }, token);

            try
            {
                work.Wait();
            } catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
            {
                Console.WriteLine("Work was cancelled cooperatively");
            }
        }

        ExceptionDemo();

        static void ExceptionDemo()
        {
            var t = Task.Run( () => throw new InvalidOperationException("Oops - but in a task"));

            try
            {
                t.Wait();
            } catch (AggregateException ex)
            {
                Console.WriteLine($"Caught: {ex.InnerException!.Message}");
            }
        }

        await AsyncDemo();

        static async Task AsyncDemo()
        {
            Console.WriteLine($"Before await on thread #{Environment.CurrentManagedThreadId}");
            await Task.Delay(50);
            Console.WriteLine($"After await on thread #{Environment.CurrentManagedThreadId}");
        }
	}


}