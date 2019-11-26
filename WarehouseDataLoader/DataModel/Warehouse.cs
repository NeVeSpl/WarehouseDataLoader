using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.DataModel
{
    internal sealed class Warehouse
    {
        private readonly List<Item> items;
        private int totalNumberOfItems;

        public string Name
        {
            get;
        }
        public IReadOnlyCollection<Item> Items
        {
            get => items;
        }
        public int TotalNumberOfItems
        {
            get => totalNumberOfItems;
        }


        public Warehouse(string name)
        {
            Name = name;
            items = new List<Item>();
        }


        public void AddItem(Item item)
        {
            items.Add(item);
            totalNumberOfItems += item.Amount;
        }
    }
}
