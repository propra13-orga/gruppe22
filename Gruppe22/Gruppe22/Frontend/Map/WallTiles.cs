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
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                TileObject temp = new TileObject(_content, _width, _height);
                WallDir _id = (WallDir)Enum.Parse(typeof(WallDir), reader.GetAttribute("Direction").ToString());
                System.Diagnostics.Debug.WriteLine(_id);
                _textures[(int)_id].ReadXml(reader);
            }
            reader.ReadEndElement();


            while (reader.NodeType != System.Xml.XmlNodeType.EndElement) reader.Read();
            reader.ReadEndElement();
        }

        /// <summary>
        /// Add a new wall
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="direction"></param>
        /// <param name="cutOut"></param>
        public void Add(string filename, WallDir direction, Rectangle cutOut)
        {
            if ((int)direction < _textures.Count)
                _textures[(int)direction].AddAnimation(filename, new Coords(cutOut.Left, cutOut.Top), 1, 1, false);
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
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);
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
                writer.WriteStartElement("wall");
                writer.WriteAttributeString("Direction", ((WallDir)i).ToString());
                _textures[i].WriteXml(writer);
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
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
            for (int i = 0; i < Enum.GetValues(typeof(WallDir)).Length ; ++i)
            {
                _textures.Add(new TileObject(_content, _width, _height));
            }

        }
        #endregion
    }
}
