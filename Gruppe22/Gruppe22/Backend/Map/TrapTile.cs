using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

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

        public override bool Save(XmlTextWriter writer)
        {
            writer.WriteAttributeString("Schaden", damage.ToString());
            return true;
        }
    }
}
