using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    /// <summary>
    /// An animation phase
    /// </summary>
    public class VisibleObject : IXmlSerializable
    {
        #region Private Fields
        private string _srcFile = "";
        private Rectangle _clipRect;
        private Texture2D _texture = null;
        private ContentManager _content = null;
        #endregion

        #region Public Fields
        /// <summary>
        /// 
        /// </summary>
        public string src { get { return _srcFile; } set { _srcFile = value; } }
        /// <summary>
        /// 
        /// </summary>
        public Rectangle clipRect { get { return _clipRect; } set { _clipRect = value; } }

        public Texture2D texture { get { return _texture; } }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public VisibleObject(ContentManager content, string srcFile, Rectangle clipRect)
        {
            _srcFile = srcFile;
            _clipRect = clipRect;
            _content = content;
            _texture = _content.Load<Texture2D>(_srcFile);
        }
        #endregion

        /// <summary>
        /// Useless function from the IXmlSerializable-Interface
        /// </summary>
        /// <returns>null</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Get object from an XML-stream
        /// </summary>
        /// <param name="reader">The reader containing the object</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            _clipRect.X = Int32.Parse(reader.GetAttribute("x"));
            _clipRect.Y = Int32.Parse(reader.GetAttribute("y"));
            _clipRect.Width = Int32.Parse(reader.GetAttribute("width"));
            _clipRect.Height = Int32.Parse(reader.GetAttribute("height"));
            _srcFile = reader.GetAttribute("file");
            Boolean isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Dump object to an XML-stream
        /// </summary>
        /// <param name="writer">The XML-stream to which the object will be dumped</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("x", _clipRect.X.ToString());
            writer.WriteAttributeString("y", _clipRect.Y.ToString());
            writer.WriteAttributeString("width", _clipRect.Width.ToString());
            writer.WriteAttributeString("height", _clipRect.Height.ToString());
            writer.WriteAttributeString("file", _srcFile.ToString());
        }
    }

    /// <summary>
    /// All animations used in displaying a specific tile
    /// </summary>
    public class TileObject : IXmlSerializable
    {

        #region Private Fields

        private int _width = 0;
        private int _height = 0;
        private List<List<VisibleObject>> _animations = null;
        private int _currentPhase = 0;
        private int _currentAnimation = 0;
        private bool _loop = true;
        private ContentManager _content = null;
        #endregion

        #region Public Fields

        /// <summary>
        /// true if the current animation loops (i.e. repeats endlessly)
        /// </summary>
        public bool loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
            }
        }
        /// <summary>
        /// True if we can return a valid bitmap
        /// </summary>
        public bool isValid
        {
            get
            {
                if ((_currentAnimation < _animations.Count) && (_currentPhase < _animations[_currentAnimation].Count))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Current animation style (if multiple styles are used, e.g. walking, talking, fighting)
        /// </summary>
        public int currentAnimation
        {
            get
            {
                return _currentAnimation;
            }
            set
            {
                if (_currentAnimation != value)
                {
                    _currentAnimation = value;
                    _currentPhase = 0;
                }
            }
        }

        /// <summary>
        /// The file containing the current animation phase
        /// </summary>
        public string animationFile
        {
            get
            {
                if ((_currentAnimation < _animations.Count) && (_currentPhase < _animations[_currentAnimation].Count))
                {
                    return _animations[_currentAnimation][_currentPhase].src;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if ((_currentAnimation < _animations.Count) && (_currentPhase < _animations[_currentAnimation].Count))
                {
                    _animations[_currentAnimation][_currentPhase].src = value;

                };
            }
        }

        /// <summary>
        /// A bitmap containing the current animation phase
        /// </summary>
        public Texture2D animationTexture
        {
            get
            {
                if ((_currentAnimation < _animations.Count) && (_currentPhase < _animations[_currentAnimation].Count))
                {

                    return _animations[_currentAnimation][_currentPhase].texture;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The rectangle used to cut out the current animation phase
        /// </summary>
        public Rectangle animationRect
        {
            get
            {
                return _animations[_currentAnimation][_currentPhase].clipRect;
            }
            set
            {
                _animations[_currentAnimation][_currentPhase].clipRect = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loop animation to next frame
        /// </summary>
        public void NextAnimation()
        {

            if ((_currentAnimation + 1 > _animations.Count) || (_animations[_currentAnimation] == null) || (_currentPhase + 2 > _animations[_currentAnimation].Count))
            {
                if (_loop) _currentPhase = 0;
            }
            else
            {
                _currentPhase += 1;
            }
        }

        /// <summary>
        /// Restart current animation at first frame
        /// </summary>
        public void ResetAnimation()
        {
            _currentPhase = 0;
        }


        /// <summary>
        /// Add multiple animation phases from a single file
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="src">Source File</param>
        /// <param name="start">TOp left corner of first frame (width and height determined by animation object!)</param>
        /// <param name="cols">Number of columns used in the animation</param>
        /// <param name="rows">Number of rows used in the animation</param>
        /// <param name="order">Whether to read row by row (false, default) or column by column (true)</param>
        /// <returns>ID of Animation added to or -1 if invalid target was passed</returns>
        public int AddAnimation(string src, Vector2 start, int animation = -1, int cols = 1, int rows = 1, bool order = false)
        {
            if (animation == -1)
            {
                _animations.Add(new List<VisibleObject>());
                animation = _animations.Count - 1;
            }
            if (animation < _animations.Count)
            {
                if (order)
                {
                    for (int y = 0; y < rows; ++y)
                    {
                        for (int x = 0; x < cols; ++x)
                        {

                            _animations[animation].Add(new VisibleObject(_content, src, new Rectangle((int)start.X + x * _width,
                                (int)start.Y + y * _height, _width, _height)));
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < cols; ++x)
                    {
                        for (int y = 0; y < rows; ++y)
                        {

                            _animations[animation].Add(new VisibleObject(_content, src, new Rectangle((int)start.X + x * _width,
                                (int)start.Y + y * _height, _width, _height)));
                        }
                    }
                }
                return animation;
            }
            else
            {
                return -1;
            }

        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animations"></param>
        public TileObject(ContentManager content, int width = 128, int height = 192)
        {
            _content = content;
            _width = width;
            _height = height;
            _animations = new List<List<VisibleObject>>();
        }
        #endregion

        /// <summary>
        /// Useless function from the IXmlSerializable-Interface
        /// </summary>
        /// <returns>null</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Get XML-data from a file
        /// </summary>
        /// <param name="reader">The file from which data will be read</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer visibleSerializer = new XmlSerializer(typeof(VisibleObject));
            reader.MoveToContent();
            Boolean isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (isEmptyElement) return;

            _width = Int32.Parse(reader.ReadElementString("width"));
            _height = Int32.Parse(reader.ReadElementString("height"));
            _loop = Boolean.Parse(reader.ReadElementString("loop"));

            reader.ReadStartElement("animations");
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("phase");
                VisibleObject temp = (VisibleObject)visibleSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            reader.ReadEndElement();

        }

        /// <summary>
        /// Dump the whole group of animations to an XML-file
        /// </summary>
        /// <param name="writer">The file used to write the data</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("width", _width.ToString());
            writer.WriteElementString("height", _height.ToString());
            writer.WriteElementString("loop", _loop.ToString());
            foreach (List<VisibleObject> visibles in _animations)
            {
                writer.WriteStartElement("animations");
                foreach (VisibleObject visible in visibles)
                {
                    writer.WriteStartElement("phase");
                    visible.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

        }
    }
}
