﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace FlatFiles.Benchmark
{
    public class Program
    {
        public static void Main()
        {
            RunBenchmarks();

            //RunPerformanceMonitor();
        }

        private static void RunBenchmarks()
        {
            var configuration = new ManualConfig
                                {
                                    Options = ConfigOptions.KeepBenchmarkFiles
                                };
            configuration.AddColumn(StatisticColumn.Min);
            configuration.AddColumn(StatisticColumn.Max);
            configuration.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            configuration.AddDiagnoser(DefaultConfig.Instance.GetDiagnosers().ToArray());
            configuration.AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
            configuration.AddJob(DefaultConfig.Instance.GetJobs().ToArray());
            configuration.AddValidator(DefaultConfig.Instance.GetValidators().ToArray());

            BenchmarkRunner.Run<CoreBenchmarkSuite>(configuration);

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }

        [SuppressMessage("CodeQuality", "IDE0051")]
        private static async Task RunPerformanceMonitor()
        {
            var tester = new AsyncVsSyncTest();
            for (var i = 0; i != 10; ++i)
            {
                await tester.SyncTest();
            }

            var stopwatch = Stopwatch.StartNew();
            var syncResult = await tester.SyncTest();
            stopwatch.Stop();
            Console.Out.WriteLine($"Sync Execution Time: {stopwatch.Elapsed}");
            Console.Out.WriteLine($"Sync Result Count: {syncResult.Length}");

            for (var i = 0; i != 10; ++i)
            {
                tester.AsyncTest().Wait();
            }

            stopwatch.Restart();
            var asyncResult = tester.AsyncTest().Result;
            stopwatch.Stop();

            Console.Out.WriteLine($"Async Execution Time: {stopwatch.Elapsed}");
            Console.Out.WriteLine($"Async Result Count: {asyncResult.Length}");
        }
    }
}
