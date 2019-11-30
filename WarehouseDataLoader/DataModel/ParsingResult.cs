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
        public IReadOnlyCollection<Shelf> Shelves
        {
            get;
        }


        public ParsingResult(IEnumerable<string> invalidLines, IEnumerable<Shelf> shelves)
        {
            InvalidLines = invalidLines.ToArray();
            Shelves = shelves.ToArray();
        }
    }
}
