﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class Player : Actor
    {

        /// <summary>
        /// Constructor 
        /// </summary>
        public Player(ContentManager content, int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(content, ActorType.Player, health, armour, damage, maxHealth, name)
        {
        }
    }
}
