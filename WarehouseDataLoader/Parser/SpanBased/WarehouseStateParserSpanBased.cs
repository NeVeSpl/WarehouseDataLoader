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
        private readonly IParserState state;
        private readonly List<string> invalidLines = new List<string>();
        private readonly IStringPool stringPool;


        public WarehouseStateParserSpanBased(IParserState state, IStringPool stringPool)
        {
            this.state = state;
            this.stringPool = stringPool;
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
                ParseWarehouseAndAmount(splitResult.leftPart, itemName, itemId);
                stockPart = splitResult.rightPart;
            }

            ParseWarehouseAndAmount(stockPart, itemName, itemId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ParseWarehouseAndAmount(in ReadOnlySpan<char> stockPart, string itemName, string itemId)
        {
            var splitResult = Split(stockPart, ',');
            string warehouseName = stringPool.GetString(splitResult.leftPart);
            int itemAmount = ConvertSpanToInt(splitResult.rightPart);

            state.AddItemToWarehouse(itemName, itemId, itemAmount, warehouseName);
        }



        private enum StockPartValidationState
        {
            VerticalBar,
            WarehouseName,
            Comma,
            ItemAmount,
        }
        private bool IsStockPartValid(in ReadOnlySpan<char> stockPart)
        {
            var state = StockPartValidationState.VerticalBar;
            foreach (char c in stockPart)
            {
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
        private SplitResult Split(in ReadOnlySpan<char> line, char delimiter)
        {
            int index = line.IndexOf(delimiter);

            if (index == -1)
            {
                return new SplitResult(false);
            }

            return new SplitResult(line.Slice(0, index), line.Slice(index + 1));
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
