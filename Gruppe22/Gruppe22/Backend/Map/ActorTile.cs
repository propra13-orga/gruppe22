using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class ActorTile : Tile
    {
        Actor _actor;
        public Actor actor
        {
            get { return _actor; }
        }
        public ActorType actorType
        {
            get
            {
                return _actor.actorType;
            }
        }

        override public void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ActorTile");
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
