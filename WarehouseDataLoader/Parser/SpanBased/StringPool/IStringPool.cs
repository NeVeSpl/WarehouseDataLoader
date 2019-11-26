using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.SpanBased.StringPool
{
    internal interface IStringPool
    {
        string GetString(in ReadOnlySpan<char> span);
    }
}
