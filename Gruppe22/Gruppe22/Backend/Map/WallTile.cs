using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Gruppe22
{
    public class WallTile : Tile
    {
        public override bool Save(XmlTextWriter writer)
        {
            writer.WriteValue("Wall");
            return true;
        }
        
        public WallTile()
        {
            canEnter = false;
        }

    }
}
