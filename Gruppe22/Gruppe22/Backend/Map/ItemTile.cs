using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class ItemTile : Tile
    {
        Item _item = null;

        public Item item
        {
            get { return _item; }
        }
        public ItemType itemType
        {
            get
            {
                if (item != null)
                    return _item.itemType;
                else
                    return ItemType.Potion;
            }
        }

        public ItemTile(object parent, Item item)
            : base(parent)
        {
            _item = item;
        }


        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("ItemTile");
            xmlw.WriteAttributeString("name", item.name);
            xmlw.WriteAttributeString("strength", item.strength.ToString());
            xmlw.WriteAttributeString("type", item.itemType.ToString());
            xmlw.WriteEndElement();
            _parent = parent;
        }

        public ItemTile(object parent)
            : base(parent)
        {

        }
    }
}
