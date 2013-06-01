using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public enum ActorType
    {
        Player = 0,
        NPC = 1,
        Enemy = 2
    }

    public class Actor
    {
        #region Private Fields
        private ActorTile _tile;
        protected ActorType _actorType;
        private int _id = 0;
        private string _name = "";
        private string _animationFile = "";
        private List<Item> _inventory = null;
        private int _mana = 0;
        private int _evade = 0;
        private int _block = 0;
        private int _penetrate = 0;
        private int _healthReg = 0;
        private int _stealHealth = 0;
        private int _stealMana = 0;
        private int _fireDamage = 0;
        private int _iceDamage = 0;
        private int _fireDefense = 0;
        private int _iceDefense = 0;
        private int _destroyWeapon = 0;
        private int _destroyArmor = 0;
        private int _maxMana = 0;
        private int _manaReg = 0;
        private int _gold = 0;
        private bool _locked = false;
        //private List<Spell> _spellbook = null;
        private int _level = 0;
        private int _damage = 0;
        private int _resist = 0;
        private int _exp = 0;
        private int _expNeeded = 0;
        private int _maxhealth = 100;
        private int _health = 50;
        private int _armor = 40;
        private int _abilityPoints = 0;
        private int _skills = 0;
        private ContentManager _content;
        #endregion

        #region Public Fields
        public bool locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        public string animationFile
        {
            get { return _animationFile; }
            set { _animationFile = value; }
        }

        public ActorTile tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public List<Item> inventory
        {
            get
            {
                return _inventory;
            }
        }

        public int mana
        {
            get
            {
                return Math.Max(_mana, 0);
            }
            set
            {
                _mana = value;
            }
        }

        public int expNeeded
        {
            get
            {
                return _expNeeded;
            }
            set
            {
                _expNeeded = value;
            }
        }
        public int maxMana
        {
            get
            {
                return Math.Max(_maxMana, 0);
            }
            set
            {
                _maxMana = value;
            }
        }

        public int manaReg
        {
            get
            {
                return _manaReg;
            }
            set
            {
                _manaReg = value;
            }
        }

        public int gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
            }
        }

        public ActorType actorType
        {
            get
            {
                return _actorType;
            }
        }

        public int health
        {
            get
            {
                return Math.Max(_health, 0);
            }
            set
            {
                _health = value;
            }
        }

        public int maxHealth
        {
            get
            {
                return _maxhealth;
            }
            set
            {
                _maxhealth = value;
            }
        }

        public int armor
        {
            get
            {
                return _armor;
            }
            set
            {
                _armor = value;
            }
        }

        public int damage
        {
            get
            {
                return _damage;
            }
            set
            {
                _damage = value;
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

        public int level
        {
            get
            {
                return _level;
            }
        }


        public int exp
        {
            get
            {
                return _exp;
            }
            set
            {
                _exp = value;
            }
        }

        public bool isDead
        {
            get
            {
                return _health <= 0 ? true : false;
            }
        }

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







        public int evade
        {
            get
            {
                return _evade;
            }
            set
            {
                _evade = value;
            }
        }

        public int block
        {
            get
            {
                return _block;
            }
            set
            {
                _block = value;
            }
        }

        public int penetrate
        {
            get
            {
                return _penetrate;
            }
            set
            {
                _penetrate = value;
            }
        }

        public int healthReg
        {
            get
            {
                return _healthReg;
            }
            set
            {
                _healthReg = value;
            }
        }

        public int stealHealth
        {
            get
            {
                return _stealHealth;
            }
            set
            {
                _stealHealth = value;
            }
        }

        public int stealMana
        {
            get
            {
                return _stealMana;
            }
            set
            {
                _stealMana = value;
            }
        }

        public int fireDamage
        {
            get
            {
                return _fireDamage;
            }
            set
            {
                _fireDamage = value;
            }
        }

        public int iceDamage
        {
            get
            {
                return _iceDamage;
            }
            set
            {
                _iceDamage = value;
            }
        }

        public int fireDefense
        {
            get
            {
                return _fireDamage;
            }
            set
            {
                _fireDamage = value;
            }
        }

        public int iceDefense
        {
            get
            {
                return _iceDefense;
            }
            set
            {
                _iceDefense = value;
            }
        }

        public int destroyWeapon
        {
            get
            {
                return _destroyWeapon;
            }
            set
            {
                _destroyWeapon = value;
            }
        }

        public int destroyArmor
        {
            get
            {
                return _destroyArmor;
            }
            set
            {
                _destroyArmor = value;
            }
        }
        public int resist
        {
            get
            {
                return _resist;
            }
            set
            {
                _resist = value;
            }
        }
        public int maxhealth
        {
            get
            {
                return _maxhealth;
            }
            set
            {
                _maxhealth = value;
            }
        }

        public int abilityPoints
        {
            get
            {
                return _abilityPoints;
            }
            set
            {
                _abilityPoints = value;
            }
        }
        public int skills
        {
            get
            {
                return _skills;
            }
            set
            {
                _skills = value;
            }
        }
        #endregion

        #region Public Methods


        public void LevelUp()
        {
            _level++;
            _abilityPoints += 40;
            _skills += 1;
            _expNeeded = 3 * _level ^ 2 + 83 * _level + 41;
            _health = _maxhealth;
        }

        public void AddProtection(int amount)
        {
            if (amount > _armor)
            {
                _armor = amount;
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip  " : (_name + " equips ")) + " new armor.");
            }
            else
            {
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the armor away.");
            }
        }

        public void AddHealth(int amount)
        {
            int temp = Math.Max(amount, _maxhealth - _health - amount);
            if (temp > 0)
            {
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You regain " : (_name + " regains ")) + temp.ToString() + " hitpoints.");
            }
            else
            {
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " puts the potion away.");
            }
            _health = Math.Min(_health + amount, _maxhealth);
        }

        public void AddStrength(int amount)
        {
            if (amount > _damage)
            {
                _damage = amount;
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip " : (_name + " equips ")) + " a new weapon.");
            }
            else
            {
                _tile.HandleEvent(false, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the weapon away.");

            }
        }

        public void Save(XmlWriter writer)
        {
            switch (_actorType)
            {
                case ActorType.Enemy:
                    writer.WriteStartElement("Enemy");

                    break;
                case ActorType.NPC:
                    writer.WriteStartElement("NPC");

                    break;
                case ActorType.Player:
                    writer.WriteStartElement("Player");

                    break;
                default:
                    writer.WriteStartElement("Actor");
                    break;
            }

            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("maxhp", Convert.ToString(_maxhealth));
            writer.WriteAttributeString("hp", Convert.ToString(_health));
            writer.WriteAttributeString("level", Convert.ToString(_level));
            writer.WriteAttributeString("mana", Convert.ToString(_mana));
            writer.WriteAttributeString("evade", Convert.ToString(_evade));
            writer.WriteAttributeString("block", Convert.ToString(_block));
            writer.WriteAttributeString("penetrate", Convert.ToString(_penetrate));
            writer.WriteAttributeString("healthReg", Convert.ToString(_healthReg));
            writer.WriteAttributeString("skills", Convert.ToString(_skills));
            writer.WriteAttributeString("abilityPoints", Convert.ToString(_abilityPoints));
            writer.WriteAttributeString("armor", Convert.ToString(_armor));
            writer.WriteAttributeString("stealHealth", Convert.ToString(_stealHealth));
            writer.WriteAttributeString("stealMana", Convert.ToString(_stealMana));
            writer.WriteAttributeString("fireDamage", Convert.ToString(_fireDamage));
            writer.WriteAttributeString("iceDamage", Convert.ToString(_iceDamage));
            writer.WriteAttributeString("fireDefense", Convert.ToString(_fireDefense));
            writer.WriteAttributeString("iceDefense", Convert.ToString(_iceDefense));
            writer.WriteAttributeString("expNeeded", Convert.ToString(_expNeeded));
            writer.WriteAttributeString("exp", Convert.ToString(_exp));
            writer.WriteAttributeString("resist", Convert.ToString(_resist));
            writer.WriteAttributeString("damage", Convert.ToString(_damage));
            writer.WriteAttributeString("locked", Convert.ToString(_locked));
            writer.WriteAttributeString("gold", Convert.ToString(_gold));
            writer.WriteAttributeString("manaReg", Convert.ToString(_manaReg));
            writer.WriteAttributeString("maxMana", Convert.ToString(_maxMana));
            writer.WriteAttributeString("destroyWeapon", Convert.ToString(_destroyWeapon));
            writer.WriteAttributeString("destroyArmor", Convert.ToString(_destroyArmor));
            writer.WriteAttributeString("animation", Convert.ToString(_animationFile));



            writer.WriteStartElement("Inventory");
            foreach (Item item in _inventory)
            {
                item.Save(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public void copyFrom(Actor a)
        {
            _actorType = a.actorType;
            _armor = a.armor;
            _mana = a.mana;
            _damage = a.damage;
            _exp = a.exp;
            _expNeeded = a.expNeeded;
            _gold = a.gold;
            _health = a.health;
            _inventory = a.inventory;
            _level = a.level;
            _manaReg = a.manaReg;
            _maxhealth = a.maxHealth;
            _maxMana = a.maxMana;
            _name = a.name;

        }

        public void Load(XmlReader reader)
        {
            _name = reader.GetAttribute("name");
            _maxhealth = Convert.ToInt32(reader.GetAttribute("maxhp"));
            _health = Convert.ToInt32(reader.GetAttribute("hp"));
            _level = Convert.ToInt32(reader.GetAttribute("level"));
            _mana = Convert.ToInt32(reader.GetAttribute("mana"));
            _evade = Convert.ToInt32(reader.GetAttribute("evade"));
            _block = Convert.ToInt32(reader.GetAttribute("block"));
            _penetrate = Convert.ToInt32(reader.GetAttribute("penetrate"));
            _healthReg = Convert.ToInt32(reader.GetAttribute("healthReg"));
            _skills = Convert.ToInt32(reader.GetAttribute("skills"));
            _abilityPoints = Convert.ToInt32(reader.GetAttribute("abilityPoints"));
            _armor = Convert.ToInt32(reader.GetAttribute("armor"));
            _stealHealth = Convert.ToInt32(reader.GetAttribute("stealHealth"));
            _stealMana = Convert.ToInt32(reader.GetAttribute("stealMana"));
            _fireDamage = Convert.ToInt32(reader.GetAttribute("fireDamage"));
            _iceDamage = Convert.ToInt32(reader.GetAttribute("iceDamage"));
            _fireDefense = Convert.ToInt32(reader.GetAttribute("fireDefense"));
            _iceDefense = Convert.ToInt32(reader.GetAttribute("iceDefense"));
            _expNeeded = Convert.ToInt32(reader.GetAttribute("expNeeded"));
            _exp = Convert.ToInt32(reader.GetAttribute("exp"));
            _resist = Convert.ToInt32(reader.GetAttribute("resist"));
            _damage = Convert.ToInt32(reader.GetAttribute("damage"));
            _level = Convert.ToInt32(reader.GetAttribute("level"));
            _locked = Convert.ToBoolean(reader.GetAttribute("locked"));
            _gold = Convert.ToInt32(reader.GetAttribute("gold"));
            _manaReg = Convert.ToInt32(reader.GetAttribute("manaReg"));
            _maxMana = Convert.ToInt32(reader.GetAttribute("maxMana"));
            _destroyWeapon = Convert.ToInt32(reader.GetAttribute("destroyWeapon"));
            _destroyArmor = Convert.ToInt32(reader.GetAttribute("destroyArmor"));
            _animationFile = reader.GetAttribute("animation");
            reader.Read();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                reader.Read();

                return;
            }
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                Item item = new Item(_content);
                item.Load(reader);
                _inventory.Add(item);
                reader.Read();
            }
            reader.ReadEndElement();
            reader.ReadEndElement(); // End Effects


        }

        public override string ToString()
        {
            return base.ToString() + " (" + _name + ")";
        }

        /// <summary>
        /// Methode to generate random names for actors
        /// </summary>
        /// <param name="r"></param>
        public void GenerateName(Random r = null)
        {
            if (r == null) r = new Random();
            int index = r.Next(6);
            switch (_actorType)
            {
                case ActorType.Player:
                    if (File.Exists("playernames.txt"))
                    {
                        _name = GenerateName("playernames.txt");
                    }
                    else
                    {
                        switch (index)
                        {
                            case 0:
                                _name = "Gerd";
                                break;
                            case 1:
                                _name = "Klaus";
                                break;
                            case 2:
                                _name = "Dieter";
                                break;
                            case 3:
                                _name = "Waldemar";
                                break;
                            case 4:
                                _name = "Friedrich";
                                break;
                            case 5:
                                _name = "Othmar";
                                break;
                        }
                    }
                    break;
                case ActorType.Enemy:
                    if (File.Exists("enemynames.txt"))
                    {
                        _name = GenerateName("enemynames.txt");
                    }
                    else
                    {
                        switch (index)
                        {
                            case 0:
                                _name = "Skeletor";
                                break;
                            case 1:
                                _name = "Skeletus";
                                break;
                            case 2:
                                _name = "Skello";
                                break;
                            case 3:
                                _name = "Skeletanus";
                                break;
                            case 4:
                                _name = "Skeletti";
                                break;
                            case 5:
                                _name = "Skelly";
                                break;
                        }
                    }
                    index = r.Next(6);
                    switch (index)
                    {
                        case 0:
                            _name += " the Dirty";
                            break;
                        case 1:
                            _name += " the Killer";
                            break;
                        case 2:
                            _name += " the Strong";
                            break;
                        case 3:
                            _name += " the Legend";
                            break;
                        case 4:
                            _name += " Maximus";
                            break;
                        case 5:
                            _name += " Minimus";
                            break;
                    }
                    break;
                case ActorType.NPC:
                    if (File.Exists("npcnames.txt"))
                    {
                        _name = GenerateName("npcnames.txt");
                    }
                    break;
            }
        }

        public string GenerateName(string filename)
        {
            using (TextReader reader = new StreamReader(filename))
            {
                Random r = new Random();
                int index = r.Next(File.ReadAllLines(filename).Length);
                int i = 0;
                while (i < index && reader.ReadLine() != null)
                {
                    i++;
                }
                return reader.ReadLine();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actorType"></param>
        /// <param name="health"></param>
        /// <param name="armor"></param>
        /// <param name="damage"></param>
        public Actor(ContentManager content, ActorType actorType, int health, int armor, int damage, int maxHealth = -1, string name = "", Random r = null, string animationFile = "")
        {
            _content = content;
            this._actorType = actorType;
            if (r == null) r = new Random();
            if (health < 0)
            {
                if (maxHealth > 0)
                {
                    health = 5 + r.Next(maxHealth - 5);
                }
                else
                {
                    health = 15 + r.Next(30);
                }
            }
            this._health = health;
            if (maxHealth == -1)
            {
                _maxhealth = health;
            }
            if (armor < 0)
            {
                armor = r.Next(10);
            }
            this._armor = armor;
            if (damage < 0)
            {
                damage = 12 + r.Next(10);
            }
            this._damage = damage;
            if (name.Trim() == "") GenerateName(r);
            else _name = name;
            this._inventory = new List<Item>();
            _animationFile = animationFile;
        }

        #endregion

    }
}
