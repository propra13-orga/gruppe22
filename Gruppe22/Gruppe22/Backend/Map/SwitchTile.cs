using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    class SwitchTile:Tile
    {
        /// <summary>
        /// Whether the switch is active
        /// </summary>
        private bool _active = false;
        /// <summary>
        /// Unique ID of switch (may be used as reference to door or trap)
        /// </summary>
        private int _id = 0;

        /// <summary>
        /// 1 based order in which switch status was changed; 0 for unchanged
        /// </summary>
        private int _order = 0;


        /// <summary>
        /// Whether the switch is active
        /// </summary>        
        public bool active { get { return _active; } set { _active = value; } }

        /// <summary>
        /// Unique ID of switch (may be used as reference to door or trap)
        /// </summary>

        public int id { get { return _id; } set { _id = value; } }

        
        /// <summary>
        /// 0 based order in which switch status was changed
        /// </summary>

        public int order { get { return _order; } set { _order = value; } }

        public SwitchTile(object parent)
            : base(parent)
        {
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("SwitchTile");
            xmlw.WriteAttributeString("active", _active.ToString());
            xmlw.WriteAttributeString("id", _id.ToString());
            xmlw.WriteAttributeString("order", _order.ToString());
            xmlw.WriteEndElement();
        }
    }
}
