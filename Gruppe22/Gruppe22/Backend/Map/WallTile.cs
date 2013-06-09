using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class WallTile : Tile
    {
        private bool _illusion = false;
        private bool _illusionVisible = false;
        private int _health = -1;
        private int _decoration = 0;
        private bool _enabled = true;

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

        public int decoration
        {
            get
            {
                return _decoration;
            }
            set
            {
                _decoration = value;
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

        public bool destructible
        {
            get
            {
                return _health > 0;
            }
        }


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

        public WallTile(object parent)
            : base(parent)
        {
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            xmlw.WriteAttributeString("Illusion", _illusion.ToString());
            xmlw.WriteAttributeString("Illusionvisible", _illusionVisible.ToString());
            xmlw.WriteAttributeString("Health", _health.ToString());
            xmlw.WriteAttributeString("Decoration", _decoration.ToString());
            xmlw.WriteAttributeString("Enabled", _enabled.ToString());
            xmlw.WriteEndElement();
        }
    }
}
