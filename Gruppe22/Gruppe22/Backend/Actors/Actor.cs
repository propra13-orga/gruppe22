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
        #region protected Fields
        protected Random _random;
        protected int _newItems = 0;
        protected ActorTile _tile;
        protected ActorType _actorType;
        protected int _id = 0;
        protected string _name = "";
        protected List<Item> _inventory = null;
        //protected int _deadcounter = 0;
        protected int _mana = 50;
        protected int _evade = 0;
        protected int _block = 0;
        protected int _penetrate = 0;
        protected int _healthReg = 0;
        protected int _stealHealth = 0;
        protected int _stealMana = 0;
        protected int _fireDamage = 0;
        protected int _iceDamage = 0;
        protected int _fireDefense = 0;
        protected int _iceDefense = 0;
        protected int _destroyWeapon = 0;
        protected int _destroyArmor = 0;
        protected int _maxMana = 100;
        protected int _manaReg = 0;
        protected int _gold = 0;
        protected bool _locked = false;
        protected int _level = 0;
        protected int _damage = 0;
        protected int _resist = 0;
        protected int _exp = 0;
        int regCounter = 0;
        protected int _expNeeded = 0;
        protected int _maxhealth = 100;
        protected int _health = 50;
        public List<int> _quicklist = null;
        protected int _armor = 40;
        protected int _abilityPoints = 0;
        protected int _skills = 0;
        protected ContentManager _content;
        protected int _viewRange = 4;
        protected string _animationFile = "player";
        protected int _stunned = 0;
        protected int _charmed = 0;
        protected int _scared = 0;
        protected List<Ability> _abilities = null;
        private bool _regenerating = false;
        #endregion

        #region Public Fields
        /*        public int deadcounter
        {
            get { return _deadcounter; }
            set { _deadcounter = value; }
        }*/


        public Item Items(int i)
        {
            for (int count = 0; count < inventory.Count; ++count)
            {
                if (_inventory[count].id == i) return _inventory[count];
            }
            return null;

        }

        public int newItems
        {
            get
            {
                return _newItems;
            }
            set
            {
                _newItems = value;
            }
        }

        public void Regen()
        {
            if (_regenerating == false)
            {
                _regenerating = true;
                regCounter += 1;
                if (regCounter > 101)
                    regCounter = 0;
                if ((regCounter % (101 - _manaReg) == 0) && (regCounter / (101 - _manaReg) > 0))
                {
                    if (_mana < _maxMana)
                        _mana += 1;
                }
                if ((regCounter % (101 - _healthReg) == 0) && (regCounter / (101 - _healthReg) > 0))
                {
                    if (_health < _maxhealth)
                        _health += 1;
                }
                _regenerating = false;
            }
        }

        public int viewRange
        {
            get
            {
                return _viewRange;
            }
            set
            {
                _viewRange = value;
            }
        }


        public int scared
        {
            get { return _scared; }
            set { _scared = value; }
        }

        public int stunned
        {
            get { return _stunned; }
            set { _stunned = value; }
        }

        public int charmed
        {
            get { return _charmed; }
            set { _charmed = value; }
        }

        public List<int> quickList
        {
            get
            {
                return _quicklist;
            }
            set
            {
                _quicklist = value;
            }
        }

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
        public List<Ability> abilities
        {
            get
            {
                return _abilities;
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
            _abilityPoints += 10;
            _skills += 1;
            _expNeeded = 3 * (_level + 1) ^ 2 + 83 * (_level + 1) + 41;
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
            writer.WriteAttributeString("file", Convert.ToString(_animationFile));
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
            writer.WriteAttributeString("viewRange", Convert.ToString(_viewRange));
            writer.WriteAttributeString("stunned", Convert.ToString(_stunned));
            writer.WriteAttributeString("charmed", Convert.ToString(_charmed));
            writer.WriteAttributeString("scared", Convert.ToString(_scared));

            if (actorType == ActorType.NPC)
            {
                NPC n = this as NPC;
                if (n != null)
                {
                    writer.WriteAttributeString("love", Convert.ToString(n.love));
                    writer.WriteAttributeString("hasShop", Convert.ToString(n.hasShop));

                }
            }

            writer.WriteStartElement("Inventory");
            foreach (Item item in _inventory)
            {
                item.Save(writer);
            }
            writer.WriteEndElement();


            writer.WriteStartElement("Abilities");
            foreach (Ability ability in _abilities)
            {
                ability.Save(writer);
            }
            writer.WriteEndElement();



            writer.WriteStartElement("Toolbar");
            for (int i = 0; i < 10; ++i)
            {
                writer.WriteStartElement("Ability" + i.ToString());
                writer.WriteAttributeString("id", _quicklist[i].ToString());
                writer.WriteEndElement();
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
            foreach (Item i in _inventory)
            {
                i.owner = this;
            }
            _level = a.level;
            _animationFile = a.animationFile;
            _manaReg = a.manaReg;
            _maxhealth = a.maxHealth;
            _maxMana = a.maxMana;
            _name = a.name;
            _evade = a.evade;
            _block = a.block;
            _penetrate = a.penetrate;
            _healthReg = a.healthReg;
            _skills = a.skills;
            _abilityPoints = a.abilityPoints;
            _armor = a.armor;
            _stealHealth = a.stealHealth;
            _stealMana = a.stealMana;
            _fireDamage = a.fireDamage;
            _iceDamage = a.iceDamage;
            _fireDefense = a.fireDefense;
            _iceDefense = a.iceDefense;
            _expNeeded = a.expNeeded;
            _exp = a.exp;
            _resist = a.resist;
            _viewRange = a.viewRange;
            _damage = a.damage;
            _level = a.level;
            _locked = a.locked;
            _manaReg = a.manaReg;
            _maxMana = a.maxMana;
            _destroyWeapon = a.destroyWeapon;
            _destroyArmor = a.destroyArmor;
            _scared = a.scared;
            _stunned = a.stunned;
            _charmed = a.charmed;
            _quicklist = a.quickList;
            _abilities = a.abilities;

            if ((a.actorType == ActorType.NPC) && (actorType == ActorType.NPC))
            {
                NPC n = a as NPC;
                NPC t = this as NPC;

                if (a != null)
                {
                    t.love = n.love;
                    t.hasShop = n.hasShop;
                }
            }
        }

        public void Load(XmlReader reader)
        {
//            System.Diagnostics.Debug.WriteLine(reader.Name);

            _newItems = 0;
            _name = reader.GetAttribute("name");
            _maxhealth = Convert.ToInt32(reader.GetAttribute("maxhp"));
            if (reader.GetAttribute("file") != null) _animationFile = Convert.ToString("file");
            if (reader.GetAttribute("viewRange") != null) _viewRange = Convert.ToInt32(reader.GetAttribute("viewRange"));
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
            if (reader.GetAttribute("stunned") == null)
                _stunned = Convert.ToInt32(reader.GetAttribute("stunned"));
            if (reader.GetAttribute("charmed") == null)
                _charmed = Convert.ToInt32(reader.GetAttribute("charmed"));
            if (reader.GetAttribute("scared") == null)
                _scared = Convert.ToInt32(reader.GetAttribute("scared"));

            if (actorType == ActorType.NPC)
            {
                NPC n = this as NPC;
                if (n != null)
                {
                    n.love = Convert.ToInt32(reader.GetAttribute("love"));
                    n.hasShop = Convert.ToBoolean(reader.GetAttribute("hasShop"));

                }
            }
            reader.Read();

            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    Item item = new Item(_content);
                    item.Load(reader);
                    item.owner = this;
                    _inventory.Add(item);
                    if (item.isNew)
                        _newItems += 1;
                    reader.Read();
                }
                reader.ReadEndElement();
            }

            // System.Diagnostics.Debug.WriteLine(reader.Name);
            if (reader.Name == "Abilities")
            {
                // System.Diagnostics.Debug.WriteLine("Abilities");
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                }
                else
                {
                    reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        Ability ability = new Ability(_content);
                        ability.Load(reader);
                        _abilities.Add(ability);
                        reader.Read();
                    }
                    reader.ReadEndElement();

                }

                // Read Quickbar / Common attacks (ranked by frequency)
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                }
                else
                {
                    // System.Diagnostics.Debug.WriteLine("Quickbar");

                    int id = 0;
                    reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        // System.Diagnostics.Debug.WriteLine(reader.Name);

                        if (reader.HasAttributes)
                        {
                            _quicklist[id] = Convert.ToInt32(reader.GetAttribute(0));
                            //     System.Diagnostics.Debug.WriteLine(reader.GetAttribute(0));
                            ++id;
                        }
                        reader.Read();
                    }
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
                // System.Diagnostics.Debug.WriteLine(reader.Name);
            }
            else return;
        }

        public override string ToString()
        {
            return base.ToString() + " (" + _name + ")";
        }

        /// <summary>
        /// Methode to generate random names for actors
        /// </summary>
        /// <param name="r"></param>
        public void GenerateName()
        {
            int index = _random.Next(6);
            switch (_actorType)
            {
                case ActorType.NPC:
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
                    index = _random.Next(6);
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

            }
        }

        public string GenerateName(string filename)
        {
            using (TextReader reader = new StreamReader(filename))
            {
                int index = _random.Next(File.ReadAllLines(filename).Length);
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
        public Actor(ContentManager content, ActorType actorType, int health, int armor, int damage, int maxHealth = -1, string name = "", Random rnd = null, string animationFile = "", int level = -1)
        {
            _content = content;
            _actorType = actorType;
            _abilities = new List<Ability>();
            _quicklist = new List<int>(10);
            for (int i = 0; i < 10; ++i)
            {
                _quicklist.Add(0);
            }
            if (rnd == null) _random = new Random(); else _random = rnd;

            if (level < 0)
            {
                _level = 0;
                for (int counter = level; counter < 0; counter += 1)
                {
                    _exp = 3 * (_level) ^ 2 + 83 * (_level) + 41;

                    LevelUp();
                }
            }
            else
            {
                _level = level;
                _expNeeded = 3 * (_level + 1) ^ 2 + 83 * (_level + 1) + 41;

            }
            if (health < 0)
            {
                if (maxHealth > 0)
                {
                    health = 5 + _random.Next(maxHealth - 5);
                }
                else
                {
                    health = 15 + _random.Next(30);
                }
            }
            this._health = health;
            if (maxHealth == -1)
            {
                _maxhealth = health;
            }
            if (armor < 0)
            {
                armor = _random.Next(10);
            }
            this._armor = armor;
            if (damage < 0)
            {
                damage = 12 + _random.Next(10);
            }
            this._damage = damage;
            if (name.Trim() == "") GenerateName();
            else _name = name;
            this._inventory = new List<Item>();
            _animationFile = animationFile;
        }

        #endregion

    }
}
