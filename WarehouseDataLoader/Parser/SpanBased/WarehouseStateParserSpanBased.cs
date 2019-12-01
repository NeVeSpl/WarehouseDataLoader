using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.SpanBased.StockPartValidator;
using WarehouseDataLoader.Parser.SpanBased.StringPool;

namespace WarehouseDataLoader.Parser.SpanBased
{
    internal sealed class WarehouseStateParserSpanBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly List<string> invalidLines = new List<string>();
        private readonly IStringPool stringPool;
        private readonly IStockPartValidator stockPartValidator;


        public WarehouseStateParserSpanBased(IWarehouse warehouse, IStringPool stringPool, IStockPartValidator stockPartValidator)
        {
            this.warehouse = warehouse;
            this.stringPool = stringPool;
            this.stockPartValidator = stockPartValidator;
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

            ReadOnlySpan<char> itemNameSpan = ParseString(ref lineAsSpan, ';');
            ReadOnlySpan<char> itemIdSpan = ParseString(ref lineAsSpan, ';');

            if ((!itemNameSpan.IsEmpty) && (!itemIdSpan.IsEmpty))
            {
                isLineValid = stockPartValidator.Validate(in lineAsSpan);
                if (isLineValid)
                {
                    string itemName = stringPool.GetString(in itemNameSpan);
                    string itemId = stringPool.GetString(in itemIdSpan);
                    while (!lineAsSpan.IsEmpty)
                    {
                        ReadOnlySpan<char> shelfSpan = ParseString(ref lineAsSpan, ',');
                        int quantity = ParseInt(ref lineAsSpan, '|');
                        
                        string shelf = stringPool.GetString(in shelfSpan);
                        warehouse.AddItemToShelf(itemId, itemName, quantity, shelf);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private ReadOnlySpan<char> ParseString(ref ReadOnlySpan<char> line, char delimiter)
        {            
            int indexOfDelimiter = line.IndexOf(delimiter);
            if (indexOfDelimiter > -1)
            {
                var tempLine = line;
                line = line.Slice(indexOfDelimiter + 1);
                return tempLine.Slice(0, indexOfDelimiter);                
            }
            return ReadOnlySpan<char>.Empty;
        }

        private int ParseInt(ref ReadOnlySpan<char> line, char delimiter)
        {
            int indexOfDelimiter = line.IndexOf(delimiter);
            var intSpan = indexOfDelimiter == -1 ? line : line.Slice(0, indexOfDelimiter);
            int result = ConvertSpanToInt(in intSpan);
            line = indexOfDelimiter == -1 ? ReadOnlySpan<char>.Empty : line.Slice(indexOfDelimiter + 1);
            return result;
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
    }
}
