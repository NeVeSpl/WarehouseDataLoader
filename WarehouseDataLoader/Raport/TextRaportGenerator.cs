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
            var orderedWarehouses = parsingResult.Warehouses.OrderByDescending(x => x.TotalNumberOfItems).ThenByDescending(x => x.Name);
            StringBuilder result = new StringBuilder();

            foreach (Warehouse warehouse in orderedWarehouses)
            {
                result.AppendLine($"{warehouse.Name} (total {warehouse.TotalNumberOfItems})");

                var orderedItems = warehouse.Items.OrderBy(x => x.Id);

                foreach (Item item in orderedItems)
                {
                    result.AppendLine($"{item.Id}: {item.Amount}");
                }

                result.AppendLine();
            }

            return result.ToString().TrimEnd();
        }
    }
}
