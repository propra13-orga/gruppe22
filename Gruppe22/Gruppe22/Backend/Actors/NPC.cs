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
        public NPC(int health=10, int armor=0, int damage=0, int maxHealth = 10, string name = "")
            : base(ActorType.NPC, health, armor, damage, maxHealth, name)
        {
        }
    }
}
