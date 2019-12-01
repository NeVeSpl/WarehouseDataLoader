using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace WarehouseDataLoader.Benchmark.Hypotheses
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class IntParseIsSlow
    {
        private readonly string[] SampleLines = new[]
            {
                "123456789",
                "987654321",               
            };


        [Benchmark(Baseline = true)]
        public long IntParse()
        {
            long result = 0;
            foreach (var line in SampleLines)
            {
                result += int.Parse(line);
            }
            return result;
        }


        [Benchmark]
        public long IntTryParse()
        {
            long result = 0;
            foreach (var line in SampleLines)
            {
                int.TryParse(line, out int parseResult);
                result += parseResult;
            }
            return result;
        }

        [Benchmark]
        public long OwnImplementation()
        {
            long result = 0;
            foreach (var line in SampleLines)
            {
                int parseResult = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    parseResult = parseResult * 10 + (line[i] - '0');
                }
                result += parseResult;
            }
            return result;
        }
    }
}
