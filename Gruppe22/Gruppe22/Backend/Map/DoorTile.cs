using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// This tile is used to seperate the different levels.
    /// A boss enemy in one level should have the key to the next level.
    /// </summary>
    public class DoorTile : WallTile
    {
        private bool _open = false;
        private int _key = 0;
        /// <summary>
        /// True if the door can be passed /is opened
        /// </summary>        
        public bool open { get { return _open; } set { _open = value; } }
        /// <summary>
        /// The id of the key for this door.
        /// The player needs this key to open the door.
        /// 0 if no key is needed.
        /// </summary>
        public int key { get { return _key; } set { _key = value; } }

        /// <summary>
        /// Constructor.
        /// Gets the number of the key and if the door is locked
        /// </summary>
        /// <param name="parent">parent.</param>
        /// <param name="locked">Status of the door</param>
        /// <param name="key">int id of the key</param>
        public DoorTile(object parent, bool locked = true, int key = 0)
            : base(parent)
        {
            _key = key;
            _open = !locked;
        }

        /// <summary>
        /// The DoorTile is a type of WallTile
        /// </summary>
        new public WallType type
        {
            get
            {
                if (_open) return Backend.WallType.OpenDoor;
                else return Backend.WallType.ClosedDoor;
            }
        }

        /// <summary>
        /// Method to save the DoorTile in a .xml file
        /// </summary>
        /// <param name="xmlw">XmlWriter.</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("DoorTile");
            xmlw.WriteAttributeString("open", _open.ToString());
            xmlw.WriteAttributeString("key", _key.ToString());
            xmlw.WriteEndElement();
        }
    }
}
