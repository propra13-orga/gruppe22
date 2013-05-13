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
        public override void Save(XmlWriter writer)
        {
            writer.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            writer.WriteValue("Floor");
        }
        #region Constructor
        /// <summary>
        /// You can always walk over a Floor
        /// </summary>
        public FloorTile()
        {
            _canEnter = true;
        }
        #endregion
    }
}
