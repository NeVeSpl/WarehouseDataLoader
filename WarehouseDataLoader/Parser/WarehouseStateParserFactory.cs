﻿using System;
using WarehouseDataLoader.DataModel;
using WarehouseDataLoader.Parser.IndexBased;
using WarehouseDataLoader.Parser.IndexBased.StockPartValidator;
using WarehouseDataLoader.Parser.RegexBased;
using WarehouseDataLoader.Parser.SpanBased;
using WarehouseDataLoader.Parser.SpanBased.StringPool;
using WarehouseDataLoader.Parser.SplitBased;
using StockPartValidatorForSpanBased = WarehouseDataLoader.Parser.SpanBased.StockPartValidator.StockPartValidatorLoopBased;

namespace WarehouseDataLoader.Parser
{
    internal static class WarehouseStateParserFactory
    {
        public static IWarehouseStateParser Create(WarehouseStateParserType type)
        {
            switch (type)
            {
                case WarehouseStateParserType.SplitBased:
                    return new WarehouseStateParserSplitBased(new Warehouse());
                case WarehouseStateParserType.RegexBased:
                    return new WarehouseStateParserRegexBased(new Warehouse());
                case WarehouseStateParserType.SpanBased:
                    return new WarehouseStateParserSpanBased(new Warehouse(), new NoStringPool(), new StockPartValidatorForSpanBased());
                case WarehouseStateParserType.SpanBasedWithStringPool:
                    return new WarehouseStateParserSpanBased(new Warehouse(), new StringPool(), new StockPartValidatorForSpanBased());
                case WarehouseStateParserType.IndexBased:
                    return new WarehouseStateParserIndexBased(new Warehouse(), new StockPartValidatorLoopBased());
            }
            throw new NotImplementedException();
        }
    }
}
