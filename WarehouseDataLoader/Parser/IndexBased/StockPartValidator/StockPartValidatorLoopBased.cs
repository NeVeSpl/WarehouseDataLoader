﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.IndexBased.StockPartValidator
{
    internal sealed class StockPartValidatorLoopBased : IStockPartValidator
    {
        public bool Validate(string line, int stockPartFirstIndex)
        {
            int tokenLength = 0;
            int i = stockPartFirstIndex;

            while (i < line.Length)
            {
                // ShelfToken
                tokenLength = 0;
                while ((i < line.Length) && !(line[i] == ',') && !(line[i] == '|'))
                {
                    tokenLength++;
                    i++;
                }
                // CommaToken
                if ((tokenLength == 0) || (i == line.Length) || (line[i] != ','))
                {
                    return false;
                }
                i++;
                // QuantityToken
                tokenLength = 0;
                while ((i < line.Length) && Char.IsDigit(line[i]))
                {
                    tokenLength++;
                    i++;
                }
                if ((tokenLength == 0) || (tokenLength > 9))
                {
                    return false;
                }
                if (i == line.Length)
                {
                    return true;
                }
                // VerticalBarToken
                if (line[i] != '|')
                {
                    return false;
                }
                i++;
            }
            return false;
        }
    }
}
