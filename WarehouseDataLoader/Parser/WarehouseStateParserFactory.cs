using System;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.IndexBased;
using WarehouseDataLoader.Parser.RegexBased;
using WarehouseDataLoader.Parser.SpanBased;
using WarehouseDataLoader.Parser.SpanBased.StringPool;
using WarehouseDataLoader.Parser.SplitBased;

namespace WarehouseDataLoader.Parser
{
    internal static class WarehouseStateParserFactory
    {
        public static IWarehouseStateParser Create(WarehouseStateParserType type)
        {
            switch (type)
            {
                case WarehouseStateParserType.SplitBased:
                    return new WarehouseStateParserSplitBased(new ParserState());
                case WarehouseStateParserType.RegexBased:
                    return new WarehouseStateParserRegexBased(new ParserState());
                case WarehouseStateParserType.SpanBased:
                    return new WarehouseStateParserSpanBased(new ParserState(), new NoStringPool());
                case WarehouseStateParserType.IndexBased:
                    return new WarehouseStateParserIndexBased(new ParserState());
            }
            throw new NotImplementedException();
        }
    }
}
