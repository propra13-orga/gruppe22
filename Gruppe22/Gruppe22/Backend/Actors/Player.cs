﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gruppe22
{
    public class Player:Actor
    {

        /// <summary>
        /// Constructor 
        /// </summary>
        public Player(string name, int health, int armour, int damage)
            : base(ActorType.Enemy, health, armour, damage)
        {
            _actorType = ActorType.Player;
        }
    }
}
