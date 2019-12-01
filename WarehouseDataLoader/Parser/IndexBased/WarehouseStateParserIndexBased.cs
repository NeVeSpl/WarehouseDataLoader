using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.IndexBased.StockPartValidator;

namespace WarehouseDataLoader.Parser.IndexBased
{
    internal sealed class WarehouseStateParserIndexBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly IStockPartValidator stockPartValidator;
        private readonly List<string> invalidLines = new List<string>();


        public WarehouseStateParserIndexBased(IWarehouse warehouse, IStockPartValidator stockPartValidator)
        {
            this.warehouse = warehouse;
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

            bool isLineValid = false;
            int currentIndex = 0;

            string? itemName = ParseString(line, ref currentIndex, ';');
            string? itemId = ParseString(line, ref currentIndex, ';');

            if ((!String.IsNullOrEmpty(itemName)) && (!String.IsNullOrEmpty(itemId)))
            {
                isLineValid = stockPartValidator.Validate(line, currentIndex);
                if (isLineValid)
                {                   
                    while(currentIndex < line.Length)
                    {
                        string?  shelf = ParseString(line, ref currentIndex, ',');
                        int quantity = ParseInt(line, ref currentIndex, '|');
                        warehouse.AddItemToShelf(itemId, itemName, quantity, shelf!);
                    }
                }
            }

            if (!isLineValid)
            {
                invalidLines.Add(line);
            }
        }

        private string? ParseString(string line, ref int currentIndex, char delimiter)
        {            
            int indexOfDelimiter = line.IndexOf(delimiter, currentIndex);
            if (indexOfDelimiter > -1)
            {
                int tempIndex = currentIndex;
                currentIndex = indexOfDelimiter + 1;
                return line.Substring(tempIndex, indexOfDelimiter - tempIndex);                
            }
            return null;
        }

        private int ParseInt(string line, ref int currentIndex, char delimiter)
        {           
            int indexOfDelimiter = line.IndexOf(delimiter, currentIndex);
            int endIndex = indexOfDelimiter == -1 ? line.Length : indexOfDelimiter;
            int result = ConvertStringRangeToInt(line, currentIndex, endIndex - 1);
            currentIndex = indexOfDelimiter == -1 ? line.Length : indexOfDelimiter + 1;
            return result;
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