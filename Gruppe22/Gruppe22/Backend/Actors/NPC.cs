using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
//NPC Entwicklerdoku fertig!
namespace Gruppe22.Backend
{
    public class NPC : Actor
    {
        /// <summary>
        /// Markiert ob der NPC ein Shop ist.
        /// </summary>
        private bool _hasShop = true;
        /// <summary>
        /// Ist true falls ein NPC ein Dialog (sprechen) besitzt.
        /// </summary>
        private bool _hasDialogue = false;
        /// <summary>
        /// Verhältnis des NPC's zum Spieler. Bei 0 ist es neutral, negative Werte stellen ein schlechtes Verhältnis dar und positive ein gutes. Dabei haben vom Betrag größere Zahlen eine größere Auswirkung.
        /// </summary>
        private int _love = 0;

        /// <summary>
        /// Öffentliche Eigenschaft zu _hasShop.
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
        /// Öffentliche Eigenschaft zu _hasDialogue.
        /// </summary>
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

        /// <summary>
        /// Öffentlcie Eigenschaft zu _love.
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
        /// Interaktion des NPC. Aktion öffne Schop bzw. Dialog.
        /// Funktionsweise uber Eventaufruf.
        /// </summary>
        public void Interact()
        {
            if (_hasShop) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Shop, this);
            if (_hasDialogue) ((Backend.IHandleEvent)_tile.parent).HandleEvent(false, Backend.Events.Dialogue, this);
        }

        /// <summary>
        /// Konstruktor. Initiallisierung der Grafikverweise.
        /// </summary>
        /// <param name="content">Resoursenverwaltung.</param>
        /// <param name="health">Lebenspunkte, Standardwert ist 10.</param>
        /// <param name="armor">Rüstung, Standardwert ist 0.</param>
        /// <param name="damage">Schaden, Standardwert ist 0.</param>
        /// <param name="maxHealth">Maximale Lebenspunkte, Standardwert ist 10.</param>
        /// <param name="name">Name des NPCs.</param>
        /// <param name="r">Randomobjekt für die Basisklasse.</param>
        /// <param name="_level">NPC aus dem Level _level.</param>
        /// <param name="shop">Legt fest ob der NPC ein Shop ist.</param>
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
