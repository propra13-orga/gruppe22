using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class TargetTile : Tile
    {

        public TargetTile(object parent)
            : base(parent)
        {

        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TargetTile");

            xmlw.WriteEndElement();
        }
    }
}
