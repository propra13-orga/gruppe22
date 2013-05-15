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
        public TeleportTile(object parent, string nextXml, Coords pos)
            : base(parent)
        {
            _nextRoom = nextXml;
            _nextPlayerPos = pos;
        }
        #endregion

        #region Public Methods

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TeleportTile");
            xmlw.WriteAttributeString("nextRoom", _nextRoom);
            xmlw.WriteAttributeString("nextX", Convert.ToString(_nextPlayerPos.x));
            xmlw.WriteAttributeString("nextY", Convert.ToString(_nextPlayerPos.y));

            xmlw.WriteEndElement();
        }
        #endregion
    }
}
