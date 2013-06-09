using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gruppe22
{
    class TriggerTile : Tile
    {
        private int _countdown = -1;
        private Coords _affectedTile = Coords.Zero;
        private string _explanationDisabled = "";
        private string _explanationEnabled = "";
        private bool _enabled = true;
        private int _repeat = -1;
        private bool _alwaysShowDisabled = false;
        private bool _alwaysShowEnabled = true;
        private string _tileimage = "";
        private bool _reactToEnemies = false;
        private bool _reactToObjects = false;
        private int _reactToItem = -1;


        public int countdown
        {
            get
            {
                return _countdown;
            }
            set
            {
                _countdown = value;
            }
        }


        public Coords affectedTile
        {
            get
            {
                return _affectedTile;
            }
            set
            {
                _affectedTile = value;
            }
        }

        public string explanationDisabled
        {
            get
            {
                return _explanationDisabled;
            }
            set
            {
                _explanationDisabled = value;
            }
        }

        public string explanationEnabled
        {
            get
            {
                return _explanationEnabled;
            }
            set
            {
                _explanationEnabled = value;
            }
        }


        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public int repeat
        {
            get
            {
                return _repeat;
            }
            set
            {
                _repeat = value;
            }
        }

        public bool alwaysShowDisabled
        {
            get
            {
                return _alwaysShowDisabled;
            }
            set
            {
                _alwaysShowDisabled = value;
            }
        }



        public bool alwaysShowEnabled
        {
            get
            {
                return _alwaysShowEnabled;
            }
            set
            {
                _alwaysShowEnabled = value;
            }
        }

        public string tileimage
        {
            get
            {
                return _tileimage;
            }
            set
            {
                _tileimage = value;
            }
        }

        public bool reactToEnemies
        {
            get
            {
                return _reactToEnemies;
            }
            set
            {
                _reactToEnemies = value;
            }
        }



        public bool reactToObjects
        {
            get
            {
                return _reactToObjects;
            }
            set
            {
                _reactToObjects = value;
            }
        }

        public int reactToItem
        {
            get
            {
                return _reactToItem;
            }
            set
            {
                _reactToItem = value;
            }
        }

        public TriggerTile(object parent, Coords affected = null, int countdown = -1, string explanationEnabled = "", string explanationDisabled = "", bool enabled = true, int repeat = -1, bool alwaysShowEnabled = true, bool alwaysShowDisabled = false, string tileImage = "", bool reactToEnemies = false, bool reactToObjects = false, int reactToItem = -1)
            : base(parent)
        {
            _countdown = countdown;
            if (affected != null) _affectedTile = affected; else _affectedTile = new Coords(-1, -1);
            _explanationDisabled = explanationDisabled;
            _explanationEnabled = explanationEnabled;
            _enabled = enabled;
            _repeat = -1;
            _alwaysShowDisabled = alwaysShowDisabled;
            _alwaysShowEnabled = alwaysShowEnabled;
            _tileimage = tileimage;
            _reactToEnemies = reactToEnemies;
            _reactToObjects = reactToObjects;
            _reactToItem = reactToItem;
        }

        public override void Save(XmlWriter xmlw)
        {
            xmlw.WriteStartElement("TriggerTile");
            xmlw.WriteAttributeString("affectX", _affectedTile.x.ToString());

            xmlw.WriteAttributeString("affectY", _affectedTile.y.ToString());
            xmlw.WriteAttributeString("explanation", Convert.ToString(_explanationEnabled));
            xmlw.WriteAttributeString("explanationdisabled", Convert.ToString(_explanationDisabled));
            xmlw.WriteAttributeString("enabled", Convert.ToString(_enabled));
            xmlw.WriteAttributeString("repeat", Convert.ToString(_repeat));

            xmlw.WriteAttributeString("alwaysShowDisabled", Convert.ToString(_alwaysShowDisabled));
            xmlw.WriteAttributeString("alwaysShowEnabled", Convert.ToString(_alwaysShowEnabled));


            xmlw.WriteAttributeString("tileimage", Convert.ToString(_tileimage));
            xmlw.WriteAttributeString("reactToEnemies", Convert.ToString(_reactToEnemies));
            xmlw.WriteAttributeString("reactToObjects", Convert.ToString(_reactToObjects));

            xmlw.WriteAttributeString("reactToItem", Convert.ToString(_reactToItem));




            xmlw.WriteEndElement();
        }
    }
}
