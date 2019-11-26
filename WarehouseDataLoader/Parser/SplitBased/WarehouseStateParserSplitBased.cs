using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.SplitBased
{
    internal sealed class WarehouseStateParserSplitBased : IWarehouseStateParser
    {
        private readonly IParserState state;
        private readonly List<string> invalidLines = new List<string>();


        public WarehouseStateParserSplitBased(IParserState state)
        {
            this.state = state;
        }


        public ParsingResult GetResult()
        {
            return new ParsingResult(invalidLines, state.Warehouses);
        }
        public void ParseLine(string line)
        {
            if (IsCommentLine(line))
            {
                return;
            }

            string[] splitedLine = line.Split(';');
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

                    foreach (var amountInWarehouse in stockPartParsingResult.amountPerWarehouse)
                    {
                        state.AddItemToWarehouse(itemName, itemId, amountInWarehouse.Amount, amountInWarehouse.WarehouseName);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private (bool isStockPartValid, List<AmountInWarehouse> amountPerWarehouse) ParseStockPart(string stockPart)
        {
            bool isStockPartValid = true;

            string[] splitedStockPart = stockPart.Split('|');
            var amountPerWarehouse = new List<AmountInWarehouse>(splitedStockPart.Length);

            foreach (string warehousePart in splitedStockPart)
            {
                string[] splitedWarehousePart = warehousePart.Split(',');
                if (splitedWarehousePart.Length == 2)
                {
                    string warehouseName = splitedWarehousePart[0];
                    string itemAmount = splitedWarehousePart[1];

                    bool isAmountParsedSuccessfully = int.TryParse(itemAmount, out int amount);
                    isStockPartValid &= isAmountParsedSuccessfully;

                    if (isAmountParsedSuccessfully)
                    {
                        amountPerWarehouse.Add(new AmountInWarehouse(warehouseName, amount));
                    }
                }
                else
                {
                    isStockPartValid = false;
                }
            }

            return (isStockPartValid, amountPerWarehouse);
        }
        private bool IsCommentLine(string line)
        {
            return line.StartsWith("#");
        }


        private readonly struct AmountInWarehouse
        {
            public readonly string WarehouseName;
            public readonly int Amount;


            public AmountInWarehouse(string name, int count)
            {
                WarehouseName = name;
                Amount = count;
            }
        }
    }
}
