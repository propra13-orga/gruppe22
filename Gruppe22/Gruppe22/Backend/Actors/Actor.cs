using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
        private List<Item> _inventory = null;
        private int _currMana;
        private int _maxMana;
        private int _manaReg;
        private int _gold;
        private bool _locked = false;
        //private List<Spell> _spellbook = null;

        /// <summary>
        /// later used to calculate Health, Damage, etc
        /// </summary>
        private int _level;
        private int _strength;
        private int _vitality;
        private int _exp;
        private int _expNeeded;

        //Lebenspunkte, Rüstung, Schaden/Angriffsstärke
        protected int
            _maxhealth = 100, _health = 50, _armour = 40, _damage = 20;
        #endregion

        #region Public Fields
        public bool locked
        {
            get { return _locked; }
            set { _locked = value; }
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

        public int currMana
        {
            get
            {
                return _currMana;
            }
            set
            {
                _currMana = value;
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
                return _maxMana;
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
                return _health;
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

        public int armour
        {
            get
            {
                return _armour;
            }
            set
            {
                _armour = value;
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

        public int vitality
        {
            get
            {
                return _vitality;
            }
            set
            {
                _vitality = value;
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
        public bool isDead { get { return _health <= 0 ? true : false; } }

        public int id { get { return _id; } set { _id = value; } }
        #endregion

        #region Public Methods


        public void LevelUp(Actor actor)
        {
            actor._vitality++;
            actor._strength++;
            actor._exp = 0;
            actor._level++;
            actor._health = actor._maxhealth;
        }

        public void SetDamage(Actor actor)
        {
            SetDamage(actor.damage);
        }

        /// <summary>
        /// Schaden nach einem Angriff für diesen Actor setzen.
        /// Erst Rüstung, dann Lebenspunkte;
        /// </summary>
        /// <param name="damage"></param>
        public void SetDamage(int damage)
        {
            int tmp = _armour - damage;
            if (tmp < 0)
                _health = (_health + tmp > 0) ? (_health + tmp) : 0;
        }

        public void AddProtection(int amount)
        {
            if (amount > _armour)
            {
                _armour = amount;
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip  " : (_name + " equips ")) + " new armor.");
            }
            else
            {
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the armor away.");
            }
        }

        public void AddHealth(int amount)
        {
            int temp = Math.Max(amount, _maxhealth - _health - amount);
            if (temp > 0)
            {
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You regain " : (_name + " regains ")) + temp.ToString() + " hitpoints.");
            }
            else
            {
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " puts the potion away.");
            }
            _health = Math.Min(_health + amount, _maxhealth);
        }

        public void AddStrength(int amount)
        {
            if (amount > _damage)
            {
                _damage = amount;
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip " : (_name + " equips ")) + " a new weapon.");
            }
            else
            {
                _tile.HandleEvent(null, Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the weapon away.");

            }
        }

        public void SaveActor(XmlWriter writer)
        {
            writer.WriteStartElement("actor");
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("maxhp", Convert.ToString(_maxhealth));
            writer.WriteAttributeString("currhp", Convert.ToString(_health));
            writer.WriteAttributeString("arm", Convert.ToString(_armour));
            writer.WriteAttributeString("lev", Convert.ToString(_level));
            writer.WriteAttributeString("str", Convert.ToString(_strength));
            writer.WriteAttributeString("vit", Convert.ToString(_vitality));
            writer.WriteAttributeString("#items", Convert.ToString(_inventory.Count()));
            writer.WriteStartElement("Inventory");
            foreach (Item itm in _inventory)
            {
                //save inventory
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public void LoadActor(XmlReader reader)
        {
            _name = reader.GetAttribute("name");
            _maxhealth = Convert.ToInt32(reader.GetAttribute("maxhp"));
            _health = Convert.ToInt32(reader.GetAttribute("currhp"));
            _armour = Convert.ToInt32(reader.GetAttribute("arm"));
            _level = Convert.ToInt32(reader.GetAttribute("lev"));
            _strength = Convert.ToInt32(reader.GetAttribute("str"));
            _vitality = Convert.ToInt32(reader.GetAttribute("vit"));
            for (int i = 0; i < Convert.ToInt32(reader.GetAttribute("#items")); i++)
            {
                //read inventory
            }
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
                        using (TextReader reader = new StreamReader("playernames.txt"))
                        {
                            index = r.Next(File.ReadAllLines("playernames.txt").Length);
                            int i = 0;
                            while (i < index && reader.ReadLine() != null)
                            {
                                i++;
                            }
                            _name = reader.ReadLine();
                        }
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
                        using (TextReader reader = new StreamReader("enemynames.txt"))
                        {
                            index = r.Next(File.ReadAllLines("enemynames.txt").Length);
                            int i = 0;
                            while (i < index && reader.ReadLine() != null)
                            {
                                i++;
                            }
                            _name = reader.ReadLine();
                        }
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
                        using (TextReader reader = new StreamReader("npcnames.txt"))
                        {
                            index = r.Next(File.ReadAllLines("npcnames.txt").Length);
                            int i = 0;
                            while (i < index && reader.ReadLine() != null)
                            {
                                i++;
                            }
                            _name = reader.ReadLine();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actorType"></param>
        /// <param name="health"></param>
        /// <param name="armour"></param>
        /// <param name="damage"></param>
        public Actor(ActorType actorType, int health, int armour, int damage, int maxHealth = -1, string name = "", Random r = null)
        {
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
            if (_maxhealth == -1)
            {
                maxHealth = health;
            }
            if (armour < 0)
            {
                armour = r.Next(10);
            }
            this._armour = armour;
            if (damage < 0)
            {
                damage = 12 + r.Next(10);
            }
            this._damage = damage;
            if (name.Trim() == "") GenerateName(r);
            else _name = name;
            this._inventory = new List<Item>();
        }

        #endregion

    }
}
