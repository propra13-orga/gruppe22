using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class ItemTile : Tile
    {
        Item _item = null;

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

        public ItemTile(object parent, Item item)
            : base(parent)
        {
            _item = item;
        }



        public ItemTile()
            : base(null, null, true)
        {
            _parent = parent;
        }

        public ItemTile(object parent)
            : base(parent, null, true)
        {
            _parent = parent;

        }
    }
}
