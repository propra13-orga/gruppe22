using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class NPC : Actor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NPC(int health, int armour, int damage)
            : base(ActorType.Enemy, health, armour, damage)
        {
            _actorType = ActorType.NPC;
        }
    }
}
