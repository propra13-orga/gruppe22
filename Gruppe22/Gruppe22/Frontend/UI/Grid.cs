using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    /// <summary>
    /// 
    /// </summary>
    public class Grid : UIElement
    {
        #region Private Fields
        /// <summary>
        /// 
        /// </summary>
        public List<VisibleObject> _icons;

        /// <summary>
        /// 
        /// </summary>
        private int _rows = 0;

        /// <summary>
        /// 
        /// </summary>
        private int _cols = 0;

        /// <summary>
        /// 
        /// </summary>
        private int _width = 32;

        /// <summary>
        /// 
        /// </summary>
        private int _height = 32;

        /// <summary>
        /// 
        /// </summary>
        private int _selected = -1;

        /// <summary>
        /// 
        /// </summary>
        private int _totalPages = -1;

        /// <summary>
        /// 
        /// </summary>
        private int _page = -1;
        #endregion

        #region Public Fields
        /// <summary>
        /// 
        /// </summary>
        public int width
        {
            set
            {
                _width = value;
                _cols = (int)(_displayRect.Width / _width);
            }
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int height
        {
            set
            {
                _height = value;
                _rows = (int)(_displayRect.Height / _height);
            }
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int rows
        {
            set
            {
                _rows = value;
                _height = (int)(_displayRect.Height / _rows);
            }
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int cols
        {
            set
            {
                _cols = value;
                _width = (int)(_displayRect.Width / _cols);
            }
            get
            {
                return _height;
            }
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void _CalcPages()
        {
            _totalPages = _icons.Count / (_rows * _cols);
            if (_icons.Count % (_rows * _cols) > 0)
            {
                _totalPages += 1;
            }
        }
        #endregion
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Button"></param>
        public override void Click(int Button)
        {
            base.Click(Button);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public Grid(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
        }
        #endregion
    }
}
