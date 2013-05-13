using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class Enemy : Actor
    {
        protected ActorType _actorType=ActorType.Enemy;

        /// <summary>
        /// Constuctor. 
        /// </summary>
        public Enemy(int health, int armour, int damage)
            : base(ActorType.Enemy, health, armour, damage)
        {
        }
    }
}
