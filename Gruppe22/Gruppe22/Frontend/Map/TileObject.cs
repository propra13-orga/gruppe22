using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    /// <summary>
    /// An animation phase
    /// </summary>
    public class VisibleObject
    {
        #region Private Fields
        private string _srcFile = "";
        private int _startX = 0;
        private int _startY = 0;
        #endregion

        #region Public Fields
        /// <summary>
        /// 
        /// </summary>
        public int X { get { return _startX; } set { _startX = value; } }
        /// <summary>
        /// 
        /// </summary>
        public int Y { get { return _startY; } set { _startY = value; } }
        /// <summary>
        /// 
        /// </summary>
        public string src { get { return _srcFile; } set { _srcFile = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public VisibleObject(string srcFile, int startX, int startY)
        {
            _startX = startX;
            _startY = startY;
            _srcFile = srcFile;
        }
        #endregion
    }

    /// <summary>
    /// All animations used in displaying a specific tile
    /// </summary>
    public class TileObject
    {
        
        #region Private Fields

        private int _width = 0;
        private int _height = 0;
        private List<VisibleObject> _animations = null;
        private int _currentPhase = 0;
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void Draw(SpriteBatch target)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            _currentPhase += 1;
            if ((_animations == null) || (_currentPhase > _animations.Count))
            {
                _currentPhase = 0;
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
        public TileObject(int width = 0, int height = 0, List<VisibleObject> animations = null)
        {
            _width = width;
            _height = height;
            _animations = animations;
        }
        #endregion
    }
}
