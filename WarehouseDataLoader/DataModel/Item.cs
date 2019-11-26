using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseDataLoader.DataModel
{
    internal sealed class Item
    {
        public string Id
        {
            get;
        }
        public string Name
        {
            get;
        }
        public int Amount
        {
            get;
        }


        public Item(string id, string name, int amount)
        {
            Id = id;
            Name = name;
            Amount = amount;
        }
    }
}
