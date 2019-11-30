using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.IndexBased
{
    internal sealed class WarehouseStateParserIndexBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly List<string> invalidLines = new List<string>();


        public WarehouseStateParserIndexBased(IWarehouse warehouse)
        {
            this.warehouse = warehouse;
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

            bool isLineValid = false;
            int indexOfFirstDelimiter = line.IndexOf(';');
            if (indexOfFirstDelimiter > -1)
            {
                string itemName = line.Substring(0, indexOfFirstDelimiter);
                int indexOfSecondDelimiter = line.IndexOf(';', indexOfFirstDelimiter + 1);
                if (indexOfSecondDelimiter > -1)
                {
                    string itemId = line.Substring(indexOfFirstDelimiter + 1, indexOfSecondDelimiter - indexOfFirstDelimiter - 1);
                    isLineValid = IsStockPartValid(line, indexOfSecondDelimiter + 1);
                    if (isLineValid)
                    {
                        ParseStockPart(line, indexOfSecondDelimiter + 1, itemName, itemId);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private void ParseStockPart(String line, int stockPartFirstIndex, string itemName, string itemId)
        {
            while (true)
            {
                var indexOfDelimiter = line.IndexOf('|', stockPartFirstIndex);
                if (indexOfDelimiter == -1)
                {
                    break;
                }
                ParseShelfAndQuantity(line, stockPartFirstIndex, indexOfDelimiter - 1, itemName, itemId);
                stockPartFirstIndex = indexOfDelimiter + 1;
            }

            ParseShelfAndQuantity(line, stockPartFirstIndex, line.Length - 1, itemName, itemId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseShelfAndQuantity(String line, int startIndex, int endIndex, string itemName, string itemId)
        {
            var indexOfDelimiter = line.IndexOf(',', startIndex);
            string shelf = line.Substring(startIndex, indexOfDelimiter - startIndex);
            int quantity = ConvertStringRangeToInt(line, indexOfDelimiter + 1, endIndex);

            warehouse.AddItemToShelf(itemId, itemName, quantity, shelf);
        }



        private enum StockPartValidationState
        {
            VerticalBarToken,
            ShelfToken,
            CommaToken,
            QuantityToken,
        }
        private bool IsStockPartValid(String line, int stockPartFirstIndex)
        {
            var state = StockPartValidationState.VerticalBarToken;
            for (int i = stockPartFirstIndex; i < line.Length; ++i)
            {
                char c = line[i];
                switch (state)
                {
                    case StockPartValidationState.VerticalBarToken:
                        if (!(c == ',') && !(c == '|'))
                        {
                            state = StockPartValidationState.ShelfToken;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.ShelfToken:
                        if (c == ',')
                        {
                            state = StockPartValidationState.CommaToken;
                        }
                        if (c == '|')
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.CommaToken:
                        if (Char.IsDigit(c))
                        {
                            state = StockPartValidationState.QuantityToken;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.QuantityToken:
                        if (c == '|')
                        {
                            state = StockPartValidationState.VerticalBarToken;
                        }
                        else
                        {
                            if (!Char.IsDigit(c))
                            {
                                return false;
                            }
                        }
                        break;
                }
            }
            return state == StockPartValidationState.QuantityToken;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommentLine(string line)
        {
            return (line.Length > 0) && (line[0] == '#');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConvertStringRangeToInt(String line, int startIndex, int endIndex)
        {
            int result = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                result = result * 10 + (line[i] - '0');
            }
            return result;
        }
    }
}
