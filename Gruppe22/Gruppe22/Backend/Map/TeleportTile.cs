using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Schema;

namespace Gruppe22
{
    public class TeleportTile : Tile
    {
        #region Private Fields
        /// <summary>
        /// Path to the .xml for the next Room
        /// </summary>
        String _nextRoom;
        /// <summary>
        /// Spawn position for the next room
        /// </summary>
        Coords _nextPlayerPos;
        #endregion

        #region Public Fields
        public String nextRoom
        {
            get
            {
                return _nextRoom;
            }
            set
            {
                _nextRoom = value;
            }
        }

        public Coords nextPlayerPos
        {
            get
            {
                return _nextPlayerPos;
            }
        }
        #endregion

        public override bool Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("TeleportTile");
            writer.WriteAttributeString("Ziel", nextRoom);
            string zipo=nextPlayerPos.x+" "+nextPlayerPos.y;
            writer.WriteAttributeString("ZielPosition", zipo);
            writer.WriteEndElement();
            return true;
        }

        #region Constructor
        public TeleportTile(string nextXml, Coords pos)
        {
            _nextRoom = nextXml;
            _nextPlayerPos = pos;
        }
        #endregion
    }
}
