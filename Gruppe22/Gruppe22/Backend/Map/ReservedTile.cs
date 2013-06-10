using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    public class ReservedTile : Tile
    {
        private Tile belongsTo;
        private bool _enabled = true;


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

        public ReservedTile(object parent)
            : base(parent)
        {
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ReservedTile");
            xmlw.WriteAttributeString("Enabled", _enabled.ToString());
            xmlw.WriteEndElement();
        }
    }
}
