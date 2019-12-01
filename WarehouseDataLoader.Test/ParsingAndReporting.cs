using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehouseDataLoader.Parser;
using WarehouseDataLoader.Raport;
using WarehouseDataLoader.Test.Utils;

namespace WarehouseDataLoader.Test
{
    [TestClass]
    public class ParsingAndReporting
    {
        [TestMethod]
        [DataRow("ShouldPastSampleTest")]        
        public void SplitBasedParser(string testCaseName)
        {        
            TestParsingAndReporting(testCaseName, WarehouseStateParserType.SplitBased);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]       
        public void SpanBasedParser(string testCaseName)
        {           
            TestParsingAndReporting(testCaseName, WarehouseStateParserType.SpanBased);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]
        public void SpanBasedWithStringPool(string testCaseName)
        {
            TestParsingAndReporting(testCaseName, WarehouseStateParserType.SpanBasedWithStringPool);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]      
        public void RegexBasedParser(string testCaseName)
        {         
            TestParsingAndReporting(testCaseName, WarehouseStateParserType.RegexBased);
        }

        [TestMethod]
        [DataRow("ShouldPastSampleTest")]       
        public void IndexBasedParser(string testCaseName)
        {           
            TestParsingAndReporting(testCaseName, WarehouseStateParserType.IndexBased);
        }


        private void TestParsingAndReporting(string testCaseName, WarehouseStateParserType parserType)
        {
            IWarehouseStateParser parser = WarehouseStateParserFactory.Create(parserType);
            string input = LoadResource(testCaseName + ".in");
            string expectedOutput = LoadResource(testCaseName + ".out");

            string[] inputLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var line in inputLines)
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
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
