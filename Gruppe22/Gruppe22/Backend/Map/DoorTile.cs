using System;
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
        /// Tür offen oder nicht
        /// </summary>
        private bool _open = false;
        /// <summary>
        /// Die ID des Schlüssels mit dem diese Tür geöffnet werden kann; 0 bedeutet man braucht kein Schlüssel
        /// </summary>
        private int _key = 0;
        /// <summary>
        /// Öffentliche Eigenschaft für den Zustand der Tür
        /// </summary>        
        public bool open { get { return _open; } set { _open = value; } }
        /// <summary>
        /// Öffentliche Eigenschaft für die Schlüssel-ID
        /// </summary>
        public int key { get { return _key; } set { _key = value; } }
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="parent">Elternobjekt.</param>
        /// <param name="locked">Verschlossen?</param>
        /// <param name="key">Schlüssel-ID</param>
        public DoorTile(object parent, bool locked = true, int key = 0)
            : base(parent)
        {
            _key = key;
            _open = !locked;
        }

        /// <summary>
        /// Neue öffentliche Eigenschaft s. WallType.
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
        /// Speichert das WallTile in die XML.
        /// </summary>
        /// <param name="xmlw">Der zu verwendete XmlWriter.</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("DoorTile");
            xmlw.WriteAttributeString("open", _open.ToString());
            xmlw.WriteAttributeString("key", _key.ToString());
            xmlw.WriteEndElement();
        }
    }
}
