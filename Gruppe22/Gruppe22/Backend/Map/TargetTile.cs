using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gruppe22.Backend
{
    /// <summary>
    /// The tile which ends the game if the player enters it.
    /// </summary>
    public class TargetTile : Tile
    {
        /// <summary>
        /// A constructor.
        /// </summary>
        /// <param name="parent"></param>
        public TargetTile(object parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Method to save the tile in a XML-file.
        /// </summary>
        /// <param name="xmlw">XMLwriter</param>
        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TargetTile");
            xmlw.WriteEndElement();
        }
    }
}
