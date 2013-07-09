using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The class used to generate NPCs.
    /// NPCs can either tell a story to the player or have a shop.
    /// </summary>
    public class NPC : Actor
    {
        private bool _hasShop = true;
        private bool _hasDialog = false;
        /// <summary>
        /// Verhältnis des NPC's zum Spieler. Bei 0 ist es neutral, negative Werte stellen ein schlechtes Verhältnis dar und positive ein gutes. Dabei haben vom Betrag größere Zahlen eine größere Auswirkung.
        /// </summary>
        private int _love = 0;

        /// <summary>
        /// Determines if the NPC has a shop.
        /// </summary>
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

        /// <summary>
        /// Determines if the NPC has something to say to the player.
        /// </summary>
        public bool hasDialog
        {
            get
            {
                return _hasDialog;
            }
            set
            {
                _hasDialog = value;
            }
        }

        /// <summary>
        /// Field to discribe the relationship
        /// </summary>
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
        /// Method to interact with a NPC.
        /// Either opens the NPC's shop or his dialogue.
        /// Calls the Eventhandler.
        /// </summary>
        /// <param name="With">The actor with which should be interacted</param>
        public void Interact(Actor With)
        {
            if (_hasShop) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Shop, this, With);
            if (_hasDialog) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Dialog, this, With);
        }

        /// <summary>
        /// The constructor for a NPC.
        /// Initializes the standard properties and graphics for that NPC.
        /// See actor for the params.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="health"></param>
        /// <param name="armor"></param>
        /// <param name="damage"></param>
        /// <param name="maxHealth"></param>
        /// <param name="name"></param>
        /// <param name="r"></param>
        /// <param name="_level"></param>
        /// <param name="shop"></param>
        public NPC(int health = 10, int armor = 0, int damage = 0, int maxHealth = 10, string name = "", Random r = null, int _level = 1, bool shop = false)
            : base(ActorType.NPC, health, armor, damage, maxHealth, name, r)
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
