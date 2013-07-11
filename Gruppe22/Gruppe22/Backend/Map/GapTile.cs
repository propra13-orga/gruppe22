using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile to have something like a hole in the floor.
    /// </summary>
    class GapTile : Tile
    {
        #region Private Fields
        private int _style = 0;
        #endregion

        #region Public Fields
        /// <summary>
        /// The style of the gap
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
        /// Method to save the GapTile in a .xml file
        /// </summary>
        /// <param name="xmlw">XmlWriter.</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("GapTile");
            xmlw.WriteAttributeString("style", _style.ToString());
            xmlw.WriteEndElement();
        }
        #endregion

        #region Constructor
        /// <summary>
        /// A empty constructor.
        /// </summary>
        /// <param name="parent">parent.</param>
        public GapTile(object parent):base(parent)
        {
        }
        #endregion
    }
}
