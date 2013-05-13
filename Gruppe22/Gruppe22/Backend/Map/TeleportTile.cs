using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

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
            set
            {
                _nextPlayerPos = value;
            }
        }
        #endregion

#region Public-Methods

        public TeleportTile(object parent,string nextRoom, Coords nextPlayerPos)
            : base(parent)
        {
            this._nextRoom = nextRoom;
            this._nextPlayerPos = nextPlayerPos;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TeleportTile");
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            xmlw.WriteAttributeString("connected", Convert.ToString(connected));
            xmlw.WriteAttributeString("connection", Convert.ToString(connection));
            xmlw.WriteAttributeString("nextRoom", _nextRoom);
            xmlw.WriteAttributeString("nextPlayerPos", _nextPlayerPos.ToString());
            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }
#endregion
    }
}
