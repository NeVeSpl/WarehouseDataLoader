using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.SpanBased.StringPool;

namespace WarehouseDataLoader.Parser.SpanBased
{
    internal sealed class WarehouseStateParserSpanBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly List<string> invalidLines = new List<string>();
        private readonly IStringPool stringPool;


        public WarehouseStateParserSpanBased(IWarehouse warehouse, IStringPool stringPool)
        {
            this.warehouse = warehouse;
            this.stringPool = stringPool;
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

            var lineAsSpan = line.AsSpan();
            bool isLineValid = false;

            var splitResult = Split(lineAsSpan, ';');
            if (splitResult.isSplitted)
            {
                string itemName = stringPool.GetString(splitResult.leftPart);
                splitResult = Split(splitResult.rightPart, ';');
                if (splitResult.isSplitted)
                {
                    string itemId = stringPool.GetString(splitResult.leftPart);
                    isLineValid = IsStockPartValid(splitResult.rightPart);
                    if (isLineValid)
                    {
                        ParseStockPart(splitResult.rightPart, itemName, itemId);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private void ParseStockPart(ReadOnlySpan<char> stockPart, string itemName, string itemId)
        {
            while (true)
            {
                var splitResult = Split(stockPart, '|');
                if (!splitResult.isSplitted)
                {
                    break;
                }
                ParseShelfAndQuantity(splitResult.leftPart, itemName, itemId);
                stockPart = splitResult.rightPart;
            }

            ParseShelfAndQuantity(stockPart, itemName, itemId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseShelfAndQuantity(in ReadOnlySpan<char> stockPart, string itemName, string itemId)
        {
            var splitResult = Split(stockPart, ',');
            string shelf = stringPool.GetString(splitResult.leftPart);
            int quantity = ConvertSpanToInt(splitResult.rightPart);

            warehouse.AddItemToShelf(itemId, itemName, quantity, shelf);
        }



        private enum StockPartValidationState
        {
            VerticalBarToken,
            ShelfToken,
            CommaToken,
            QuantityToken,
        }
        private bool IsStockPartValid(in ReadOnlySpan<char> stockPart)
        {
            var state = StockPartValidationState.VerticalBarToken;
            int quantityLength = 0;
            foreach (char c in stockPart)
            {
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
                            quantityLength = 1;
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
                            else
                            {
                                quantityLength++;
                                if (quantityLength > 9)
                                {
                                    return false;
                                }
                            }
                        }
                        break;
                }
            }
            return state == StockPartValidationState.QuantityToken;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SplitResult Split(in ReadOnlySpan<char> line, char delimiter)
        {
            int index = line.IndexOf(delimiter);

            if ((index > 0) && (index < line.Length - 1))
            {
                return new SplitResult(line.Slice(0, index), line.Slice(index + 1));
            }
           
            return new SplitResult(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCommentLine(string line)
        {
            return (line.Length > 0) && (line[0] == '#');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConvertSpanToInt(in ReadOnlySpan<char> span)
        {
            int result = 0;
            for (int i = 0; i < span.Length; i++)
            {
                result = result * 10 + (span[i] - '0');
            }
            return result;
        }


        private readonly ref struct SplitResult
        {
            public readonly ReadOnlySpan<char> leftPart;
            public readonly ReadOnlySpan<char> rightPart;
            public readonly bool isSplitted;

            public SplitResult(in ReadOnlySpan<char> leftPart, in ReadOnlySpan<char> rightPart)
            {
                this.leftPart = leftPart;
                this.rightPart = rightPart;
                this.isSplitted = true;
            }
            public SplitResult(bool isSplitted)
            {
                this.leftPart = null;
                this.rightPart = null;
                this.isSplitted = isSplitted;
            }
        }
    }
}
