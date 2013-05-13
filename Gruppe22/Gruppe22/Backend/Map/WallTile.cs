using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class WallTile : Tile
    {
        public WallTile(object parent):base(parent)
        {
            _canEnter = false;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }
    }
}
