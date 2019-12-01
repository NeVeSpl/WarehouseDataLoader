using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser;

namespace WarehouseDataLoader.Test
{
    public abstract class ValidLineDetection
    {
        private protected WarehouseStateParserType parserType;

        [TestMethod]
        [DataRow(",")]
        [DataRow("|")]
        public void ShouldParseLineWithItemNameThatContains(string text)
        {
            TestValidLineDetection($"Item{text} name;Item ID;Shelf,3053");
        }  
        
        [TestMethod]
        [DataRow(",")]
        [DataRow("|")]
        public void ShouldParseLineWithItemIdThatContains(string text)
        {
            TestValidLineDetection($"Item name;Item{text} ID;Shelf,3053");
        }       
       
        [TestMethod]
        public void ShouldParseLineWithShelfThatContainsSemicolon()
        {
            TestValidLineDetection("Item name;Item ID;She;lf,3053");
        }

        [TestMethod]
        public void ShouldParseLineWithMaximumAllowedQuantity()
        {
            TestValidLineDetection("Item name;Item ID;Shelf,999999999");
        }
        [TestMethod]
        public void ShouldParseLineWithMinimumAllowedQuantity()
        {
            TestValidLineDetection("Item name;Item ID;Shelf,1");
        }


        private void TestValidLineDetection(string inputLine)
        {
            var parser = WarehouseStateParserFactory.Create(parserType);
            parser.ParseLine(inputLine);
            ParsingResult parserResult = parser.GetResult();
            Assert.AreEqual(0, parserResult.InvalidLines.Count);
        }
    }

    [TestClass]
    public class ValidLineDetectionIndexBasedParser : ValidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.IndexBased;
        }
    }
    [TestClass]
    public class ValidLineDetectionRegexBasedParser : ValidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.RegexBased;
        }
    }
    [TestClass]
    public class ValidLineDetectionSpanBasedParser : ValidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SpanBased;
        }
    }
    [TestClass]
    public class ValidLineDetectionSpanBasedWithStringPoolParser : ValidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SpanBasedWithStringPool;
        }
    }
    [TestClass]
    public class ValidLineDetectionSplitBasedParser : ValidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SplitBased;
        }
    }
}
