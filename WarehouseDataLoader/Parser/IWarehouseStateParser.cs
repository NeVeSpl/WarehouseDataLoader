using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser
{
    internal interface IWarehouseStateParser
    {
        ParsingResult GetResult();
        void ParseLine(string line);
    }
}
