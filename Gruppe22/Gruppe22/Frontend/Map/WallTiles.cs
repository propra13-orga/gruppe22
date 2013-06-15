using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{

    public enum WallType
    {
        Normal = 0,
        OpenDoor = 1,
        ClosedDoor = 2,
        Deco1 = 3,
        Deco2 = 4,
        Deco3 = 5
    }
    public class WallTiles : TileSet
    {


        #region Public Methods
        /// <summary>
        /// Load object from XML-file
        /// </summary>
        /// <param name="reader">Load object from XML-stream</param>
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            _width = Int32.Parse(reader.GetAttribute("width"));
            _height = Int32.Parse(reader.GetAttribute("height"));
            Boolean isEmptyElement = reader.IsEmptyElement;

            if (isEmptyElement)
                return;

            reader.ReadStartElement("WallTiles");
            reader.ReadStartElement("walls");
            while ((reader.NodeType != System.Xml.XmlNodeType.EndElement) && (reader.NodeType != System.Xml.XmlNodeType.None))
            {
                while (reader.NodeType == System.Xml.XmlNodeType.Whitespace) reader.Read();
                TileObject temp = new TileObject(_content, _width, _height);
                WallType _type = WallType.Normal;
                if (reader.GetAttribute("Type") != null)
                    _type = (WallType)Enum.Parse(typeof(WallType), reader.GetAttribute("Type").ToString());
                WallDir _id = (WallDir)Enum.Parse(typeof(WallDir), reader.GetAttribute("Direction").ToString());
                _textures[(int)_type * 100 + (int)_id].ReadXml(reader);
            }
            reader.ReadEndElement();


            while ((reader.NodeType != System.Xml.XmlNodeType.EndElement) && (reader.NodeType != System.Xml.XmlNodeType.None))
                reader.Read();
            reader.ReadEndElement();
        }

        /// <summary>
        /// Save (merely a shortcut to the serializer
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename = "bla.xml")
        {
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, Encoding.Unicode);
            XmlSerializer serializer = new XmlSerializer(typeof(WallTiles));
            serializer.Serialize(writer, this);
            writer.Close();
        }

        /// <summary>
        /// Load (cannot use serializer at this level!)
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename = "bla.xml")
        {
            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(filename, settings);
            ReadXml(reader);
            reader.Close();
        }

        /// <summary>
        /// Dump object to XML-file
        /// </summary>
        /// <param name="writer">Write object to XML-stream</param>
        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("width", _width.ToString());
            writer.WriteAttributeString("height", _height.ToString());
            writer.WriteStartElement("walls");
            for (int i = 0; i < _textures.Count; ++i)
            {
                if (_textures[i].animationFile != "")
                {
                    writer.WriteStartElement("wall");
                    writer.WriteAttributeString("Direction", ((WallDir)(i % 100)).ToString());
                    writer.WriteAttributeString("Type", ((WallType)(i / 100)).ToString());
                    _textures[i].WriteXml(writer);
                    writer.WriteEndElement();
                }

            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// Add a new wall
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="direction"></param>
        /// <param name="cutOut"></param>
        public void Add(string filename, WallDir direction, Rectangle cutOut, Coords offset = null, Coords crop = null, WallType subtype = WallType.Normal)
        {
            if (offset == null) offset = Coords.Zero;
            if (crop == null) crop = Coords.Zero;
            if ((int)direction + (int)subtype * 100 < _textures.Count)
            {
                // _textures[(int)direction].width = cutOut.Width;
                // _textures[(int)direction].height = cutOut.Height;
                _textures[(int)direction + (int)subtype * 100].AddAnimation(filename, new Coords(cutOut.Left, cutOut.Top), 1, 1, false);
                _textures[(int)direction + (int)subtype * 100].cropX = crop.x;
                _textures[(int)direction + (int)subtype * 100].cropY = crop.y;
                _textures[(int)direction + (int)subtype * 100].offsetX = offset.x;
                _textures[(int)direction + (int)subtype * 100].offsetY = offset.y;
            }


        }

        #endregion

        #region Constructor

        public WallTiles()
        {

        }

        public WallTiles(ContentManager content, int width, int height, string fileName = "")
            : base(content, width, height, fileName)
        {
            _textures = new List<TileObject>();
            for (int i = 0; i < Enum.GetValues(typeof(WallType)).Length * 100 + Enum.GetValues(typeof(WallDir)).Length; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }

        }
        #endregion
    }
}
