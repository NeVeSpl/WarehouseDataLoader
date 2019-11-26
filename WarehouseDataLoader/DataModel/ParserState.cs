using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.DataModel
{
    internal interface IParserState
    {
        IReadOnlyCollection<Warehouse> Warehouses { get; }
        void AddItemToWarehouse(string itemName, string itemId, int itemAmount, string warehouseName);
    }

    internal sealed class ParserState : IParserState
    {
        private readonly Dictionary<string, Warehouse> warehouses = new Dictionary<string, Warehouse>();

        public IReadOnlyCollection<Warehouse> Warehouses => warehouses.Values;


        public void AddItemToWarehouse(string itemName, string itemId, int itemAmount, string warehouseName)
        {
            if (!warehouses.ContainsKey(warehouseName))
            {
                warehouses[warehouseName] = new Warehouse(warehouseName);
            }
            var newItem = new Item(itemId, itemName, itemAmount);
            warehouses[warehouseName].AddItem(newItem);
        }
    }
}
