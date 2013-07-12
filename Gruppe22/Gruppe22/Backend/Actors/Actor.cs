using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The possible types of an actor
    /// </summary>
    public enum ActorType
    {
        Player = 0,
        NPC = 1,
        Enemy = 2
    }

    /// <summary>
    /// A living entity (backend) for example the player
    /// </summary>
    public class Actor
    {
        #region protected Fields
        /// <summary>
        /// A random.
        /// </summary>
        protected Random _random;

        /// <summary>
        /// A unique identity used in online games
        /// </summary>
        protected string _GUID = "";
        /// <summary>
        /// Determines whether player is available 
        /// </summary>
        protected bool _online = false;
        /// <summary>
        /// Number of lives (ressurection)
        /// </summary>
        protected int _lives = -1;
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
        protected bool _aggro = true;
        protected bool _ranged = false;
        protected bool _crazy = false;
        protected bool _friendly = false;
        protected int _resist = 0;
        protected int _exp = 0;
        int regCounter = 0;
        protected int _expNeeded = 0;
        protected int _maxhealth = 100;
        protected int _health = 50;
        public List<int> _quicklist = null;
        protected int _armor = 40;
        protected int _abilityPoints = 0;
        protected Direction _direction = Direction.Right;
        protected int _skills = 0;
        protected int _viewRange = 4;
        protected string _animationFile = ".\\content\\player.xml";
        protected int _stunned = 0;
        protected int _charmed = 0;
        protected int _scared = 0;
        protected List<Ability> _abilities = null;
        private bool _regenerating = false;
        protected int _lastCheckpoint = 1;
        #endregion

        #region Public Fields
        /*        public int deadcounter
        {
            get { return _deadcounter; }
            set { _deadcounter = value; }
        }*/

        /// <summary>
        /// Method to get the items an actor has.
        /// </summary>
        /// <param name="i">The id of the item</param>
        /// <returns>Return the position in the inventory-list of the item with the asked id.
        /// Returns null if there is no item with this id.</returns>
        public Item Items(int i)
        {
            for (int count = 0; count < inventory.Count; ++count)
            {
                if (_inventory[count].id == i) return _inventory[count];
            }
            return null;

        }

        /// <summary>
        /// Unique identifier for players in LAN-mode.
        /// </summary>
        public string GUID
        {
            get
            {
                return _GUID;
            }
            set
            {
                _GUID = value;
            }
        }

        /// <summary>
        /// Determines whether a player is online.
        /// </summary>
        public bool online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
            }
        }

        /// <summary>
        /// Number of current lives (read/write).
        /// </summary>
        public int lives
        {
            get
            {
                return _lives;
            }
            set
            {
                _lives = value;
            }
        }


        /// <summary>
        /// Number of current lives (read/write).
        /// </summary>
        public int lastCheckpoint
        {
            get
            {
                return _lastCheckpoint;
            }
            set
            {
                _lastCheckpoint = value;
            }
        }

        /// <summary>
        /// List of items which have changed since last viewing inventory.
        /// </summary>
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

        /// <summary>
        /// Direction actor is currently facing (read/write).
        /// </summary>
        public Direction direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        /// <summary>
        /// Method to let the player regenerate mana and health.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// Determines if the actor is scared of something.
        /// Scared actors run away from their attacker.
        /// </summary>
        public int scared
        {
            get
            {
                return _scared;
            }
            set
            {
                _scared = value;
            }
        }

        /// <summary>
        /// Determines if the actor is stunned.
        /// Stunned actors can not do anything.
        /// </summary>
        public int stunned
        {
            get
            {
                return _stunned;
            }
            set
            {
                _stunned = value;
            }
        }

        /// <summary>
        /// Field to determine if an actor is charmed (temporary friendly).
        /// Setting this just sets the friendly attribute.
        /// </summary>
        public int charmed
        {
            get { return _charmed; }
            set
            {
                _charmed = value;
                if (_charmed != 0) _friendly = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// True if the actor if friendly with the player.
        /// Friendly actors do not attack the player.
        /// </summary>
        public bool friendly
        {
            get
            {
                return _friendly;
            }
            set
            {
                _friendly = value;
            }
        }

        /// <summary>
        /// Field to set if an actor attacks the player by default.
        /// </summary>
        public bool aggro
        {
            get
            {
                return _aggro;
            }
            set
            {
                _aggro = value;
            }
        }

        /// <summary>
        /// Field to determine if an actor is crazy.
        /// A crazy actor attacks every other actor.
        /// </summary>
        public bool crazy
        {
            get
            {
                return _crazy;
            }
            set
            {
                _crazy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ranged
        {
            get
            {
                return _ranged;
            }
            set
            {
                _ranged = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
            }
        }

        /// <summary>
        /// The path to the graphics file for the actor.
        /// </summary>
        public string animationFile
        {
            get
            {
                return _animationFile;
            }
            set
            {
                _animationFile = value;
            }
        }

        /// <summary>
        /// A reference to the tile on the map which represents the actor.
        /// </summary>
        public ActorTile tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        /// <summary>
        /// A list of items the actor has.
        /// </summary>
        public List<Item> inventory
        {
            get
            {
                return _inventory;
            }
        }

        /// <summary>
        /// A list of abilities or skills the actor has.
        /// </summary>
        public List<Ability> abilities
        {
            get
            {
                return _abilities;
            }
        }

        /// <summary>
        /// The current amount of mana the actor has.
        /// can not be less than zero
        /// </summary>
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

        /// <summary>
        /// The amount of experience needed to reach the next level.
        /// </summary>
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

        /// <summary>
        /// The maximum amount of mana the actor can have.
        /// </summary>
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

        /// <summary>
        /// How much mana the actor regenerates.
        /// </summary>
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

        /// <summary>
        /// The amount of gold the actors has.
        /// Gold is the currency used to buy items.
        /// </summary>
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

        /// <summary>
        /// The type the actor is.
        /// e.g. an enemy
        /// </summary>
        public ActorType actorType
        {
            get
            {
                return _actorType;
            }
        }

        /// <summary>
        /// The current health the player has.
        /// Can not be less than zero.
        /// </summary>
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

        /// <summary>
        /// The maximum amount of health the actor has.
        /// </summary>
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

        /// <summary>
        /// The armor an actor has.
        /// Should be values between 0 and 100.
        /// </summary>
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

        /// <summary>
        /// The damage an actor deals with an attack.
        /// </summary>
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

        /// <summary>
        /// Simply the name of the actor.
        /// </summary>
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

        /// <summary>
        /// The current level of the actor.
        /// By default starting at 0.
        /// Can only be changed by a levelup
        /// </summary>
        public int level
        {
            get
            {
                return _level;
            }
        }

        /// <summary>
        /// The current experience points the actor has.
        /// </summary>
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

        /// <summary>
        /// Determines if the actor is alive or dead
        /// by checking his current health
        /// </summary>
        public bool isDead
        {
            get
            {
                return _health <= 0 ? true : false;
            }
        }

        /// <summary>
        /// The actors ID.
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

        /// <summary>
        /// The evade value of the actor.
        /// Evade is used to calculate the chance to evade an attack.
        /// Evading an attack negates all the damage.
        /// Values should range from 0 to 100.
        /// </summary>
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

        /// <summary>
        /// The block value.
        /// Block is used to determine the chance to block an attack.
        /// Blocking negates some damage.
        /// Values should range from 0 to 100.
        /// </summary>
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

        /// <summary>
        /// Penetrate is the counterpart to block.
        /// Penetrate lets an actor get through the block of an other actor.
        /// Values should range from 0 to 100.
        /// </summary>
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

        /// <summary>
        /// The amount of health an actor regenerates in one clock.
        /// </summary>
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

        /// <summary>
        /// The amount of health an actor can steal from an other actor with an attack.
        /// </summary>
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

        /// <summary>
        ///The amount of mana an actor can steal from an other actor with an attack.
        /// </summary>
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

        /// <summary>
        /// The amount of fire-type damage an actor deals with an attack.
        /// </summary>
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

        /// <summary>
        /// The amount of ice-type damage an actor deals with an attack.
        /// </summary>
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

        /// <summary>
        /// The "armor" against fire damage the actor has.
        /// </summary>
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

        /// <summary>
        /// The "armor" against fire damage the actor has.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// The max amount of health the actor has.
        /// </summary>
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

        /// <summary>
        /// The points an actor has left to spend on abilities or skills.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// Method called when the exp reach the needed exp.
        /// Lets the player level up.
        /// The actor gains abilitypoints, skills and his health is set to maxhealth.
        /// Also the needed exp increase for the next level.
        /// </summary>
        public void LevelUp()
        {
            _level++;
            _abilityPoints += 10;
            _skills += 1;
            _expNeeded = 3 * (_level + 1) ^ 2 + 83 * (_level + 1) + 41;
            _health = _maxhealth;
        }

        /// <summary>
        /// Method to increase the amount of armor an actor has.
        /// Does not change anything if armor>amount
        /// </summary>
        /// <param name="amount">the new amount the armor the actor should have</param>
        public void AddProtection(int amount)
        {
            if (amount > _armor)
            {
                _armor = amount;
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip  " : (_name + " equips ")) + " new armor.");
            }
            else
            {
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the armor away.");
            }
        }

        /// <summary>
        /// Method to let the player get some health back
        /// </summary>
        /// <param name="amount">The amount of healthpoints that should be added</param>
        public void AddHealth(int amount)
        {
            int temp = Math.Max(amount, _maxhealth - _health - amount);
            if (temp > 0)
            {
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You regain " : (_name + " regains ")) + temp.ToString() + " hitpoints.");
            }
            else
            {
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " puts the potion away.");
            }
            _health = Math.Min(_health + amount, _maxhealth);
        }

        /// <summary>
        /// Method to directly increase the damage an actor deals.
        /// Does nothing if damage>amount.
        /// </summary>
        /// <param name="amount">The new amount of damage the actor should deal</param>
        public void AddStrength(int amount)
        {
            if (amount > _damage)
            {
                _damage = amount;
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You equip " : (_name + " equips ")) + " a new weapon.");
            }
            else
            {
                _tile.HandleEvent(false, Backend.Events.ShowMessage, ((_actorType == ActorType.Player) ? "You put " : (_name + " puts ")) + " the weapon away.");

            }
        }

        /// <summary>
        /// Method to save an actor in an XML-file.
        /// Just writes every property to the file.
        /// </summary>
        /// <param name="writer">The writer which should be used to save the data</param>
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
            writer.WriteAttributeString("crazy", Convert.ToString(_crazy));
            writer.WriteAttributeString("ranged", Convert.ToString(_ranged));
            writer.WriteAttributeString("aggro", Convert.ToString(_aggro));
            writer.WriteAttributeString("friendly", Convert.ToString(_friendly));

            writer.WriteAttributeString("fireDefense", Convert.ToString(_fireDefense));
            writer.WriteAttributeString("iceDefense", Convert.ToString(_iceDefense));
            writer.WriteAttributeString("expNeeded", Convert.ToString(_expNeeded));
            writer.WriteAttributeString("exp", Convert.ToString(_exp));
            writer.WriteAttributeString("resist", Convert.ToString(_resist));
            writer.WriteAttributeString("damage", Convert.ToString(_damage));
            writer.WriteAttributeString("gold", Convert.ToString(_gold));
            writer.WriteAttributeString("lastCheckpoint", Convert.ToString(_lastCheckpoint));
            writer.WriteAttributeString("manaReg", Convert.ToString(_manaReg));
            writer.WriteAttributeString("maxMana", Convert.ToString(_maxMana));
            writer.WriteAttributeString("destroyWeapon", Convert.ToString(_destroyWeapon));
            writer.WriteAttributeString("destroyArmor", Convert.ToString(_destroyArmor));
            writer.WriteAttributeString("animation", Convert.ToString(_animationFile));
            writer.WriteAttributeString("viewRange", Convert.ToString(_viewRange));
            if (_GUID != "") writer.WriteAttributeString("GUID", Convert.ToString(GUID));
            if (_stunned != 0) writer.WriteAttributeString("stunned", Convert.ToString(_stunned));
            if (_charmed != 0) writer.WriteAttributeString("charmed", Convert.ToString(_charmed));
            if (_lives != -1) writer.WriteAttributeString("lives", Convert.ToString(_lives));
            if (_direction != Direction.None) writer.WriteAttributeString("direction", Convert.ToString(direction));

            if (_charmed != 0) _friendly = true;
            writer.WriteAttributeString("scared", Convert.ToString(_scared));

            if (actorType == ActorType.NPC)
            {
                NPC n = this as NPC;
                if (n != null)
                {
                    writer.WriteAttributeString("love", Convert.ToString(n.love));
                    writer.WriteAttributeString("hasShop", Convert.ToString(n.hasShop));
                    writer.WriteAttributeString("hasDialogue", Convert.ToString(n.hasDialog));
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

        /// <summary>
        /// Methode to determine if the player has the key to the next level.
        /// </summary>
        /// <param name="level">The level for which the key is checked.</param>
        /// <returns>True if the player got the key.</returns>
        public bool HasKey(int level)
        {
            for (int i = 0; i < _inventory.Count; ++i)
            {
                if ((_inventory[i].itemType == Backend.ItemType.Key) && (_inventory[i].level == 10 + level)) return true;
            }
            return false;
        }

        /// <summary>
        /// Duplicates the properties of actor a to the current actor.
        /// </summary>
        /// <param name="a">The actor from which the properties should be cloned</param>
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
            _lives = a.lives;
            _direction = a.direction;

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

        /// <summary>
        /// Method to read actor properties from a XML-file.
        /// </summary>
        /// <param name="reader">The used XMLReader for reading the actor.</param>
        public void Load(XmlReader reader)
        {
            //            System.Diagnostics.Debug.WriteLine(reader.Name);

            _newItems = 0;
            _inventory.Clear();
            _name = reader.GetAttribute("name");
            _maxhealth = Convert.ToInt32(reader.GetAttribute("maxhp"));
            if (reader.GetAttribute("file") != null) _animationFile = Convert.ToString("file");
            if (reader.GetAttribute("viewRange") != null) _viewRange = Convert.ToInt32(reader.GetAttribute("viewRange"));
            _health = Convert.ToInt32(reader.GetAttribute("hp"));
            _level = Convert.ToInt32(reader.GetAttribute("level"));
            _mana = Convert.ToInt32(reader.GetAttribute("mana"));
            _evade = Convert.ToInt32(reader.GetAttribute("evade"));
            if (reader.GetAttribute("GUID") != null) _GUID = reader.GetAttribute("GUID");
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
            _gold = Convert.ToInt32(reader.GetAttribute("gold"));
            _manaReg = Convert.ToInt32(reader.GetAttribute("manaReg"));
            _maxMana = Convert.ToInt32(reader.GetAttribute("maxMana"));
            _destroyWeapon = Convert.ToInt32(reader.GetAttribute("destroyWeapon"));
            _destroyArmor = Convert.ToInt32(reader.GetAttribute("destroyArmor"));
            if (reader.GetAttribute("lastCheckPoint") != null)
            {
                _lastCheckpoint = Convert.ToInt32(reader.GetAttribute("lastCheckPoint"));
            }
            if (reader.GetAttribute("lives") != null) _lives = Convert.ToInt32(reader.GetAttribute("lives"));
            if (reader.GetAttribute("direction") != null)
                _direction = (Direction)Enum.Parse(typeof(Direction), reader.GetAttribute("direction"));
            if (_direction == Direction.None) _direction = Direction.Up;

            if (reader.GetAttribute("crazy") != null) _crazy = Convert.ToBoolean(reader.GetAttribute("crazy"));
            if (reader.GetAttribute("ranged") != null) _ranged = Convert.ToBoolean(reader.GetAttribute("ranged"));
            if (reader.GetAttribute("aggro") != null) _aggro = Convert.ToBoolean(reader.GetAttribute("aggro"));
            if (reader.GetAttribute("friendly") != null) _friendly = Convert.ToBoolean(reader.GetAttribute("friendly"));


            _animationFile = reader.GetAttribute("animation");
            if (reader.GetAttribute("stunned") != null)
                _stunned = Convert.ToInt32(reader.GetAttribute("stunned"));
            if (reader.GetAttribute("charmed") != null)
                _charmed = Convert.ToInt32(reader.GetAttribute("charmed"));
            if (reader.GetAttribute("scared") != null)
                _scared = Convert.ToInt32(reader.GetAttribute("scared"));

            if (actorType == ActorType.NPC)
            {
                NPC n = this as NPC;
                if (n != null)
                {
                    n.love = Convert.ToInt32(reader.GetAttribute("love"));
                    n.hasShop = Convert.ToBoolean(reader.GetAttribute("hasShop"));
                    n.hasDialog = Convert.ToBoolean(reader.GetAttribute("hasDialogue"));

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
                    Item item = new Item();
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
                        Ability ability = new Ability();
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

        /// <summary>
        /// Add new item to inventory
        /// </summary>
        /// <param name="item">The item to add to inventory</param>
        public void AddItem(Item item)
        {
            _newItems += 1;
            int max = 0;
            for (int i = 0; i < _inventory.Count; ++i)
            {
                if (_inventory[i].id > max)
                {
                    max = _inventory[i].id;
                }
            }
            item.id = max + 1;
            item.owner = this;
            item.tile = null;
            item.isNew = true;
            _inventory.Add(item);
        }
        /// <summary>
        /// Changed ToString Method for the actor's name.
        /// </summary>
        /// <returns>Returns the name of the actor in brackets.</returns>
        public override string ToString()
        {
            return base.ToString() + " (" + _name + ")";
        }

        /// <summary>
        /// Method to generate a random actor name.
        /// Uses GenerateName() for files if a file exists.
        /// Otherwise chooses one of six random names.
        /// </summary>
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
        /// <summary>
        /// Method to choose an random actor name from a file.
        /// </summary>
        /// <param name="filename">The file with one name per line</param>
        /// <returns>The chosen name.</returns>
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
        /// The constructor used for actors.
        /// Sets default values for the most important properties.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="actorType">Player, NPC oder Enemy</param>
        /// <param name="health">Standardwert 15+random(30) or 5+random(maxhealth-5) if maxhealth is passed</param>
        /// <param name="armor">default random(10)</param>
        /// <param name="damage">default 12+random(10)</param>
        /// <param name="maxHealth">default = health</param>
        /// <param name="name">uses GenerateName() by default</param>
        /// <param name="rnd">a random used to generate the actors starting values</param>
        /// <param name="animationFile">the file used to display the actor</param>
        /// <param name="level">the default starting level is 1</param>
        public Actor(ActorType actorType, int health, int armor, int damage, int maxHealth = -1, string name = "", Random rnd = null, string animationFile = "", int level = -1)
        {

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
