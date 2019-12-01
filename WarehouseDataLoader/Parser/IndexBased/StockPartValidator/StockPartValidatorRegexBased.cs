using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WarehouseDataLoader.Parser.IndexBased.StockPartValidator
{
    internal sealed class StockPartValidatorRegexBased : IStockPartValidator
    {
        private static readonly Regex pattern = new Regex(@"[^,\|]+,\d{1,9}(?:\|[^,\|]+,\d{1,9})*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public bool Validate(string line, int stockPartFirstIndex)
        {
            Match match = pattern.Match(line, stockPartFirstIndex);
            if ((match.Success) && (match.Groups[0].Length == line.Length - stockPartFirstIndex))
            {
                return true;
            }
            return false;
        }
    }
}
