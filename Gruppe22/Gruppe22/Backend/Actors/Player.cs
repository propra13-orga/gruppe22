using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    public class Player : Actor
    {

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public Player(ContentManager content, int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(content, ActorType.Player, health, armour, damage, maxHealth, name)
        {
            _actorType = ActorType.Player;
            _viewRange = 4;
            _animationFile = ".\\content\\player.xml";
        }
    }
}
