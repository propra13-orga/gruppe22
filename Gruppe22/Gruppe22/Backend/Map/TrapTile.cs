using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22
{
    public class TrapTile : Tile
    {
        #region Private Fields
        /// <summary>
        /// the damage a trap deals to the player
        /// </summary>
        private int _damage;
        #endregion

        #region Public Fields
        public int damage
        {
            get
            {
                return _damage;
            }
            set
            {
                _damage = value;
            }
        }
        #endregion

        public TrapTile(object parent,int dmg)
            : base(parent)
        {
            _damage = dmg;
        }
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TrapTile");
            xmlw.WriteAttributeString("damage", Convert.ToString(_damage));

            xmlw.WriteEndElement();
        }
    }
}
