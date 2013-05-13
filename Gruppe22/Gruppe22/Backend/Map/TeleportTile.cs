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
        string _nextRoom;
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

        #region Constructor
        public TeleportTile(string nextXml, Coords pos)
        {
            _nextRoom = nextXml;
            _nextPlayerPos = pos;
        }
        #endregion

        #region Public-Methods

        public TeleportTile(object parent, string nextRoom, Coords nextPlayerPos)
            : base(parent)
        {
            this._nextRoom = nextRoom;
            this._nextPlayerPos = nextPlayerPos;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TeleportTile");
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            xmlw.WriteAttributeString("nextRoom", _nextRoom);
            xmlw.WriteAttributeString("nextX", Convert.ToString(_nextPlayerPos.x));
            xmlw.WriteAttributeString("nextY", Convert.ToString(_nextPlayerPos.y));
            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
        }
        #endregion
    }
}
