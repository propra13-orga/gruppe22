using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    class GapTile : Tile
    {
        #region Private Fields
        /// <summary>
        /// Stil der Spalt-Tiles
        /// </summary>
        private int _style = 0;
        #endregion

        #region Public Fields
        /// <summary>
        /// Öffentliche Eigenschaft 
        /// </summary>
        public int style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Speichert dieses Tile in die XML.
        /// </summary>
        /// <param name="xmlw">Der zu verwendete XmlWriter.</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("GapTile");
            xmlw.WriteAttributeString("style", _style.ToString());
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="parent">Elternobjekt.</param>
        public GapTile(object parent):base(parent)
        {
        }
        #endregion
    }
}
