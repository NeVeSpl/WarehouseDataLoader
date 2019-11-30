using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using WarehouseDataLoader.Parser;
using WarehouseDataLoader.Raport;
using WarehouseDataLoader.Test.Utils;

namespace WarehouseDataLoader.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        [DataRow("ShouldPastSampleTest")]        
        public void TestParsingAndReportingByUsingSplitBasedParser(string testCaseName)
        {
            var parser = WarehouseStateParserFactory.Create(WarehouseStateParserType.SplitBased);
            TestParsingAndReporting(testCaseName, parser);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]       
        public void TestParsingAndReportingByUsingSpanBasedParser(string testCaseName)
        {
            var parser = WarehouseStateParserFactory.Create(WarehouseStateParserType.SpanBased);
            TestParsingAndReporting(testCaseName, parser);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]      
        public void TestParsingAndReportingByUsingRegexBasedParser(string testCaseName)
        {
            var parser = WarehouseStateParserFactory.Create(WarehouseStateParserType.RegexBased);
            TestParsingAndReporting(testCaseName, parser);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]       
        public void TestParsingAndReportingByUsingIndexBasedParser(string testCaseName)
        {
            var parser = WarehouseStateParserFactory.Create(WarehouseStateParserType.IndexBased);
            TestParsingAndReporting(testCaseName, parser);
        }


        private void TestParsingAndReporting(string testCaseName, IWarehouseStateParser parser)
        {
            string input = LoadResource(testCaseName + ".in");
            string expectedOutput = LoadResource(testCaseName + ".out");

            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                parser.ParseLine(line);
            }

            var parsingResult = parser.GetResult();

            var raportGenerator = new TextRaportGenerator();
            string output = raportGenerator.Generate(parsingResult);

            AssertExt.AreEquivalent(expectedOutput, output);
        }
        private string LoadResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"WarehouseDataLoader.Test.Data.{fileName}";

            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
