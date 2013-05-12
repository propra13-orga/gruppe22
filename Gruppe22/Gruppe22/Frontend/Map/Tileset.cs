using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gruppe22
{

    /// <summary>
    /// Different wall-directions
    /// </summary>
    public enum WallDir
    {
        None = 0,
        LeftRightUp,
        LeftRightDown,
        UpDownLeft,
        UpDownRight,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        LeftRight,
        UpDown,
        FourWay,
        LeftClose,
        DownClose,
        RightClose,
        UpClose,
        Free,
        LeftRightUpDiag,
        LeftRightDownDiag,
        UpDownLeftDiag,
        UpDownRightDiag,
        UpLeftDiag,
        UpRightDiag,
        DownLeftDiag,
        DownRightDiag,
        LeftRightDiag,
        UpDownDiag,
        FourWayDiag,
        LeftCloseDiag,
        DownCloseDiag,
        RightCloseDiag,
        UpCloseDiag,
        FourDiag,
        DiagUpClose,
        DiagDownClose,
        DiagUpDownClose,
        DiagUpClose2,
        DiagDownClose2,
        DiagUpDownClose2,
        DiagLeftClose,
        DiagRightClose,
        DiagLeftRightClose,
        DiagLeftClose2,
        DiagRightClose2,
        DiagLeftRightClose2
    }


    public class TileSet: IXmlSerializable
    {
       #region Private Fields
        protected string _fileName = "";
        protected List<TileObject> _textures;
        protected int _width = 0;
        protected int _height = 0;
        protected ContentManager _content = null;
        #endregion

        #region Public Fields
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public TileObject this[int i]
        {
            get
            {
                return _textures[i];
            }
        }

        /// <summary>
        /// Width of elements in the tileset
        /// </summary>
        public int width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Height of elements in the tileset
        /// </summary>
        public int height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        #endregion

        #region Public Methods
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
        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            _width = Int32.Parse(reader.GetAttribute("width"));
            _height = Int32.Parse(reader.GetAttribute("height"));
            Boolean isEmptyElement = reader.IsEmptyElement;

            if (isEmptyElement)
                return;

            reader.ReadStartElement("TileSet");
            reader.ReadStartElement("Tile");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                TileObject temp = new TileObject(_content, _width, _height);
                int _id = Int32.Parse(reader.GetAttribute("ID").ToString());
                while(_id>_textures.Count){
                    _textures.Add(new TileObject());
                }
                _textures[_id].ReadXml(reader);
            }
            reader.ReadEndElement();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement) reader.Read();
            reader.ReadEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="id"></param>
        /// <param name="cutOut"></param>
        public virtual void Add(string filename, int id, Rectangle cutOut)
        {
            if ((int)id < _textures.Count)
                _textures[id].AddAnimation(filename, new Vector2(cutOut.Left, cutOut.Top), 1, 1, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public virtual void Save(string filename = "bla.xml")
        {
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filename, Encoding.Unicode);

            XmlSerializer serializer = new XmlSerializer(typeof(WallTiles));
            serializer.Serialize(writer, this);
            writer.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public virtual void Load(string filename = "bla.xml")
        {
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(filename);
            ReadXml(reader);
            reader.Close();
        }

        /// <summary>
        /// Dump object to XML-file
        /// </summary>
        /// <param name="writer">Write object to XML-stream</param>
        public virtual void WriteXml(System.Xml.XmlWriter writer)
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

        /// <summary>
        /// Necessary Constructor for serialization; avoid otherwise (!)
        /// </summary>
        public TileSet()
        {

        }

        public TileSet(ContentManager content, int width, int height, string fileName = "")
        {
            _textures = new List<TileObject>();
            _width = width;
            _height = height;
            _content = content;
        }
        #endregion
    }

}
