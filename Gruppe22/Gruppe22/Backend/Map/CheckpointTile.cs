using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    /// <summary>
    /// A tile used as a saving point for the player.
    /// </summary>
    public class CheckpointTile : Tile
    {
        private bool _visited = false;
        private int _bonuslife = 0;

        /// <summary>
        /// Constructor for the Checkpoint.
        /// Can get passed the bonuslifes and if it was visited before
        /// </summary>
        /// <param name="parent">parentobject.</param>
        /// <param name="visited">Set the visited attribute</param>
        /// <param name="bonuslife">Set the bonuslifes</param>
        public CheckpointTile(object parent, bool visited, int bonuslife) :
            base(parent)
        {
            _visited = visited;
            _bonuslife = bonuslife;
        }

        /// <summary>
        /// True if the checkpoint was activated before
        /// </summary>
        public bool visited
        {
            get
            {
                return _visited;
            }
            set
            {
                _visited = value;
            }
        }

        /// <summary>
        /// A number of bonus lifes the player could be granted
        /// </summary>
        public int bonuslife
        {
            get
            {
                return _bonuslife;
            }
            set
            {
                _bonuslife = value;
            }
        }

        /// <summary>
        /// Method to save the CheckpointTile in a .xml file
        /// </summary>
        /// <param name="xmlw">XmlWriter.</param>
        public override void Save(System.Xml.XmlWriter xmlw)
        {
            xmlw.WriteStartElement("CheckpointTile");
            xmlw.WriteAttributeString("visited", _visited.ToString());
            xmlw.WriteAttributeString("bonuslife", _bonuslife.ToString());
            xmlw.WriteEndElement();
        }
    }
}
