using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Schema;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile used to connect rooms.
    /// If the player enters this tile he changes the room.
    /// </summary>
    public class TeleportTile : Tile
    {
        #region Private Fields

        private string _nextRoom;
        private Backend.Coords _nextPlayerPos;
        private bool _hidden = false;
        private bool _enabled = true;
        private bool _teleport = false;
        private bool _down = false;

        #endregion

        #region Public Fields

        /// <summary>
        /// The path to the .xml file for the room the player enters.
        /// </summary>
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

        /// <summary>
        /// The coordinates the player stands on after entering the next room.
        /// </summary>
        public Backend.Coords nextPlayerPos
        {
            get
            {
                return _nextPlayerPos;
            }
        }
        
        /// <summary>
        /// Bool to determine if the teleporter is visible.
        /// </summary>
        public bool hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool teleport
        {
            get
            {
                return _teleport;
            }
            set
            {
                _teleport = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool down
        {
            get
            {
                return _down;
            }
            set
            {
                _down = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// The method to save a TeleportTile in a XML-file.
        /// Just writes all properties to the file.
        /// </summary>
        /// <param name="xmlw">A XMLwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TeleportTile");
            xmlw.WriteAttributeString("nextRoom", _nextRoom);
            xmlw.WriteAttributeString("nextX", Convert.ToString(_nextPlayerPos.x));
            xmlw.WriteAttributeString("nextY", Convert.ToString(_nextPlayerPos.y));
            xmlw.WriteAttributeString("hidden", Convert.ToString(_hidden));
            xmlw.WriteAttributeString("enabled", Convert.ToString(_enabled));
            xmlw.WriteAttributeString("down", Convert.ToString(_down));
            xmlw.WriteAttributeString("teleport", Convert.ToString(_teleport));
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor

        /// <summary>
        /// The constructor for the TeleportTile.
        /// Setting default values.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="nextXml"></param>
        /// <param name="pos"></param>
        /// <param name="isTeleport"></param>
        /// <param name="isHidden"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isUp"></param>
        public TeleportTile(object parent, string nextXml, Backend.Coords pos, bool isTeleport = false, bool isHidden = false, bool isEnabled = true, bool isUp = false)
            : base(parent)
        {
            _nextRoom = nextXml;
            _nextPlayerPos = pos;
            _teleport = isTeleport;
            _hidden = isHidden;
            _enabled = isEnabled;
            _down = !isUp;
        }
        #endregion
    }
}
