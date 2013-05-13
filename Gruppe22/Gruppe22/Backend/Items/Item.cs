using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public enum ItemType
    {
        Treasure = 0,
        Potion = 1,
        Weapon = 2
    }
    public class Item
    {
        private ItemType _itemType=ItemType.Treasure;
        private ItemTile _tile;

        public ItemTile tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;
            }
        }
        public ItemType itemType
        {
            get
            {
                return _itemType;
            }
        }
    }
}
