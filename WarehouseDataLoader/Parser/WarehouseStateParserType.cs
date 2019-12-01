using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser
{
    public enum WarehouseStateParserType
    {
        SplitBased,
        RegexBased,
        SpanBased,
        SpanBasedWithStringPool,
        IndexBased,
    }
}
