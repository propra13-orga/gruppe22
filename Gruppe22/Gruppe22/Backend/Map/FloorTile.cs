using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Gruppe22
{
    class FloorTile : Tile
    {
        public override bool Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Floortile");
            writer.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            writer.WriteEndElement();            
            return true;
        }
        #region Constructor
        /// <summary>
        /// You can always walk over a Floor
        /// </summary>
        public FloorTile()
        {
            canEnter = true;
        }
        #endregion
    }
}
