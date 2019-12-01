using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.IndexBased.StockPartValidator
{
    internal interface IStockPartValidator
    {
        bool Validate(String line, int stockPartFirstIndex);
    }
}
