using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    public class NPC : Actor
    {
        private bool _hasShop = true;
        private bool _hasDialogue = false;
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

        public bool hasDialogue
        {
            get
            {
                return _hasDialogue;
            }
            set
            {
                _hasDialogue = value;
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

        /// <summary>
        /// 
        /// </summary>
        public void Interact()
        {
            if (_hasShop) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Shop, this);
            if (_hasDialogue) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Dialogue, this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NPC(ContentManager content, int health = 10, int armor = 0, int damage = 0, int maxHealth = 10, string name = "", Random r = null, int _level = 1, bool shop = false)
            : base(content, ActorType.NPC, health, armor, damage, maxHealth, name, r)
        {
            _actorType = ActorType.NPC;
            if (shop) _animationFile = ".\\content\\luigi.xml";
            else
                switch (_level)
                {
                    case 1:
                        switch (_random.Next(3))
                        {
                            case 0:
                                _animationFile = ".\\content\\mage.xml";
                                break;
                            case 1:
                                _animationFile = ".\\content\\guard.xml";
                                break;
                            case 2:
                                _animationFile = ".\\content\\npc1.xml";
                                break;
                        }
                        break;
                    case 2:
                        switch (_random.Next(3))
                        {
                            case 0:
                                _animationFile = ".\\content\\mage.xml";
                                break;
                            case 1:
                                _animationFile = ".\\content\\monk.xml";
                                break;
                            case 2:
                                _animationFile = ".\\content\\npc1.xml";
                                break;
                        }
                        break;
                    default:
                        switch (_random.Next(3))
                        {
                            case 0:
                                _animationFile = ".\\content\\princess.xml";
                                break;
                            case 1:
                                _animationFile = ".\\content\\xmas.xml";
                                break;
                            case 2:
                                _animationFile = ".\\content\\npc1.xml";
                                break;
                        }
                        break;
                }
        }
    }
}
