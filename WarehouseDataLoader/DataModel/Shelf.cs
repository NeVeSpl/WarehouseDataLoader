using System.Collections.Generic;

namespace WarehouseDataLoader.DataModel
{
    internal sealed class Shelf
    {
        private readonly Dictionary<string, Item> items;
        private int totalQuantity;

        public string Name
        {
            get;
        }
        public IReadOnlyCollection<Item> Items
        {
            get => items.Values;
        }
        public int TotalQuantity
        {
            get => totalQuantity;
        }


        public Shelf(string name)
        {
            Name = name;
            items = new Dictionary<string, Item>();
        }


        public void AddItem(string itemId, string itemName, int itemQuantity)
        {
            if (!items.ContainsKey(itemId))
            {
                items[itemId] = new Item(itemId, itemName, itemQuantity);
            }
            else
            {
                items[itemId].Quantity += itemQuantity;
            }
            totalQuantity += itemQuantity;
        }
    }
}
