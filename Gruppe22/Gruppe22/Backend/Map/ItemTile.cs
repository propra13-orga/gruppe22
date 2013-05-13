﻿using System;
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
                return _item.itemType;
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
            xmlw.WriteAttributeString("canEnter", Convert.ToString(canEnter));
            xmlw.WriteAttributeString("connected", Convert.ToString(connected));
            xmlw.WriteAttributeString("connection", Convert.ToString(connection));
            foreach (Tile tile in _overlay)
            {
                tile.Save(xmlw);
            }
            xmlw.WriteEndElement();
            _parent = parent;
        }

        public ItemTile(object parent)
            : base(parent, null, true)
        {
            _parent = parent;

        }
    }
}
