using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class CheckpointTile : Tile
    {
        /// <summary>
        /// Legt fest ob dieser Checkpoint schon besucht wurde.
        /// </summary>
        private bool _visited = false;
        /// <summary>
        /// Bonusleben für den Spieler.
        /// </summary>
        private int _bonuslife = 0;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="parent">Elternobjekt.</param>
        /// <param name="visited">Besuchtfeld.</param>
        /// <param name="bonuslife">Bonusleben.</param>
        public CheckpointTile(object parent, bool visited, int bonuslife) :
            base(parent)
        {
            _visited = visited;
            _bonuslife = bonuslife;
        }

        /// <summary>
        /// Öffentliche Eigenschaft des Feldes
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
        /// Öffentliche Eigenschaft zu den Bonusleben.
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
        /// Speichert das CheckpointTile in den Xml-Datenstrom.
        /// </summary>
        /// <param name="xmlw">Der zu verwendete XmlWriter.</param>
        public override void Save(System.Xml.XmlWriter xmlw)
        {
            xmlw.WriteStartElement("CheckpointTile");
            xmlw.WriteAttributeString("visited", _visited.ToString());
            xmlw.WriteAttributeString("bonuslife", _bonuslife.ToString());
            xmlw.WriteEndElement();
        }
    }
}
