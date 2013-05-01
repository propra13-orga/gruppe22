using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class ItemTile : Tile
    {
        Item _item;
        public Item item
        {
            get { return _item; }
        }
        public ItemType itemType
        {
            get
            {
                return _item.itemType;
            }
        }
    }
}
