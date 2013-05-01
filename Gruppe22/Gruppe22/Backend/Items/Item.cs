﻿using System;
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
        private ItemType _itemType;
        public ItemType itemType
        {
            get
            {
                return _itemType;
            }
        }
    }
}
