using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class Enemy : Actor
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public Enemy(int health = -1, int armour = -1, int damage = -1, int maxHealth = -1, string name = "", Random r = null)
            : base(ActorType.Enemy, health, armour, damage, maxHealth, name, r)
        {
        }

    }
}
