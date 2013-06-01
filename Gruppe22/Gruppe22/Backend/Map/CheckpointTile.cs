using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    public class CheckpointTile : Tile
    {
        public CheckpointTile(object parent) :
            base(parent)
        { 
        }

        public override void Save(System.Xml.XmlWriter xmlw)
        {
            xmlw.WriteStartElement("CheckpointTile");
            xmlw.WriteEndElement();
        }
    }
}
