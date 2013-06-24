using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22.Backend
{
    public class CheckpointTile : Tile
    {
        private bool _visited = false;
        private int _bonuslife = 0;

        public CheckpointTile(object parent, bool visited, int bonuslife) :
            base(parent)
        {
            _visited = visited;
            _bonuslife = bonuslife;
        }

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

        public override void Save(System.Xml.XmlWriter xmlw)
        {
            xmlw.WriteStartElement("CheckpointTile");
            xmlw.WriteAttributeString("visited", _visited.ToString());
            xmlw.WriteAttributeString("bonuslife", _bonuslife.ToString());
            xmlw.WriteEndElement();
        }
    }
}
