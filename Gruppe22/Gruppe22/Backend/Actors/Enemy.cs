using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class Enemy : Actor
    {

        public void AssignSkillsAndAbilities()
        {
            while (_skills > 0)
            {
                int r = _random.Next(11);
                switch (r)
                {
                    case 0:
                        _damage += 1;
                        break;
                    case 1:
                        _maxhealth += 10;
                        break;
                    case 2:
                        _maxMana += 10;
                        break;
                    case 3:
                        _evade += 1;
                        break;
                    case 4:
                        _block += 1;
                        break;
                    case 5:
                        _penetrate += 1;
                        break;
                    case 6:
                        _resist += 1;
                        break;
                    case 7:
                        _fireDamage += 1;
                        break;
                    case 8:
                        _fireDefense += 1;
                        break;
                    case 9:
                        _iceDamage += 1;
                        break;
                    case 10:
                        _iceDefense += 1;
                        break;
                }
                _skills -= 1;

            }
            while (_abilityPoints > 0)
            {
                _abilityPoints -= 1;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Enemy(ContentManager content, int health = -1, int armour = -1, int damage = -1, int maxHealth = -1, string name = "", Random r = null, int level = 1)
            : base(content, ActorType.Enemy, health, armour, damage, maxHealth, name, r)
        {
            if (r == null) { r = new Random(); }
            _actorType = ActorType.Enemy;
            switch (level)
            {
                case 0:
                    _animationFile = ".\\content\\mouse.xml";
                    _damage = 2;
                    _penetrate = 0;
                    _scared = -1;
                    _exp = 10;
                    aggro = true;
                    crazy = false;
                    break;
                case 1:
                    switch (_random.Next(4))
                    {
                        case 0:
                            _animationFile = ".\\content\\bat.xml";
                            aggro = true;
                            crazy = true;
                            break;
                        case 1:
                            _animationFile = ".\\content\\gnome.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 2:
                            _animationFile = ".\\content\\wolf.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 3:
                            _animationFile = ".\\content\\spider.xml";
                            aggro = true;
                            crazy = true;
                            break;
                    }
                    break;
                case 2:
                    switch (r.Next(4))
                    {
                        case 0:
                            _animationFile = ".\\content\\skeleton.xml";
                            aggro = true;
                            crazy = true;
                            break;
                        case 1:
                            _animationFile = ".\\content\\ghost.xml";
                            aggro = false;
                            crazy = false;
                            break;
                        case 2:
                            _animationFile = ".\\content\\mummy.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 3:
                            _animationFile = ".\\content\\vamp.xml";
                            aggro = true;
                            crazy = false;
                            break;
                    }
                    break;
                case 3:
                    switch (r.Next(4))
                    {
                        case 0:
                            _animationFile = ".\\content\\croc.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 1:
                            _animationFile = ".\\content\\thief.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 2:
                            _animationFile = ".\\content\\cyclops.xml";
                            aggro = true;
                            crazy = false;
                            break;
                        case 3:
                            _animationFile = ".\\content\\devil.xml";
                            aggro = true;
                            crazy = false;
                            break;
                    }
                    break;
                case 11:
                    _animationFile = ".\\content\\rat.xml";
                    aggro = true;
                    crazy = false;
                    _level = 4;
                    break;
                case 12:
                    _animationFile = ".\\content\\necro.xml";
                    aggro = true;
                    crazy = false;
                    _level = 5;
                    break;
                case 13:
                    _animationFile = ".\\content\\dragon.xml";
                    aggro = true;
                    crazy = false;
                    _level = 6;
                    break;
            }
            if (level > 0)
            {
                Item item = new Item(_content, r, 0, _level, false);
                _inventory.Add(item);
                item.owner = this;
            }
            _gold = _random.Next(10 + _level / 10) * _level;
            if (level > 10)
            {
                Item item = new Item(_content, this, ItemType.Key, "Key to level " + (level % 10).ToString(), null, 0, level);
                _inventory.Add(item);
                item.owner = this;
            }
        }
    }
}
