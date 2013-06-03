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
        public Enemy(ContentManager content, int health = -1, int armour = -1, int damage = -1, int maxHealth = -1, string name = "", Random r = null)
            : base(content, ActorType.Enemy, health, armour, damage, maxHealth, name, r)
        {
        }

    }
}
