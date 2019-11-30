using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Raport
{
    internal sealed class TextRaportGenerator
    {
        public string Generate(ParsingResult parsingResult)
        {
            var orderedShelves = parsingResult.Shelves.OrderByDescending(x => x.TotalQuantity).ThenBy(x => x.Name);
            var result = new StringBuilder();

            foreach (Shelf shelf in orderedShelves)
            {
                result.AppendLine($"{shelf.Name} (total {shelf.TotalQuantity})");

                var orderedItems = shelf.Items.OrderBy(x => x.Id);

                foreach (Item item in orderedItems)
                {
                    result.AppendLine($"{item.Id}: {item.Quantity}");
                }

                result.AppendLine();
            }

            result.AppendLine();
            result.AppendLine("# Invalid lines");
            
            foreach (string invalidLine in parsingResult.InvalidLines)
            {
                result.AppendLine(invalidLine);
            }

            return result.ToString().TrimEnd();
        }
    }
}
