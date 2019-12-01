using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using WarehouseDataLoader.Parser.IndexBased.StockPartValidator;

namespace WarehouseDataLoader.Benchmark
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class StockPartValidatorBenchmark
    {
        private readonly string[] SampleLines = new[]
            {
                "Tokyo,1213|Yokohama,952|Osaka,459",             
                "Tokyo,1|Yokohama,2|Osaka,3|Nagoya,4|Sapporo,5|Fukuoka,6|Kobe,7|Kawasaki,8|Kyoto,9",
                "Shelf;1234567890|Shelf,7",
            };

        [Benchmark]
        public bool RegexBased()
        {
            bool result = false;
            var validator = new StockPartValidatorRegexBased();           
            foreach (var line in SampleLines)
            {
                result = validator.Validate(line, 0);
            }
            return result;
        }

        [Benchmark]
        public bool OnStateMachine()
        {
            bool result = false;
            var validator = new StockPartValidatorOnStateMachine();
            foreach (var line in SampleLines)
            {
                result = validator.Validate(line, 0);
            }
            return result;
        }
    }
}
