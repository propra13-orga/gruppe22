﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class WallTile : Tile
    {
        public WallTile()
        {
            canEnter = false;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("WallTile");
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            xmlw.WriteAttributeString("connected", Convert.ToString(connected));
            xmlw.WriteAttributeString("connection", Convert.ToString(connection));
            foreach (Tile tile in overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }
    }
}
