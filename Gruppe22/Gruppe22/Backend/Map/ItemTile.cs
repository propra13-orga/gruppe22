using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A class used to manage items laying on the floor of a room.
    /// </summary>
    public class ItemTile : Tile
    {
        Item _item = null;
        /// <summary>
        /// The item which will lay on the floor.
        /// </summary>
        public Item item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// The type of the item,
        /// a potion by default.
        /// </summary>
        public ItemType itemType
        {
            get
            {
                if (item != null)
                    return _item.itemType;
                else
                    return Backend.ItemType.Potion;
            }
        }

        /// <summary>
        /// Method save the ItemTile in a .xml file.
        /// Just writes a start tag and calls the save method for the item.
        /// </summary>
        /// <param name="xmlw">Xmlwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ItemTile");
            _item.Save(xmlw);
            xmlw.WriteEndElement();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item">The item to place on the ItemTile</param>
        public ItemTile(object parent, Item item)
            : base(parent)
        {
            _item = item;
        }


        public ItemTile(object parent)
            : base(parent)
        {
        }
    }
}
