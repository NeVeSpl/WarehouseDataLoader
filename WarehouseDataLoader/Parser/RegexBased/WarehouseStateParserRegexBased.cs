using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WarehouseDataLoader.DataModel;

namespace WarehouseDataLoader.Parser.RegexBased
{
    internal sealed class WarehouseStateParserRegexBased : IWarehouseStateParser
    {
        private readonly IParserState state;
        private readonly List<string> invalidLines = new List<string>();
        private static readonly Regex pattern = new Regex(@"^([^;]*);([^;]*);([^,]*),(\d+)(?:\|([^,]*),(\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public WarehouseStateParserRegexBased(IParserState state)
        {
            this.state = state;
        }


        public ParsingResult GetResult()
        {
            return new ParsingResult(invalidLines, state.Warehouses);
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
                string warehouseName = match.Groups[3].Captures[0].Value;
                string itemAmountString = match.Groups[4].Captures[0].Value;
                int itemAmount = int.Parse(itemAmountString);
                state.AddItemToWarehouse(itemName, itemId, itemAmount, warehouseName);

                for (int i = 0; i < match.Groups[5].Captures.Count; ++i)
                {
                    warehouseName = match.Groups[5].Captures[i].Value;
                    itemAmountString = match.Groups[6].Captures[i].Value;
                    itemAmount = int.Parse(itemAmountString);
                    state.AddItemToWarehouse(itemName, itemId, itemAmount, warehouseName);
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
