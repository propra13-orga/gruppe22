﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    public class DoorTile : WallTile
    {
        /// <summary>
        /// Whether the door is open
        /// </summary>
        private bool _open = false;
        /// <summary>
        /// ID of key used to open door; 0 for no key
        /// </summary>
        private int _key = 0;
        /// <summary>
        /// Whether the door is open
        /// </summary>        
        public bool open { get { return _open; } set { _open = value; } }
        /// <summary>
        /// ID of key used to open door; 0 for no key
        /// </summary>

        public int key { get { return _key; } set { _key = value; } }

        public DoorTile(object parent, bool locked = true, int key = 0)
            : base(parent)
        {
            _key = key;
            _open = !locked;
        }


        new public WallType type
        {
            get
            {
                if (_open) return Backend.WallType.OpenDoor;
                else return Backend.WallType.ClosedDoor;
            }
        }
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("DoorTile");
            xmlw.WriteAttributeString("open", _open.ToString());
            xmlw.WriteAttributeString("key", _key.ToString());
            xmlw.WriteEndElement();
        }
    }
}
