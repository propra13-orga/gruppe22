using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The class used for players, a type of an actor.
    /// </summary>
    public class Player : Actor
    {
        /// <summary>
        /// The constructor for a player.
        /// Sets the default values.
        /// See actor for the params.
        /// </summary>
        /// <param name="health"></param>
        /// <param name="armour"></param>
        /// <param name="damage"></param>
        /// <param name="maxHealth"></param>
        /// <param name="name"></param>
        public Player( int health = 100, int armour = 30, int damage = 20, int maxHealth = -1, string name = "")
            : base(ActorType.Player, health, armour, damage, maxHealth, name)
        {
            _actorType = ActorType.Player;
            _viewRange = 4;
            _animationFile = ".\\content\\player.xml";
        }
    }
}
