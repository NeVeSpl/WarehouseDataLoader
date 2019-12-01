using System;
using BenchmarkDotNet.Running;

namespace WarehouseDataLoader.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {           
            var summary =  BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
