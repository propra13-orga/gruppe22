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
        public NPC(int health, int armour, int damage, int maxHealth = -1, string name = "")
            : base(ActorType.NPC, health, armour, damage, maxHealth, name)
        {
        }
    }
}
