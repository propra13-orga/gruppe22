using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    class ProjectileTile : Tile
    {
         public ProjectileTile(object parent):base(parent)
        {
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ProjectileTile");
            xmlw.WriteEndElement();
        }
    }
}
