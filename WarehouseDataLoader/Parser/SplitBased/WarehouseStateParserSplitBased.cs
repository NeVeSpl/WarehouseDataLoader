using System;
using System.Collections.Generic;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.SplitBased
{
    internal sealed class WarehouseStateParserSplitBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly List<string> invalidLines = new List<string>();


        public WarehouseStateParserSplitBased(IWarehouse state)
        {
            this.warehouse = state;
        }


        public ParsingResult GetResult()
        {
            return new ParsingResult(invalidLines, warehouse.Shelves);
        }
        public void ParseLine(string line)
        {
            if (IsCommentLine(line))
            {
                return;
            }

            string[] splitedLine = line.Split(';', 3, StringSplitOptions.RemoveEmptyEntries);
            bool isLineValid = false;

            if (splitedLine.Length == 3)
            {
                string itemName = splitedLine[0];
                string itemId = splitedLine[1];
                string stockPart = splitedLine[2];               
                var stockPartParsingResult = ParseStockPart(stockPart);
                if (stockPartParsingResult.isStockPartValid)
                {
                    isLineValid = true;

                    foreach (var quantityOnShelf in stockPartParsingResult.quantityPerShelf)
                    {
                        warehouse.AddItemToShelf(itemId, itemName, quantityOnShelf.Quantity, quantityOnShelf.Shelf);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private (bool isStockPartValid, List<QuantityOnShelf> quantityPerShelf) ParseStockPart(string stockPart)
        {
            bool isStockPartValid = true;

            string[] splitedStockPart = stockPart.Split('|');
            var quantityPerShelf = new List<QuantityOnShelf>(splitedStockPart.Length);

            foreach (string shelfAndQuantity in splitedStockPart)
            {
                bool isShelfAndQuantityValid = false;
                string[] splitedShelfAndQuantity = shelfAndQuantity.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (splitedShelfAndQuantity.Length == 2)
                {
                    string shelf = splitedShelfAndQuantity[0];
                    string quantityString = splitedShelfAndQuantity[1];

                    bool isQuantityParsedSuccessfully = int.TryParse(quantityString, out int quantity);                   

                    if ((isQuantityParsedSuccessfully) && (quantityString.Length < 10))
                    {
                        quantityPerShelf.Add(new QuantityOnShelf(shelf, quantity));
                        isShelfAndQuantityValid = true;
                    }
                }
                isStockPartValid &= isShelfAndQuantityValid;
            }

            return (isStockPartValid, quantityPerShelf);
        }
        private bool IsCommentLine(string line)
        {
            return line.StartsWith("#");
        }


        private readonly struct QuantityOnShelf
        {
            public readonly string Shelf;
            public readonly int Quantity;


            public QuantityOnShelf(string shelf, int quantity)
            {
                Shelf = shelf;
                Quantity = quantity;
            }
        }
    }
}