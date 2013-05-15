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
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            xmlw.WriteEndElement();
        }
    }
}
