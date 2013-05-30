using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class NPC : Actor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NPC(ContentManager content,int health=10, int armor=0, int damage=0, int maxHealth = 10, string name = "")
            : base(content,ActorType.NPC, health, armor, damage, maxHealth, name)
        {
        }
    }
}
