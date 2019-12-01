using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.Parser.SpanBased.StockPartValidator
{
    internal sealed class StockPartValidatorOnStateMachine : IStockPartValidator
    {
        private enum StockPartValidationState
        {
            VerticalBarToken,
            ShelfToken,
            CommaToken,
            QuantityToken,
        }

        public bool Validate(in ReadOnlySpan<char> stockPart)
        {
            var state = StockPartValidationState.VerticalBarToken;
            int quantityLength = 0;
            foreach (char c in stockPart)
            {
                switch (state)
                {
                    case StockPartValidationState.VerticalBarToken:
                        if (!(c == ',') && !(c == '|'))
                        {
                            state = StockPartValidationState.ShelfToken;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.ShelfToken:
                        if (c == ',')
                        {
                            state = StockPartValidationState.CommaToken;
                        }
                        if (c == '|')
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.CommaToken:
                        if (Char.IsDigit(c))
                        {
                            state = StockPartValidationState.QuantityToken;
                            quantityLength = 1;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case StockPartValidationState.QuantityToken:
                        if (c == '|')
                        {
                            state = StockPartValidationState.VerticalBarToken;
                        }
                        else
                        {
                            if (!Char.IsDigit(c))
                            {
                                return false;
                            }
                            else
                            {
                                quantityLength++;
                                if (quantityLength > 9)
                                {
                                    return false;
                                }
                            }
                        }
                        break;
                }
            }
            return state == StockPartValidationState.QuantityToken;
        }
    }
}
