using System;
using System.Runtime.CompilerServices;
using WarehouseDataLoader.Parser;
using WarehouseDataLoader.Raport;


[assembly: InternalsVisibleTo("WarehouseDataLoader.Test")]
[assembly: InternalsVisibleTo("WarehouseDataLoader.Benchmark")]
namespace WarehouseDataLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            IWarehouseStateParser parser = WarehouseStateParserFactory.Create(WarehouseStateParserType.SplitBased);
            string line;

            while ((line = Console.ReadLine()) != null)
            {
                parser.ParseLine(line);
            }

            var parsingResult = parser.GetResult();

            var raportGenerator = new TextRaportGenerator();
            string raport = raportGenerator.Generate(parsingResult);
            Console.WriteLine(raport);
        }
    }
}
