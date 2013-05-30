﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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
        private VisibleObject _icon = null;
        private bool _equipped = false;
        private string _name = "";
        private string _description = "";
        private bool _destroyed = false;
        private ContentManager _content = null;
        List<ItemEffect> _effects = null;

        public virtual void EquipItem()
        {
            if (_owner != null)
            {
                if (!_equipped)
                {
                    _equipped = true;
                    foreach (ItemEffect effect in _effects)
                    {
                        ChangeEffect(effect, true);
                    }


                }
                else
                {
                    _equipped = false;
                    foreach (ItemEffect effect in _effects)
                    {
                        ChangeEffect(effect, false);
                    }
                }
            }
        }

        public virtual void UseItem()
        {
            if (_owner != null)
            {
                _destroyed = true;
                foreach (ItemEffect effect in _effects)
                {
                    ChangeEffect(effect, true);
                }
            }
        }


        public void ChangeEffect(ItemEffect effect, bool enable)
        {
            if (_owner != null)
            {
                switch (effect.property)
                {
                    case ItemProperty.Evade:
                        if (enable)
                        {
                            _owner.evade += effect.effect;
                        }
                        else
                        {
                            _owner.evade -= effect.effect;
                        }
                        break;
                    case ItemProperty.Block:
                        if (enable)
                        {
                            _owner.block += effect.effect;
                        }
                        else
                        {
                            _owner.block -= effect.effect;
                        }
                        break;
                    case ItemProperty.Penetrate:
                        if (enable)
                        {
                            _owner.penetrate += effect.effect;
                        }
                        else
                        {
                            _owner.penetrate -= effect.effect;
                        }
                        break;
                    case ItemProperty.ReduceDamage:
                        if (enable)
                        {
                            _owner.armor += effect.effect;
                        }
                        else
                        {
                            _owner.armor -= effect.effect;
                        }
                        break;
                    case ItemProperty.MaxHealth:
                        if (enable)
                        {
                            _owner.maxHealth += effect.effect;
                        }
                        else
                        {
                            _owner.maxHealth -= effect.effect;
                        }
                        break;
                    case ItemProperty.MaxMana:
                        if (enable)
                        {
                            _owner.maxMana += effect.effect;
                        }
                        else
                        {
                            _owner.maxMana -= effect.effect;
                        }
                        break;
                    case ItemProperty.Mana:
                        if (enable)
                        {
                            _owner.mana += effect.effect;
                        }
                        else
                        {
                            _owner.mana -= effect.effect;
                        }
                        break;
                    case ItemProperty.Health:
                        if (enable)
                        {
                            _owner.health += effect.effect;
                        }
                        else
                        {
                            _owner.health -= effect.effect;
                        }
                        break;
                    case ItemProperty.ManaRegen:
                        if (enable)
                        {
                            _owner.manaReg += effect.effect;
                        }
                        else
                        {
                            _owner.manaReg -= effect.effect;
                        }
                        break;
                    case ItemProperty.HealthRegen:
                        if (enable)
                        {
                            _owner.healthReg += effect.effect;
                        }
                        else
                        {
                            _owner.healthReg -= effect.effect;
                        }
                        break;
                    case ItemProperty.StealHealth:
                        if (enable)
                        {
                            _owner.stealHealth += effect.effect;
                        }
                        else
                        {
                            _owner.stealHealth -= effect.effect;
                        }
                        break;
                    case ItemProperty.StealMana:
                        if (enable)
                        {
                            _owner.stealMana += effect.effect;
                        }
                        else
                        {
                            _owner.stealMana -= effect.effect;
                        }
                        break;
                    case ItemProperty.FireDamage:
                        if (enable)
                        {
                            _owner.fireDamage += effect.effect;
                        }
                        else
                        {
                            _owner.fireDamage -= effect.effect;
                        }
                        break;
                    case ItemProperty.IceDamage:
                        if (enable)
                        {
                            _owner.iceDamage += effect.effect;
                        }
                        else
                        {
                            _owner.iceDamage -= effect.effect;
                        }
                        break;
                    case ItemProperty.FireProtect:
                        if (enable)
                        {
                            _owner.fireDefense += effect.effect;
                        }
                        else
                        {
                            _owner.fireDefense -= effect.effect;
                        }
                        break;
                    case ItemProperty.IceProtect:
                        if (enable)
                        {
                            _owner.iceDefense += effect.effect;
                        }
                        else
                        {
                            _owner.iceDefense -= effect.effect;
                        }
                        break;
                    case ItemProperty.DestroyWeapon:
                        if (enable)
                        {
                            _owner.destroyWeapon += effect.effect;
                        }
                        else
                        {
                            _owner.destroyWeapon -= effect.effect;
                        }
                        break;
                    case ItemProperty.DestroyArmor:
                        if (enable)
                        {
                            _owner.destroyArmor += effect.effect;
                        }
                        else
                        {
                            _owner.destroyArmor -= effect.effect;
                        }
                        break;
                }
            }

        }

        public VisibleObject icon { get { return _icon; } set { _icon = value; } }
        public bool equipped { get { return _equipped; } set { _equipped = value; } }

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

        public void Update()
        {
            if (_equipped)
            {
                foreach (ItemEffect effect in _effects)
                {
                    if (effect.duration > -1)
                    {
                        effect.duration -= 1;
                        if (effect.duration == -1)
                        {
                            ChangeEffect(effect, false);
                        }
                    }
                }
            }
        }

        public void Load(XmlReader reader)
        {
            reader.Read();
            _name = reader.GetAttribute("name");
            _icon = new VisibleObject(_content, reader.GetAttribute("iconfile"), new Rectangle(Convert.ToInt32(reader.GetAttribute("clipRectX")),
                Convert.ToInt32(reader.GetAttribute("clipRectY")),
                Convert.ToInt32(reader.GetAttribute("clipRectW")),
                Convert.ToInt32(reader.GetAttribute("clipRectH"))));
            _icon.offsetX = Convert.ToInt32(reader.GetAttribute("iconoffsetX"));
            _icon.offsetY = Convert.ToInt32(reader.GetAttribute("iconoffsetY"));
            _icon.cropX = Convert.ToInt32(reader.GetAttribute("iconcropX"));
            _icon.cropY = Convert.ToInt32(reader.GetAttribute("iconcropY"));
            _equipped = Convert.ToBoolean(reader.GetAttribute("equipped"));
            _destroyed = Convert.ToBoolean(reader.GetAttribute("destroyed"));
            _itemType = (ItemType)Enum.Parse(typeof(ItemType), reader.GetAttribute("type"));

            _name = reader.GetAttribute("name");
            _description = reader.GetAttribute("description");
            reader.Read(); // Begin Effect
            if (reader.IsEmptyElement)
            {
                reader.Read(); // End Effects
                reader.Read(); // End Item
                return;
            }
            reader.Read(); // First effect

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                _effects.Add(new ItemEffect((ItemProperty)Enum.Parse(typeof(ItemProperty), reader.GetAttribute("property")),
                Convert.ToInt32(reader.GetAttribute("influence")),
                Convert.ToInt32(reader.GetAttribute("duration"))));
                reader.Read();
            }
            reader.Read(); // End Effects
            reader.Read(); // End Item
        }

        public void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("Item");
            if (_icon != null)
            {
                xmlw.WriteAttributeString("iconfile", Convert.ToString(_icon.src));
                xmlw.WriteAttributeString("iconoffsetX", Convert.ToString(_icon.offsetX));
                xmlw.WriteAttributeString("iconoffsetY", Convert.ToString(_icon.offsetY));
                xmlw.WriteAttributeString("iconcropX", Convert.ToString(_icon.cropX));
                xmlw.WriteAttributeString("iconcropY", Convert.ToString(_icon.cropY));
                xmlw.WriteAttributeString("clipRectX", Convert.ToString(_icon.clipRect.X));
                xmlw.WriteAttributeString("clipRectY", Convert.ToString(_icon.clipRect.Y));
                xmlw.WriteAttributeString("clipRectW", Convert.ToString(_icon.clipRect.Width));
                xmlw.WriteAttributeString("clipRectH", Convert.ToString(_icon.clipRect.Height));
            }
            xmlw.WriteAttributeString("equipped", Convert.ToString(_equipped));
            xmlw.WriteAttributeString("type", Convert.ToString(_itemType));

            xmlw.WriteAttributeString("destroyed", Convert.ToString(_destroyed));
            xmlw.WriteAttributeString("name", Convert.ToString(name));
            xmlw.WriteAttributeString("description", Convert.ToString(_description));
            xmlw.WriteStartElement("Effects");

            foreach (ItemEffect effect in _effects)
            {
                xmlw.WriteStartElement("Effect");
                xmlw.WriteAttributeString("property", Convert.ToString(effect.property));
                xmlw.WriteAttributeString("influence", Convert.ToString(effect.effect));
                xmlw.WriteAttributeString("duration", Convert.ToString(effect.duration));
                xmlw.WriteEndElement();
            }
            xmlw.WriteEndElement();
            xmlw.WriteEndElement();

        }

        public bool destroyed
        {
            get
            {
                return _destroyed;
            }
            set
            {
                _destroyed = value;
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


        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
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
                    _name = "Armor";
                    break;
                case ItemType.Weapon:
                    _name = "Sword";
                    break;
                case ItemType.Potion:
                    _name = "Potion";
                    break;
            }
            if (_effects.Count == 0)
            {
                _name = "Broken " + _name;
            }
            for (int i = 0; i < _effects.Count; ++i)
            {
                switch (_effects[i].property)
                {
                    case ItemProperty.Block:
                        {
                            if ((_itemType == ItemType.Weapon) || (_itemType == ItemType.Potion))
                            {
                                if (_name.IndexOf("Defensive") < 0)
                                    _name = "Defensive " + _name;
                            }
                            else
                            {
                                if (_name.IndexOf("Sturdy") < 0)
                                    _name = "Sturdy " + _name;
                            }
                        }
                        break;
                    case ItemProperty.Health:
                    case ItemProperty.HealthRegen:
                    case ItemProperty.MaxHealth:
                        {
                            if (_name.IndexOf("Heal") < 0)

                                _name = _name + " of Healing";

                        }
                        break;
                    case ItemProperty.DestroyArmor:
                    case ItemProperty.DestroyWeapon:
                        {
                            if (_name.IndexOf("Destructive") < 0)
                                _name = "Destructive " + _name; ;
                        }
                        break;
                    case ItemProperty.Evade:
                        {
                            if (_name.IndexOf("Evasive") < 0)
                                _name = "Evasive " + _name;
                        }
                        break;
                    case ItemProperty.FireDamage:
                    case ItemProperty.FireProtect:
                        if (_name.IndexOf("Fiery") < 0)
                            _name = "Fiery " + _name;
                        break;
                    case ItemProperty.IceProtect:
                    case ItemProperty.IceDamage:
                        if (_name.IndexOf("Icy") < 0)
                            _name = "Icy " + _name;
                        break;

                    case ItemProperty.Mana:
                    case ItemProperty.ManaRegen:
                    case ItemProperty.MaxMana:
                        if (_name.IndexOf("Magic") < 0)
                            _name = "Magic " + _name;
                        break;

                    case ItemProperty.Penetrate:
                        if (_name.IndexOf("Penetrating") < 0)
                            _name = "Penetrating " + _name;
                        break;

                    case ItemProperty.ReduceDamage:
                        if (_name.IndexOf("Protective") < 0)
                            _name = "Protective " + _name;
                        break;
                    case ItemProperty.StealHealth:
                    case ItemProperty.StealMana:
                        if (_name.IndexOf("Vampiric") < 0)
                            _name = "Vampiric " + _name;
                        break;
                }

            }
        }

        public void GenerateProperties(Random r = null)
        {
            if (r == null) r = new Random();

            int properties = r.Next(3);
            for (int i = 0; i < r.Next(4); ++i)
            {
                _effects.Add(new ItemEffect((ItemProperty)r.Next(Enum.GetValues(typeof(ItemProperty)).Length),
                10 - r.Next(20), r.Next(5) - 1));
            }
        }

        public void Pickup(Actor actor)
        {
            if (_owner != null)
            {
                _owner.inventory.Remove(this);
            }
            _owner = actor;
            actor.inventory.Add(this);
            _tile = null;
        }


        public void Drop(FloorTile tile)
        {
            if (_owner != null)
                _owner.inventory.Remove(this);
            _owner = null;
            _tile = new ItemTile(tile, this);
            tile.Add(_tile);
        }


        public void GenerateIcon()
        {
            /*

                        _environment[1].Add("items", 0, new Rectangle(0, 0, 64, 64));
            _environment[1].Add("items", 1, new Rectangle(64, 0, 64, 64));
            _environment[1].Add("items", 2, new Rectangle(128, 0, 64, 64));
            _environment[1].Add("items", 3, new Rectangle(192, 0, 64, 64));
            _environment[1].Add("items", 4, new Rectangle(320, 0, 64, 64));
            _environment[1].Add("items", 5, new Rectangle(160, 256, 32, 32));
             */
            switch (itemType)
            {
                case ItemType.Armor:
                    _icon = new VisibleObject(_content, "items", new Rectangle(0, 0, 64, 64));
                    break;
                case ItemType.Weapon:
                    _icon = new VisibleObject(_content, "items", new Rectangle(320, 0, 64, 64));
                    break;
                case ItemType.Potion:
                    _icon = new VisibleObject(_content, "items", new Rectangle(160, 256, 32, 32));
                    _icon.offsetX = 16;
                    _icon.offsetY = 16;
                    break;
            }
        }

        public Item(ContentManager content, Random r = null)
            : this(content)
        {
            if (r == null) r = new Random();
            _itemType = (ItemType)r.Next(3);
            GenerateProperties(r);
            GenerateName();
            GenerateIcon();
        }

        public Item(ContentManager content, ItemType itemtype, string name = "", Random r = null, VisibleObject icon = null)
            : this(content)
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
            GenerateProperties(r);
            GenerateIcon();
        }

        public Item(ContentManager content, ItemTile parent, ItemType itemtype, string name = "", VisibleObject icon = null)
            : this(content)
        {
            _tile = parent;
            _name = name;
            _itemType = itemtype;
            if (icon == null) GenerateIcon();
        }

        public Item(ContentManager content, Actor owner, ItemType itemtype, string name = "", VisibleObject icon = null)
            : this(content)
        {
            _owner = owner;

        }

        public Item(ContentManager content)
        {
            _effects = new List<ItemEffect>();
            _content = content;
        }
    }
}
