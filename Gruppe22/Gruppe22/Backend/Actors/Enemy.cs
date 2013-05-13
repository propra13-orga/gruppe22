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
        public Enemy(int health, int armour, int damage)
            : base(ActorType.Enemy, health, armour, damage)
        {
            _actorType=ActorType.Enemy;
        }
    }
}
