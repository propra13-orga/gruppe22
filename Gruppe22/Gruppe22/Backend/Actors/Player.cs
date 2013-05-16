using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class Player : Actor
    {

        /// <summary>
        /// Constructor 
        /// </summary>
        public Player(int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(ActorType.Player, health, armour, damage, maxHealth, name)
        {
        }
    }
}
