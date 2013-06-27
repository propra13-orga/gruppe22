using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    public enum ItemType
    {
        Armor = 0,
        Potion = 1,
        Weapon = 2,
        Gold = 3,
        Key = 4,
        Note = 5,
        Cloak = 6,
        Bow = 7,
        Staff = 8,
        Ring = 9
    }
    public class Item
    {
        /// <summary>
        /// ID des Items.
        /// </summary>
        private int _id = 1;
        /// <summary>
        /// Level des Items. 
        /// </summary>
        private int _level = 1;
        /// <summary>
        /// Typ des Items.
        /// </summary>
        private ItemType _itemType = Backend.ItemType.Armor;
        /// <summary>
        /// Das Tile-Objekt zu dem Item.
        /// </summary>
        private ItemTile _tile = null;
        /// <summary>
        /// Der Actor zu dem dieses Item gehört.
        /// </summary>
        private Actor _owner = null;
        private ImageData _icon = null;
        /// <summary>
        /// Dieses Feld zeigt an, ob das Item von einem Actor aufgehoben wurde oder nicht.
        /// </summary>
        private bool _equipped = false;
        /// <summary>
        /// Feld für den Namen des Items.
        /// </summary>
        private string _name = "";
        /// <summary>
        /// Beschreibung des Items.
        /// </summary>
        private string _description = "";
        private int _value = 0;
        private bool _new = false;
        private bool _destroyed = false;
        private ContentManager _content = null;
        List<ItemEffect> _effects = null;

        public int value
        {
            get { return _value; }
            set { _value = value; }
        }


        public List<ItemEffect> effects
        {
            get
            {
                return _effects;
            }
        }
        public bool isNew
        {
            get
            {
                return _new;
            }
            set
            {
                _new = value;
            }
        }

        public Actor owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }

        public virtual void EquipItem()
        {
            // System.Diagnostics.Debug.WriteLine("Equip");
            if (_owner != null)
            {
                //  System.Diagnostics.Debug.WriteLine(_owner.name);
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

        /// <summary>
        /// Eigenschaft zu der ID des Items.
        /// </summary>
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
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
        public int level { get { return _level; } set { _level = value; } }
        public ImageData icon { get { return _icon; } set { _icon = value; } }
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
            _name = reader.GetAttribute("name");
            _icon = new ImageData(reader.GetAttribute("iconfile"), new Rectangle(Convert.ToInt32(reader.GetAttribute("clipRectX")),
                Convert.ToInt32(reader.GetAttribute("clipRectY")),
                Convert.ToInt32(reader.GetAttribute("clipRectW")) + Convert.ToInt32(reader.GetAttribute("iconoffsetX")) + Convert.ToInt32(reader.GetAttribute("iconcropX")),
                Convert.ToInt32(reader.GetAttribute("clipRectH")) + Convert.ToInt32(reader.GetAttribute("iconcropY")) + Convert.ToInt32(reader.GetAttribute("iconoffsetY"))),
                new Coords(Convert.ToInt32(reader.GetAttribute("iconoffsetX")),
            Convert.ToInt32(reader.GetAttribute("iconoffsetY"))),
            new Coords(Convert.ToInt32(reader.GetAttribute("iconcropX")),
                Convert.ToInt32(reader.GetAttribute("iconcropY"))));
            _equipped = Convert.ToBoolean(reader.GetAttribute("equipped"));
            _destroyed = Convert.ToBoolean(reader.GetAttribute("destroyed"));
            if (reader.GetAttribute("level") != null) _level = Convert.ToInt32(reader.GetAttribute("level"));

            if (reader.GetAttribute("id") != null) _id = Convert.ToInt32(reader.GetAttribute("id"));


            _itemType = (ItemType)Enum.Parse(typeof(ItemType), reader.GetAttribute("type"));
            if (reader.GetAttribute("new") != null) _new = Convert.ToBoolean(reader.GetAttribute("new"));
            if (reader.GetAttribute("value") != null) _value = Convert.ToInt32(reader.GetAttribute("value"));
            _description = reader.GetAttribute("description");
            reader.Read(); // Begin Effect
            if (reader.IsEmptyElement)
            {
                reader.Read(); // End Effects
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
            reader.ReadEndElement(); // End Effects
        }

        public void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("Item");
            if (_icon != null)
            {
                xmlw.WriteAttributeString("iconfile", Convert.ToString(_icon.name));
                xmlw.WriteAttributeString("iconoffsetX", Convert.ToString(_icon.offset.x));
                xmlw.WriteAttributeString("iconoffsetY", Convert.ToString(_icon.offset.y));
                xmlw.WriteAttributeString("iconcropX", Convert.ToString(_icon.crop.x));
                xmlw.WriteAttributeString("iconcropY", Convert.ToString(_icon.crop.y));
                xmlw.WriteAttributeString("clipRectX", Convert.ToString(_icon.rect.X));
                xmlw.WriteAttributeString("clipRectY", Convert.ToString(_icon.rect.Y));
                xmlw.WriteAttributeString("clipRectW", Convert.ToString(_icon.rect.Width));
                xmlw.WriteAttributeString("clipRectH", Convert.ToString(_icon.rect.Height));
            }
            xmlw.WriteAttributeString("equipped", Convert.ToString(_equipped));
            xmlw.WriteAttributeString("type", Convert.ToString(_itemType));
            xmlw.WriteAttributeString("level", Convert.ToString(_level));

            xmlw.WriteAttributeString("id", Convert.ToString(_id));

            xmlw.WriteAttributeString("value", Convert.ToString(_value));

            xmlw.WriteAttributeString("destroyed", Convert.ToString(_destroyed));
            xmlw.WriteAttributeString("name", Convert.ToString(name));
            xmlw.WriteAttributeString("description", Convert.ToString(_description));
            xmlw.WriteAttributeString("new", Convert.ToString(_new));

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

        public static string PropertyToString(ItemProperty property)
        {
            switch (property)
            {
                case ItemProperty.Evade:
                    return "Evade ";

                case ItemProperty.Block:
                    return "Block ";

                case ItemProperty.Penetrate:
                    return "Penetration ";

                case ItemProperty.ReduceDamage:
                    return "Reduce Damage ";

                case ItemProperty.StealMana:
                    return "Steal Mana ";

                case ItemProperty.IceDamage:
                    return "Ice Damage ";

                case ItemProperty.FireProtect:
                    return "Fire Protection ";

                case ItemProperty.IceProtect:
                    return "Ice Protection ";

                case ItemProperty.DestroyWeapon:
                    return "Destroy Weapon ";

                case ItemProperty.DestroyArmor:
                    return "Destroy Armor ";

                case ItemProperty.Resist:
                    return "Resist Effect ";

                case ItemProperty.Damage:
                    return "Physical Damage ";

                case ItemProperty.FireDamage:
                    return "Fire Damage ";

                case ItemProperty.StealHealth:
                    return "Steal Health ";

                case ItemProperty.MaxHealth:
                    return "Max. Health ";

                case ItemProperty.Mana:
                    return "Mana ";

                case ItemProperty.MaxMana:
                    return "Max. Mana ";

                case ItemProperty.Health:
                    return "Health ";

                case ItemProperty.ManaRegen:
                    return "Mana Regeneration ";

                case ItemProperty.HealthRegen:
                    return "Health Regeneration ";

            }
            return "";
        }

        public string abilityList
        {
            get
            {
                string result = "";
                for (int i = 0; i < _effects.Count; ++i)
                {
                    if (_effects[i].effect != 0)
                    {

                        if (_effects[i].effect < 0)
                        {
                            result += "\n<red>";
                        }
                        else
                        {
                            result += "\n<green>";
                        }
                        result += PropertyToString(_effects[i].property);
                        if (_effects[i].effect > 0)
                        {
                            result += "+" + _effects[i].effect.ToString();
                        }
                        else
                        {
                            result += "-" + (-_effects[i].effect).ToString();
                        }
                    }
                }
                return result;
            }
        }

        public void GenerateName()
        {

            switch (_itemType)
            {
                case Backend.ItemType.Cloak:
                    _name = "Cloak";
                    break;
                case Backend.ItemType.Armor:
                    _name = "Chainmail";

                    break;
                case Backend.ItemType.Weapon:
                    _name = "Sword";
                    break;
                case Backend.ItemType.Staff:
                    _name = "Staff";
                    break;
                case Backend.ItemType.Bow:
                    _name = "Bow";
                    break;
                case Backend.ItemType.Key:
                    _name = "Key";
                    break;

                case Backend.ItemType.Note:
                    _name = "Note";
                    break;
                case Backend.ItemType.Ring:
                    _name = "Ring";

                    break;
                case Backend.ItemType.Gold:
                    _name = _value.ToString() + " gold pieces";
                    break;

                case Backend.ItemType.Potion:
                    _name = "Potion";
                    break;
            }
            if ((_effects.Count == 0) && (_itemType != Backend.ItemType.Gold) && (_itemType != Backend.ItemType.Key) && (_itemType != Backend.ItemType.Note) && (_itemType != Backend.ItemType.Ring))
            {
                _name = "Broken " + _name;
            }
            for (int i = 0; i < _effects.Count; ++i)
            {
                if (_effects[i].effect < 0)
                {
                    if (_name.IndexOf("Cursed") < 0)
                        _name = "Cursed " + _name;

                }

                switch (_effects[i].property)
                {
                    case ItemProperty.Block:
                        {
                            if ((_itemType == Backend.ItemType.Weapon) || (_itemType == Backend.ItemType.Potion))
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
                                _name = "Destructive " + _name;
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
            EstimateValue();
        }

        public void EstimateValue()
        {
            if (value == 0)
            {
                switch (_itemType)
                {
                    case Backend.ItemType.Cloak:
                    case Backend.ItemType.Armor:
                        value = 30;
                        break;
                    case Backend.ItemType.Gold:
                        value = 100;
                        break;
                    case Backend.ItemType.Potion:
                        value = 10;
                        break;
                    case Backend.ItemType.Weapon:
                    case Backend.ItemType.Staff:
                    case Backend.ItemType.Bow:
                        value = 20;
                        break;
                    default:
                        value = 0;
                        break;
                }
                if (value > 0)
                    foreach (ItemEffect effect in _effects)
                    {
                        if (effect.effect < 0)
                        {
                            value -= 2;
                        }
                        else
                        {
                            value += effect.effect * 2;
                        }
                        if (effect.duration > 0) value += effect.duration;
                    }
            }
        }

        public void GenerateProperties(Random r = null)
        {
            if (r == null) r = new Random();
            /*
            int properties = r.Next(3);
            for (int i = 0; i < r.Next(4); ++i)
            {
                _effects.Add(new ItemEffect((ItemProperty)r.Next(Enum.GetValues(typeof(ItemProperty)).Length),
                10 - r.Next(20), r.Next(5) - 1));
            } */

            switch (_itemType)
            {
                case Backend.ItemType.Cloak:
                case Backend.ItemType.Armor:
                    _effects.Add(new ItemEffect(ItemProperty.ReduceDamage, 5 * _level + r.Next(10)));
                    break;
                case Backend.ItemType.Weapon:
                case Backend.ItemType.Staff:
                case Backend.ItemType.Bow:
                    _effects.Add(new ItemEffect(ItemProperty.Damage, 5 * _level + r.Next(10)));
                    if (r.Next(100) > 50)
                    {
                        _effects.Add(new ItemEffect(ItemProperty.Penetrate, 2 * (_level + r.Next(5))));
                    }
                    if (r.Next(100) > 50)
                    {
                        _effects.Add(new ItemEffect(ItemProperty.Evade, 2 * (_level + r.Next(5))));
                    }
                    if (r.Next(100) > 50)
                    {
                        _effects.Add(new ItemEffect(ItemProperty.FireDamage, 2 * (_level + r.Next(5))));
                    }
                    if (r.Next(100) > 50)
                    {
                        _effects.Add(new ItemEffect(ItemProperty.IceDamage, 2 * (_level + r.Next(5))));
                    }
                    break;

                case Backend.ItemType.Gold:
                case Backend.ItemType.Key:
                case Backend.ItemType.Note:
                case Backend.ItemType.Ring:
                    break;

                case Backend.ItemType.Potion:
                    switch (r.Next(2))
                    {
                        case 0:
                            _effects.Add(new ItemEffect(ItemProperty.Health, 5 + 10 * (_level + r.Next(5))));

                            break;
                        case 1:
                            _effects.Add(new ItemEffect(ItemProperty.Mana, 5 + 10 * (_level + r.Next(5))));

                            break;
                    }
                    break;
            }
        }

        public void Pickup(Actor actor)
        {
            if (_owner != null)
            {
                _owner.inventory.Remove(this);
            }
            _owner = actor;
            int temp = 0;
            if (_itemType != Backend.ItemType.Gold)
            {
                for (int i = 0; i < actor.inventory.Count; ++i)
                {
                    temp = Math.Max(id, actor.inventory[i].id);
                }
                _id = temp + 1;
                actor.inventory.Add(this);
            }
            else
            {
                actor.gold += _value;
            }
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


                case Backend.ItemType.Armor:
                    if (_effects[0].effect > 15)
                    {
                        if (_effects[0].effect > 20)
                        {
                            _icon = new ImageData("armor", new Rectangle(199, 573, 48, 48), new Backend.Coords(8, 8), new Backend.Coords(8, 8));
                        }
                        else
                        {
                            _icon = new ImageData("armor", new Rectangle(136, 573, 48, 48), new Backend.Coords(8, 8), new Backend.Coords(8, 8));
                        }
                    }
                    else
                    {
                        if (_effects[0].effect > 10)
                        {
                            _icon = new ImageData("armor", new Rectangle(8, 573, 48, 48), new Backend.Coords(8, 8), new Backend.Coords(8, 8));
                        }
                        else
                        {
                            _icon = new ImageData("armor", new Rectangle(72, 573, 48, 48), new Backend.Coords(8, 8), new Backend.Coords(8, 8));
                        }
                    }
                    break;
                case Backend.ItemType.Cloak:
                    if (_effects[0].effect > 17)
                    {
                        if (_effects[0].effect > 21)
                        {
                            _icon = new ImageData("items", new Rectangle(128, 384, 64, 64));
                        }
                        else
                        {
                            _icon = new ImageData("items", new Rectangle(192, 0, 64, 64));
                        }
                    }
                    else
                    {
                        if (_effects[0].effect > 13)
                        {
                            _icon = new ImageData("items", new Rectangle(128, 0, 64, 64));
                        }
                        else
                        {
                            if (_effects[0].effect > 9)
                            {
                                _icon = new ImageData("items", new Rectangle(64, 0, 64, 64));
                            }
                            else
                            {
                                _icon = new ImageData("items", new Rectangle(0, 0, 64, 64));
                            }
                        }
                    }

                    break;
                case Backend.ItemType.Weapon:
                    if (_effects[0].effect > 17)
                    {
                        if (_effects[0].effect > 21)
                        {
                            _icon = new ImageData("weapon", new Rectangle(8, 186, 58, 58), new Backend.Coords(3, 3), new Backend.Coords(3, 3));
                        }
                        else
                        {
                            _icon = new ImageData("weapon", new Rectangle(69, 186, 58, 58), new Backend.Coords(3, 3), new Backend.Coords(3, 3));
                        }
                    }
                    else
                    {
                        if (_effects[0].effect > 13)
                        {
                            _icon = new ImageData("weapon", new Rectangle(197, 186, 58, 58), new Backend.Coords(3, 3), new Backend.Coords(3, 3));
                        }
                        else
                        {
                            if (_effects[0].effect > 9)
                            {
                                _icon = new ImageData("weapon", new Rectangle(260, 186, 58, 58), new Backend.Coords(3, 3), new Backend.Coords(3, 3));
                            }
                            else
                            {
                                _icon = new ImageData("items", new Rectangle(320, 0, 64, 64));
                            }
                        }
                    }
                    break;
                case Backend.ItemType.Staff:
                    if (_effects[0].effect > 17)
                    {
                        if (_effects[0].effect > 21)
                        {
                            _icon = new ImageData("weapon", new Rectangle(3, 380, 63, 52), new Backend.Coords(1, 6), new Backend.Coords(0, 6));
                        }
                        else
                        {
                            _icon = new ImageData("weapon", new Rectangle(67, 380, 63, 52), new Backend.Coords(1, 6), new Backend.Coords(0, 6));
                        }
                    }
                    else
                    {
                        if (_effects[0].effect > 13)
                        {
                            _icon = new ImageData("weapon", new Rectangle(131, 380, 63, 52), new Backend.Coords(1, 6), new Backend.Coords(0, 6));
                        }
                        else
                        {
                            if (_effects[0].effect > 9)
                            {
                                _icon = new ImageData("weapon", new Rectangle(195, 380, 63, 52), new Backend.Coords(1, 6), new Backend.Coords(0, 6));
                            }
                            else
                            {
                                _icon = new ImageData("weapon", new Rectangle(260, 380, 63, 52), new Backend.Coords(1, 6), new Backend.Coords(0, 6));
                            }
                        }
                    }
                    break;
                case Backend.ItemType.Bow:
                    if (_effects[0].effect > 17)
                    {
                        if (_effects[0].effect > 21)
                        {
                            _icon = new ImageData("weapon", new Rectangle(5, 282, 60, 53), new Backend.Coords(2, 5), new Backend.Coords(2, 6));
                        }
                        else
                        {
                            _icon = new ImageData("weapon", new Rectangle(70, 282, 60, 53), new Backend.Coords(2, 5), new Backend.Coords(2, 6));
                        }
                    }
                    else
                    {
                        if (_effects[0].effect > 13)
                        {
                            _icon = new ImageData("weapon", new Rectangle(134, 282, 60, 53), new Backend.Coords(2, 5), new Backend.Coords(2, 6));
                        }
                        else
                        {
                            if (_effects[0].effect > 9)
                            {
                                _icon = new ImageData("weapon", new Rectangle(198, 282, 60, 53), new Backend.Coords(2, 5), new Backend.Coords(2, 6));
                            }
                            else
                            {
                                _icon = new ImageData("weapon", new Rectangle(262, 282, 60, 53), new Backend.Coords(2, 5), new Backend.Coords(2, 6));
                            }
                        }
                    }
                    break;
                case Backend.ItemType.Key:
                    switch (_level)
                    {
                        case 1:
                            _icon = new ImageData("questitems", new Rectangle(0, 224, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            break;
                        case 2:
                            _icon = new ImageData("questitems", new Rectangle(0, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            break;
                        case 3:
                            _icon = new ImageData("questitems", new Rectangle(0, 320, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            break;
                        default:
                            _icon = new ImageData("questitems", new Rectangle(0, 352, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            break;
                    }
                    break;

                case Backend.ItemType.Note:
                    if (_level < 3)
                    {
                        _icon = new ImageData("questitems", new Rectangle(128, 192, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));

                    }
                    else
                    {
                        _icon = new ImageData("questitems", new Rectangle(128, 159, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                    }
                    break;
                case Backend.ItemType.Ring:
                    _icon = new ImageData("armor", new Rectangle(331, 478, 47, 47), new Backend.Coords(9, 9), new Backend.Coords(8, 8));

                    break;
                case Backend.ItemType.Gold:
                    _icon = new ImageData("items", new Rectangle(381, 899, 38, 45), new Backend.Coords(13, 9), new Backend.Coords(13, 10));
                    break;

                case Backend.ItemType.Potion:
                    switch (_effects[0].property)
                    {
                        case ItemProperty.Mana:
                            if (_effects[0].effect > 30)
                            {
                                _icon = new ImageData("items", new Rectangle(160, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            }
                            else
                            {
                                _icon = new ImageData("items", new Rectangle(127, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            }
                            break;
                        case ItemProperty.Health:
                            if (_effects[0].effect > 30)
                            {
                                _icon = new ImageData("items", new Rectangle(97, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            }
                            else
                            {
                                _icon = new ImageData("items", new Rectangle(65, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            }
                            break;
                        default:
                            if (_effects[0].effect > 30)
                            {
                                _icon = new ImageData("items", new Rectangle(223, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));
                            }
                            else
                            {
                                _icon = new ImageData("items", new Rectangle(193, 256, 32, 32), new Backend.Coords(16, 16), new Backend.Coords(16, 16));

                            } break;
                    }

                    break;
            }

        }

        public Item(ContentManager content, Random r = null, int value = 0, int level = 1, bool gold = true)
            : this(content)
        {
            if (r == null) r = new Random();
            _value = value;
            _level = level;
            switch (r.Next(6 + (gold ? 1 : 0)))
            {
                case 0:
                    _itemType = Backend.ItemType.Staff;
                    break;
                case 1:
                    _itemType = Backend.ItemType.Potion;
                    break;
                case 2:
                    _itemType = Backend.ItemType.Cloak;
                    break;
                case 3:
                    _itemType = Backend.ItemType.Bow;
                    break;
                case 4:
                    _itemType = Backend.ItemType.Armor;
                    break;
                case 5:
                    _itemType = Backend.ItemType.Weapon;
                    break;
                case 6:
                    _itemType = Backend.ItemType.Gold;
                    break;
            }
            GenerateProperties(r);
            GenerateName();
            GenerateIcon();
        }

        public Item(ContentManager content, ItemType itemtype, string name = "", Random r = null, ImageData icon = null, int value = 0, int level = 1)
            : this(content)
        {
            _level = level;
            _value = value;
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

        public Item(ContentManager content, ItemTile parent, ItemType itemtype, string name = "", ImageData icon = null, int value = 0, int level = 1)
            : this(content)
        {
            _level = level;
            _tile = parent;
            _value = value;
            _itemType = itemtype;

            if (name != "")
            {
                _name = name;
            }
            else
            {
                GenerateName();
            }
            if (icon == null) GenerateIcon();
        }

        public Item(ContentManager content, Actor owner, ItemType itemtype, string name = "", ImageData icon = null, int value = 0, int level = 1)
            : this(content)
        {
            _owner = owner;
            _value = value;
            _level = level;
            _itemType = itemtype;
            if (name != "")
            {
                _name = name;
            }
            else
            {
                GenerateName();
            }
            _icon = icon;
            if (icon == null) GenerateIcon();
        }

        public Item(ContentManager content)
        {
            _effects = new List<ItemEffect>();
            _content = content;
        }
    }
}
