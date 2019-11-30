using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.DataModel
{
    internal interface IWarehouse
    {
        IReadOnlyCollection<Shelf> Shelves { get; }
        void AddItemToShelf(string itemId, string itemName, int itemQuantity, string shelf);
    }

    internal sealed class Warehouse : IWarehouse
    {
        private readonly Dictionary<string, Shelf> shelves = new Dictionary<string, Shelf>();

        public IReadOnlyCollection<Shelf> Shelves => shelves.Values;


        public void AddItemToShelf(string itemId, string itemName, int itemQuantity, string shelf)
        {
            if (!shelves.ContainsKey(shelf))
            {
                shelves[shelf] = new Shelf(shelf);
            }            
            shelves[shelf].AddItem(itemId, itemName, itemQuantity);
        }
    }
}
