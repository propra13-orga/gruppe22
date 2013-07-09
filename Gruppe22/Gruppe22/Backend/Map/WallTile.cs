using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile representing a wall in the game.
    /// A wall blocks actor movements.
    /// </summary>
    public class WallTile : Tile
    {
        private bool _illusion = false;
        private bool _illusionVisible = false;
        private int _health = -1;
        private bool _enabled = true;
        public WallType _type = Backend.WallType.Normal;

        /// <summary>
        /// The amount of health the wall has.
        /// Some walls have health and get destroyed by an actor.
        /// </summary>
        public int health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        
        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <summary>
        /// Bool to determine if the wall can get destroyed.
        /// Is true if the wall has more than 0 health.
        /// </summary>
        public bool destructible
        {
            get
            {
                return _health > 0;
            }
        }

        /// <summary>
        /// A bool to determine if the trap is an illusion.
        /// Illusionary walls can be passed by actors.
        /// </summary>
        public bool illusion
        {
            get
            {
                return _illusion;
            }
            set
            {
                _illusion = value;
            }
        }

        /// <summary>
        /// A bool for illusionary walls which tells if the illusion is activ.
        /// </summary>
        public bool illusionVisible
        {
            get
            {
                return _illusionVisible;
            }
            set
            {
                _illusionVisible = value;
            }
        }

        /// <summary>
        /// A constructor for the WallTile.
        /// Just calls the base constructor.
        /// </summary>
        /// <param name="parent"></param>
        public WallTile(object parent)
            : base(parent)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual WallType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Another constructor for the WallTile.
        /// Chooses random decorations for the wall.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="r">A random used to choose the deco.</param>
        public WallTile(object parent, Random r)
            : base(parent)
        {
            if (r.Next(100) > 80)
            {
                _type = Backend.WallType.Deco1;
            }
            if (r.Next(100) > 80)
            {
                _type = Backend.WallType.Deco3;
            }
            if (r.Next(100) > 80)
            {
                _type = Backend.WallType.Deco2;
            }
        }

        /// <summary>
        /// The method to save a WallTile in a XML-file
        /// </summary>
        /// <param name="xmlw">A XMLwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            if (_illusion) xmlw.WriteAttributeString("Illusion", _illusion.ToString());
            if (_illusionVisible) xmlw.WriteAttributeString("Illusionvisible", _illusionVisible.ToString());
            if (_health > -1) xmlw.WriteAttributeString("Health", _health.ToString());
            if (_type != Backend.WallType.Normal) xmlw.WriteAttributeString("Type", _type.ToString());
            if (!_enabled) xmlw.WriteAttributeString("Enabled", _enabled.ToString());
            xmlw.WriteEndElement();
        }
    }
}
