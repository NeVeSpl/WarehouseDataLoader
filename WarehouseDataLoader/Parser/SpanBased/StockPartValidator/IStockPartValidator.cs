using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.SpanBased.StockPartValidator
{
    internal interface IStockPartValidator
    {
        bool Validate(in ReadOnlySpan<char> stockPart);
    }
}
