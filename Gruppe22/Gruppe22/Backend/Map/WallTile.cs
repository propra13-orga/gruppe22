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
        private bool _enabled = true;
        public WallType _type = WallType.Normal;

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

        public WallTile(object parent, Random r)
            : base(parent)
        {
            if (r.Next(100) > 80)
            {
                _type = WallType.Deco1;
            }
            if (r.Next(100) > 80)
            {
                _type = WallType.Deco3;
            }
            if (r.Next(100) > 80)
            {
                _type = WallType.Deco2;
            }
        }
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            if (_illusion) xmlw.WriteAttributeString("Illusion", _illusion.ToString());
            if (_illusionVisible) xmlw.WriteAttributeString("Illusionvisible", _illusionVisible.ToString());
            if (_health > -1) xmlw.WriteAttributeString("Health", _health.ToString());
            if (_type != WallType.Normal) xmlw.WriteAttributeString("Type", _type.ToString());
            if (!_enabled) xmlw.WriteAttributeString("Enabled", _enabled.ToString());
            xmlw.WriteEndElement();
        }
    }
}
