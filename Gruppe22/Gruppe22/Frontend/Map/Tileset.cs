using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Gruppe22
{
    public class Tileset : IXmlSerializable
    {
        private string _fileName = "";
        private List<TileObject> _walls;
        private List<TileObject> _floor;
        private int _width = 0;
        private int _height = 0;


        /// <summary>
        /// Useless function required by IXmlSerializable
        /// </summary>
        /// <returns>Null (always)</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;

        }

        /// <summary>
        /// Load object from XML-file
        /// </summary>
        /// <param name="reader">Load object from XML-stream</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer tileReader = new XmlSerializer(typeof(TileObject));
            reader.MoveToContent();
            Boolean isEmptyElement = reader.IsEmptyElement; // (1)
            reader.ReadStartElement();

            if (isEmptyElement)
                return;

            _width = Int32.Parse(reader.ReadElementString("width"));
            _height = Int32.Parse(reader.ReadElementString("height"));

            reader.Read();

            reader.ReadStartElement("walls");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("wall");
                TileObject temp = (TileObject)tileReader.Deserialize(reader);
                _walls.Add(temp);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();

            reader.ReadStartElement("floors");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("floor");
                TileObject temp = (TileObject)tileReader.Deserialize(reader);
                _floor.Add(temp);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void Add(string filename, Direction direction, Rectangle cutOut)
        {
            _walls.Add(new TileObject(null, cutOut.Width, cutOut.Height));
            _walls[(int)direction].AddAnimation(filename, new Vector2(cutOut.Left, cutOut.Top), -1, 1, 1, false);
        }


        public void Save(string filename = "bla.xml")
        {
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, Encoding.Unicode);
            WriteXml(writer);
            writer.Close();
        }

        public void Load(string filename = "bla.xml")
        {
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);
            ReadXml(reader);
            reader.Close();
        }

        /// <summary>
        /// Dump object to XML-file
        /// </summary>
        /// <param name="writer">Write object to XML-stream</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("width", _width.ToString());
            writer.WriteAttributeString("height", _height.ToString());
            writer.WriteStartElement("walls");
            foreach (TileObject wall in _walls)
            {
                writer.WriteStartElement("wall");
                wall.WriteXml(writer);
                writer.WriteEndElement();

            }
            writer.WriteEndElement();
            writer.WriteStartElement("floors");
            foreach (TileObject floor in _floor)
            {
                writer.WriteStartElement("floor");
                floor.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
