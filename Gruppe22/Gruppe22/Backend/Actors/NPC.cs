using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class NPC : Actor
    {
        /// <summary>
        /// Constuctor. 
        protected ActorType _actorType = ActorType.NPC;

        /// </summary>
        public NPC(int health, int armour, int damage)
            : base(ActorType.Enemy, health, armour, damage)
        {
        }
    }
}
