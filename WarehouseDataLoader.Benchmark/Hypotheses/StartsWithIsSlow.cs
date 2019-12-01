using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace WarehouseDataLoader.Benchmark.Hypotheses
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class StartsWithIsSlow
    {
        private readonly string[] SampleLines = new[]
            {
                "Bulbasaur;#001;Tokyo,1213|Yokohama,952|Osaka,459",
                "# some comment",             
                "",
            };



        [Benchmark(Baseline = true)]
        public bool StartsWith()
        {
            bool result = false;
            foreach (var line in SampleLines)
            {
                result = line.StartsWith("#");
            }
            return result;
        }


        [Benchmark]
        public bool FirstIndexCheck()
        {
            bool result = false;
            foreach (var line in SampleLines)
            {
                result = (line.Length > 0) && (line[0] == '#');
            }
            return result;
        }
    }
}
