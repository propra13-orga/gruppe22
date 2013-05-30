using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class Enemy : Actor
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public Enemy(ContentManager content,int health = -1, int armour = -1, int damage = -1, int maxHealth = -1, string name = "", Random r = null)
            : base(content,ActorType.Enemy, health, armour, damage, maxHealth, name, r)
        {
        }

    }
}
