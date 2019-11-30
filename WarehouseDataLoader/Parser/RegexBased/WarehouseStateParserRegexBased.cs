using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.RegexBased
{
    internal sealed class WarehouseStateParserRegexBased : IWarehouseStateParser
    {
        private readonly IWarehouse warehouse;
        private readonly List<string> invalidLines = new List<string>();
        private static readonly Regex pattern = new Regex(@"^([^;]*);([^;]*);([^,]*),(\d+)(?:\|([^,]*),(\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public WarehouseStateParserRegexBased(IWarehouse warehouse)
        {
            this.warehouse = warehouse;
        }


        public ParsingResult GetResult()
        {
            return new ParsingResult(invalidLines, warehouse.Shelves);
        }

        public void ParseLine(string line)
        {
            if (IsCommentLine(line))
            {
                return;
            }

            Match match = pattern.Match(line);

            if (match.Success)
            {
                string itemName = match.Groups[1].Captures[0].Value;
                string itemId = match.Groups[2].Captures[0].Value;
                string shelf = match.Groups[3].Captures[0].Value;
                string itemQuantityString = match.Groups[4].Captures[0].Value;
                int itemQuantity = int.Parse(itemQuantityString);
                warehouse.AddItemToShelf(itemId, itemName, itemQuantity, shelf);

                for (int i = 0; i < match.Groups[5].Captures.Count; ++i)
                {
                    shelf = match.Groups[5].Captures[i].Value;
                    itemQuantityString = match.Groups[6].Captures[i].Value;
                    itemQuantity = int.Parse(itemQuantityString);
                    warehouse.AddItemToShelf(itemId, itemName, itemQuantity, shelf);
                }
            }
            else
            {
                invalidLines.Add(line);
            }
        }


        private bool IsCommentLine(string line)
        {
            return line.StartsWith("#");
        }
    }
}
