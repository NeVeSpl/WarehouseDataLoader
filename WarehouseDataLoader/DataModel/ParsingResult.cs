using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarehouseDataLoader.DataModel
{
    internal sealed class ParsingResult
    {
        public IReadOnlyCollection<string> InvalidLines
        {
            get;
        }
        public IReadOnlyCollection<Warehouse> Warehouses
        {
            get;
        }


        public ParsingResult(IEnumerable<string> invalidLines, IEnumerable<Warehouse> warehouses)
        {
            InvalidLines = invalidLines.ToArray();
            Warehouses = warehouses.ToArray();
        }
    }
}
