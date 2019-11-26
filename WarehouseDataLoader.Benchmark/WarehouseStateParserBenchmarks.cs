using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.IndexBased;
using WarehouseDataLoader.Parser.RegexBased;
using WarehouseDataLoader.Parser.SpanBased;
using WarehouseDataLoader.Parser.SpanBased.StringPool;
using WarehouseDataLoader.Parser.SplitBased;

namespace WarehouseDataLoader.Benchmark
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class WarehouseStateParserBenchmarks
    {
        private const int HowManyTimes = 30;
        private readonly string[] SampleLines = new[]
            {               
                "Bulbasaur;#001;Tokyo,1213|Yokohama,952|Osaka,459",
                "Ivysaur;#002;Nagoya,7|Sapporo,123|Fukuoka,874",
                "Venusaur;#003;Kobe,235|Kawasaki,2|Kyoto,790",
                "Charmander;#004;Tokyo,1",
                "Charmeleon;#005;Tokyo,1213|Yokohama,952|Osaka,459",
                "Charizard;#006;Kyoto,2832|Yokohama,926|Fukuoka,17890",
                "Squirtle;#007;Kobe,235|Yokohama,2|Kyoto,790",
                "Wartortle;#008;Kobe,9",
                "Blastoise;#009;Fukuoka,1",
                "Caterpie;#010;Tokyo,1|Yokohama,2|Osaka,3|Nagoya,4|Sapporo,5|Fukuoka,6|Kobe,7|Kawasaki,8|Kyoto,9",
                "Metapod;#011;Tokyo,10|Yokohama,20|Osaka,30|Nagoya,40|Sapporo,50|Fukuoka,60|Kobe,70|Kawasaki,80",
                "Butterfree;#012;Tokyo,100|Yokohama,200|Osaka,300|Nagoya,400|Sapporo,500|Fukuoka,600|Kobe,700|",
                "Weedle;#013;Tokyo,1000|Yokohama,2000|Osaka,3000|Nagoya,4000|Sapporo,5000|Fukuoka,6000",
                "Kakuna;#014;Tokyo,10000|Yokohama,20000|Osaka,30000|Nagoya,40000|Sapporo,50000",
                "Beedrill;#015;Tokyo,10000|Yokohama,20000|Osaka,30000|Nagoya,40000",
                "Pidgey;#016;Tokyo,100000|Yokohama,200000|Osaka,300000",
            };


        [Benchmark(Baseline = true)]
        public void SplitBased()
        {
            var parser = new WarehouseStateParserSplitBased(new ParserStateStub());
            for (int i = 0; i < HowManyTimes; ++i)
            {
                foreach (var line in SampleLines)
                {
                    parser.ParseLine(line);
                }
            }
        }

        [Benchmark]
        public void SpanBased()
        {
            var parser = new WarehouseStateParserSpanBased(new ParserStateStub(), new NoStringPool());
            for (int i = 0; i < HowManyTimes; ++i)
            {
                foreach (var line in SampleLines)
                {
                    parser.ParseLine(line);
                }
            }
        }
        [Benchmark]
        public void SpanBasedWithStringPool()
        {
            var parser = new WarehouseStateParserSpanBased(new ParserStateStub(), new StringPool());
            for (int i = 0; i < HowManyTimes; ++i)
            {
                foreach (var line in SampleLines)
                {
                    parser.ParseLine(line);
                }
            }
        }

        [Benchmark]
        public void RegexBased()
        {
            var parser = new WarehouseStateParserRegexBased(new ParserStateStub());
            for (int i = 0; i < HowManyTimes; ++i)
            {
                foreach (var line in SampleLines)
                {
                    parser.ParseLine(line);
                }
            }
        }

        [Benchmark]
        public void IndexBased()
        {
            var parser = new WarehouseStateParserIndexBased(new ParserStateStub());
            for (int i = 0; i < HowManyTimes; ++i)
            {
                foreach (var line in SampleLines)
                {
                    parser.ParseLine(line);
                }
            }
        }






        internal class ParserStateStub : IParserState
        {
            public IReadOnlyCollection<Warehouse> Warehouses { get; }

            public void AddItemToWarehouse(string itemName, string itemId, int itemAmount, string warehouseName)
            {

            }
        }
    }
}
