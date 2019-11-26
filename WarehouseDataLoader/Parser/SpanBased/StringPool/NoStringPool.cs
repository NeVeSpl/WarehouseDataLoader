using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.SpanBased.StringPool
{
    internal class NoStringPool : IStringPool
    {
        public string GetString(in ReadOnlySpan<char> span)
        {
            return span.ToString();
        }
    }
}
