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

        #region XML SaveLoad
        /// <summary>
        /// see Tile.cs
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public override bool Save(XmlTextWriter writer)
        {
            writer.WriteAttributeString("Ziel", nextRoom);
            writer.WriteAttributeString("x", Convert.ToString(_nextPlayerPos.x));
            writer.WriteAttributeString("y", Convert.ToString(_nextPlayerPos.y));
            writer.WriteValue("Teleport");
            return true;
        }
        public bool Load(XmlTextReader reader)
        {
            canEnter = Convert.ToBoolean(reader.GetAttribute("canEnter"));
            connected = Convert.ToBoolean(reader.GetAttribute("connected"));
            string con = reader.GetAttribute("connection");
            if (con.Equals("invalid"))
                connection = Connection.Invalid;
            _nextRoom = reader.GetAttribute("Ziel");
            _nextPlayerPos.x = Convert.ToInt32(reader.GetAttribute("x"));
            _nextPlayerPos.y = Convert.ToInt32(reader.GetAttribute("y"));
            return true;
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
