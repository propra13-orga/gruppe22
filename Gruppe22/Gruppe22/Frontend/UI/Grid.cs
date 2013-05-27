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
        private int _rows = 2;

        /// <summary>
        /// 
        /// </summary>
        private int _cols = 3;

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
        private int _totalPages = 3;

        /// <summary>
        /// 
        /// </summary>
        private int _page = -1;

        private Texture2D _arrows = null;
        private Texture2D _background = null;
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
            _spriteBatch.Begin();
            for (int y = 0; y < _rows; ++y)
            {
                for (int x = 0; x < _cols; ++x)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3), _displayRect.Top + y * (_height + 3), _width + 2, _height + 2), new Rectangle(39, 6, 1, 1), Color.White);
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3) + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), new Rectangle(39, 6, 1, 1), Color.Black);
                }
            }
            if (_totalPages > 1)
            {
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Top + 5, 28, 28), new Rectangle(32, 0, 28, 28), Color.White);
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Bottom - 35, 28, 28), new Rectangle(0, 0, 28, 28), Color.White);
            }

            _spriteBatch.End();
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
            _background = _content.Load<Texture2D>("Minimap");
            _arrows = _content.Load<Texture2D>("Arrows");
            _cols = (int)((_displayRect.Width - 35) / (_width+3));
            _rows = (int)((_displayRect.Height) / (_height+3));
        }
        #endregion
    }
}
