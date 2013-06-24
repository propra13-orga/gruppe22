using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    public class ReservedTile : Tile
    {
        private bool _enabled = true;
        private string _filename = "";
        private int _index;
        private bool _canEnter = false;
        private int _envIndex = -1;

        public int envIndex
        {
            get
            {
                return _envIndex;
            }
            set
            {
                _envIndex = value;
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

        public bool canEnter
        {
            get
            {
                return _canEnter;
            }
            set
            {
                _canEnter = value;
            }
        }

        public string filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }

        public int index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        public ReservedTile(object parent, string filename = "", int index = 0, bool canenter = false, bool enabled = true)
            : base(parent)
        {
            _filename = filename;
            _index = index;
            _canEnter = canenter;
            _enabled = enabled;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ReservedTile");
            xmlw.WriteAttributeString("Enabled", _enabled.ToString());
            xmlw.WriteAttributeString("CanEnter", _canEnter.ToString());
            xmlw.WriteAttributeString("Filename", _filename.ToString());
            xmlw.WriteAttributeString("Index", _index.ToString());
            xmlw.WriteEndElement();
        }
    }
}
