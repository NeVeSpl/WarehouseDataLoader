using System;
using BenchmarkDotNet.Running;

namespace WarehouseDataLoader.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ParserBenchmarks>();
        }
    }
}
