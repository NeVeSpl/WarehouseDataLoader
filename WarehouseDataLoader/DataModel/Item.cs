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
        public int Quantity
        {
            get;
            set;
        }


        public Item(string id, string name, int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }
    }
}
