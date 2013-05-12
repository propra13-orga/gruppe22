﻿using System;
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
        private int _offsetX = 0;
        private int _offsetY = 0;
        private int _cropX = 0;
        private int _cropY = 0;

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

        public int offsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        public int offsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }

        public int cropX
        {
            get { return _cropX; }
            set { _cropX = value; }
        }

        public int cropY
        {
            get { return _cropY; }
            set { _cropY = value; }
        }
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
            if (_srcFile != "")
            {
                _texture = _content.Load<Texture2D>(_srcFile);
            }
        }

        public VisibleObject()
        {

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

            Boolean isEmptyElement = reader.IsEmptyElement;



            reader.MoveToContent();


            _clipRect.X = Int32.Parse(reader.GetAttribute("x"));
            _clipRect.Y = Int32.Parse(reader.GetAttribute("y"));
            _clipRect.Width = Int32.Parse(reader.GetAttribute("width"));
            _clipRect.Height = Int32.Parse(reader.GetAttribute("height"));
            _srcFile = reader.GetAttribute("file");

            if (_srcFile != "")
            {
                _texture = _content.Load<Texture2D>(_srcFile);
            }


            if (isEmptyElement)
            {
                return;
            }

            if (!isEmptyElement)
                reader.ReadEndElement();
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

        private List<VisibleObject> _animations = null;
        private int _currentPhase = 0;
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

        public int offsetX
        {
            get
            {
                if (_currentPhase < _animations.Count)
                {
                    return _animations[_currentPhase].offsetX;
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                if (_currentPhase < _animations.Count)
                {
                    _animations[_currentPhase].offsetX = value;
                }
            }
        }

        public int offsetY
        {
            get
            {
                if (_currentPhase < _animations.Count)
                {
                    return _animations[_currentPhase].offsetY;
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                if (_currentPhase < _animations.Count)
                {
                    _animations[_currentPhase].offsetY = value;
                }
            }
        }

        public int cropX
        {
            get
            {
                if (_currentPhase < _animations.Count)
                {
                    return _animations[_currentPhase].cropX;
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                if (_currentPhase < _animations.Count)
                {
                    _animations[_currentPhase].cropX = value;
                }
            }
        }

        public int cropY
        {
            get
            {
                if (_currentPhase < _animations.Count)
                {
                    return _animations[_currentPhase].cropY;
                }
                else
                {
                    return 0;
                }

            }
            set
            {
                if (_currentPhase < _animations.Count)
                {
                    _animations[_currentPhase].cropY = value;
                }
            }
        }

        /// <summary>
        /// True if we can return a valid bitmap
        /// </summary>
        public bool isValid
        {
            get
            {
                if (_currentPhase < _animations.Count)
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
        /// The file containing the current animation phase
        /// </summary>
        public string animationFile
        {
            get
            {
                if ((_currentPhase < _animations.Count))
                {
                    return _animations[_currentPhase].src;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if ((_currentPhase < _animations.Count))
                {
                    _animations[_currentPhase].src = value;

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
                if ((_currentPhase < _animations.Count))
                {

                    return _animations[_currentPhase].texture;
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
                return _animations[_currentPhase].clipRect;
            }
            set
            {
                _animations[_currentPhase].clipRect = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loop animation to next frame
        /// </summary>
        public void NextAnimation()
        {

            if  (_currentPhase + 2 > _animations.Count)
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
        public void AddAnimation(string src, Vector2 start, int cols = 1, int rows = 1, bool order = false)
        {
            if (order)
            {
                for (int y = 0; y < rows; ++y)
                {
                    for (int x = 0; x < cols; ++x)
                    {

                        _animations.Add(new VisibleObject(_content, src, new Rectangle((int)start.X + x * _width,
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

                        _animations.Add(new VisibleObject(_content, src, new Rectangle((int)start.X + x * _width,
                            (int)start.Y + y * _height, _width, _height)));
                    }
                }

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
            _animations = new List<VisibleObject>();
        }

        public TileObject()
        {

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
            reader.MoveToContent();
            Boolean isEmptyElement = reader.IsEmptyElement;


            _width = Int32.Parse(reader.GetAttribute("width"));
            _height = Int32.Parse(reader.GetAttribute("height"));
            _loop = Boolean.Parse(reader.GetAttribute("loop"));

            reader.Read();

            if (isEmptyElement) return;

            reader.ReadStartElement("animations");

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                VisibleObject temp = new VisibleObject(_content, "", Rectangle.Empty);
                temp.ReadXml(reader);
                _animations.Add(temp);
                reader.Read();
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
            writer.WriteAttributeString("width", _width.ToString());
            writer.WriteAttributeString("height", _height.ToString());
            writer.WriteAttributeString("loop", _loop.ToString());

            writer.WriteStartElement("animations");
            for (int i = 0; i < _animations.Count; ++i)
            {
                writer.WriteStartElement("phase");
                writer.WriteAttributeString("id", i.ToString());
                _animations[i].WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();


        }
    }
}
