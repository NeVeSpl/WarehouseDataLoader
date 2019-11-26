using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.IndexBased
{
    internal sealed class WarehouseStateParserIndexBased : IWarehouseStateParser
    {
        private readonly IParserState state;
        private readonly List<string> invalidLines = new List<string>();


        public WarehouseStateParserIndexBased(IParserState state)
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

            bool isLineValid = false;
            int indexOfFirstDelimiter = line.IndexOf(';');
            if (indexOfFirstDelimiter > -1)
            {
                string itemName = line.Substring(0, indexOfFirstDelimiter);
                int indexOfSecondDelimiter = line.IndexOf(';', indexOfFirstDelimiter + 1);
                if (indexOfSecondDelimiter > indexOfFirstDelimiter)
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
                ParseWarehouseAndAmount(line, stockPartFirstIndex, indexOfDelimiter - 1, itemName, itemId);
                stockPartFirstIndex = indexOfDelimiter + 1;
            }

            ParseWarehouseAndAmount(line, stockPartFirstIndex, line.Length - 1, itemName, itemId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseWarehouseAndAmount(String line, int startIndex, int endIndex, string itemName, string itemId)
        {
            var indexOfDelimiter = line.IndexOf(',', startIndex);
            string warehouseName = line.Substring(startIndex, indexOfDelimiter - startIndex);
            int itemAmount = ConvertStringRangeToInt(line, indexOfDelimiter + 1, endIndex);

            state.AddItemToWarehouse(itemName, itemId, itemAmount, warehouseName);
        }



        private enum StockPartValidationState
        {
            VerticalBar,
            WarehouseName,
            Comma,
            ItemAmount,
        }
        private bool IsStockPartValid(String line, int stockPartFirstIndex)
        {
            var state = StockPartValidationState.VerticalBar;
            for (int i = stockPartFirstIndex; i < line.Length; ++i)
            {
                char c = line[i];
                switch (state)
                {
                    case StockPartValidationState.VerticalBar:
                        if (!(c == ',') && !(c == '|'))
                        {
                            state = StockPartValidationState.WarehouseName;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.WarehouseName:
                        if (c == ',')
                        {
                            state = StockPartValidationState.Comma;
                        }
                        if (c == '|')
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.Comma:
                        if (Char.IsDigit(c))
                        {
                            state = StockPartValidationState.ItemAmount;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.ItemAmount:
                        if (c == '|')
                        {
                            state = StockPartValidationState.VerticalBar;
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
            return state == StockPartValidationState.ItemAmount;
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
