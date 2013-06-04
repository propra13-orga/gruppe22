using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{
    public class NPC : Actor
    {
        private bool _hasShop = true;
        private int _love = 0;

        public bool hasShop
        {
            get
            {
                return _hasShop;
            }
            set
            {
                _hasShop = value;
            }
        }

        public int love
        {
            get
            {
                return _love;
            }
            set
            {
                _love = value;
            }
        }

        public void Interact()
        {
            if (_hasShop) ((IHandleEvent)_tile.parent).HandleEvent(false, Events.Shop, this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NPC(ContentManager content, int health = 10, int armor = 0, int damage = 0, int maxHealth = 10, string name = "", Random r = null)
            : base(content, ActorType.NPC, health, armor, damage, maxHealth, name, r)
        {
            _actorType = ActorType.NPC;
            _animationFile = "luigi";
        }
    }
}
