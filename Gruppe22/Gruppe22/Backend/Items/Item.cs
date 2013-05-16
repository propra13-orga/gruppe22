using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public enum ItemType
    {
        Armor = 0,
        Potion = 1,
        Weapon = 2
    }
    public class Item
    {
        private ItemType _itemType = ItemType.Armor;
        private ItemTile _tile = null;
        private Actor _owner = null;
        private string _name = "";
        private int _strength = 0;

        public virtual void EquipItem(Actor actor)
        {
            if (actor != null)
            {
                _owner = actor;
                switch (_itemType)
                {
                    case ItemType.Armor:
                        _owner.AddProtection(_strength);
                        break;
                    case ItemType.Potion:
                        _owner.AddHealth(_strength);
                        break;
                    case ItemType.Weapon:
                        _owner.AddStrength(_strength);
                        break;
                }
            }
        }

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

        public int strength
        {
            get
            {
                return _strength;
            }
            set
            {
                _strength = value;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public ItemType itemType
        {
            get
            {
                return _itemType;
            }
        }

        public void GenerateName()
        {
            switch (_itemType)
            {
                case ItemType.Armor:
                    _name = "some rusted armor";
                    break;
                case ItemType.Weapon:
                    _name = "a broken weapon";
                    break;
                case ItemType.Potion:
                    _name = "a mediocre potion";
                    break;
            }
        }
        public Item()
        {
            Random r = new Random();
            _strength = 2 + r.Next(10);
            _itemType = (ItemType)r.Next(2);
            GenerateName();
        }

        public Item(ItemType itemtype, string name = "", int strength = -1)
            : this()
        {
            _itemType = itemtype;
            if (name != "")
            {
                _name = name;
            }
            else
            {
                GenerateName();
            }
            if (strength > 0) _strength = strength;
        }


        public Item(ItemTile parent, ItemType itemtype, string name = "", int strength = -1)
            : this()
        {
            _tile = parent;

        }


        public Item(Actor owner, ItemType itemtype, string name = "", int strength = -1)
            : this()
        {
            _owner = owner;
        }
    }
}
