using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Schema;

namespace Gruppe22.Backend
{
    public class TeleportTile : Tile
    {
        #region Private Fields
        /// <summary>
        /// Path to the .xml for the next Room
        /// </summary>
        private string _nextRoom;
        /// <summary>
        /// Spawn position for the next room
        /// </summary>
        private Backend.Coords _nextPlayerPos;

        private bool _hidden = false;
        private bool _enabled = true;
        private bool _teleport = false;
        private bool _down = false;
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

        public Backend.Coords nextPlayerPos
        {
            get
            {
                return _nextPlayerPos;
            }
        }
        #endregion

        #region Constructor

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


        public TeleportTile(object parent, string nextXml, Backend.Coords pos, bool isTeleport = false, bool isHidden = false, bool isEnabled = true, bool isUp=false)
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

        #region Public Methods

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
    }
}
