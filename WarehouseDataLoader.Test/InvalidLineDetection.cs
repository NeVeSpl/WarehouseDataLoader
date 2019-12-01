using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser;

namespace WarehouseDataLoader.Test
{
    public abstract class InvalidLineDetection
    {
        private protected WarehouseStateParserType parserType;


        [TestMethod]       
        public void ShouldDetectLineWithEmptyItemName()
        {          
            TestInvalidLineDetection(";Item ID;Shelf,3053");
        }

        [TestMethod]
        public void ShouldDetectLineWithEmptyItemID()
        {
            TestInvalidLineDetection("Item name;;Shelf,3053");
        }

        [TestMethod]
        public void ShouldDetectLineWithEmptyShelf()
        {
            TestInvalidLineDetection("Item name;Item ID;,3053");
        }

        [TestMethod]       
        public void ShouldDetectLineWithEmptyQuantity()
        {
            TestInvalidLineDetection("Item name;Item ID;Shelf,");
        }

        [TestMethod]
        public void ShouldDetectLineWithoutStockPart()
        {
            TestInvalidLineDetection("Item name;Item ID;");
        }

        [TestMethod]       
        public void ShouldDetectLineWithTooLongQuantity()
        {
            TestInvalidLineDetection("Item name;Item ID;Shelf,1234567890");
        }

        [TestMethod]
        [DataRow("Item name,Item ID;Shelf;1234567890|Shelf,7")]
        [DataRow("Item name;Item ID;Shelf,1234567890|Shelf,7|")]
        public void ShouldDetectIncorrectlyFormattedLine(string line)
        {
            TestInvalidLineDetection(line);
        }


        private void TestInvalidLineDetection(string inputLine)
        {
            var parser = WarehouseStateParserFactory.Create(parserType);
            parser.ParseLine(inputLine);
            ParsingResult parserResult = parser.GetResult();
            Assert.AreEqual(inputLine, parserResult.InvalidLines.FirstOrDefault());
        }
    }

    [TestClass]
    public class InvalidLineDetectionIndexBasedParser : InvalidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.IndexBased;
        }
    }
    [TestClass]
    public class InvalidLineDetectionRegexBasedParser : InvalidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.RegexBased;
        }
    }
    [TestClass]
    public class InvalidLineDetectionSpanBasedParser : InvalidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SpanBased;
        }
    }
    [TestClass]
    public class InvalidLineDetectionSpanBasedWithStringPoolParser : InvalidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SpanBasedWithStringPool;
        }
    }
    [TestClass]
    public class InvalidLineDetectionSplitBasedParser : InvalidLineDetection
    {
        [TestInitialize]
        public void TestInitialize()
        {
            parserType = WarehouseStateParserType.SplitBased;
        }
    }
}
