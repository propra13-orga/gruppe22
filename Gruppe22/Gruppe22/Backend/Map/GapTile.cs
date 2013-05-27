using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    class GapTile : Tile
    {
        #region Private Fields
        private int _style = 0;
        #endregion

        #region Public Fields
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
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("GapTile");
            xmlw.WriteAttributeString("style", _style.ToString());
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor
        public GapTile(object parent):base(parent)
        {
        }
        #endregion
    }
}
