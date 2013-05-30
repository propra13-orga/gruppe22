﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{

    public class GridElement
    {
        private int _id = 0;
        private string _tooltip = "";
        private VisibleObject _icon = null;
        private bool _checked = false;
        public VisibleObject icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        public string tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }
        public int id
        {
            get { return _id; }
        }
        public bool isChecked
        {
            get { return _checked; }
            set { _checked = value; }
        }

        public GridElement(int id, string tooltip, VisibleObject icon, bool isChecked)
        {
            _id = id;
            _tooltip = tooltip;
            _icon = icon;
            _checked = isChecked;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Grid : UIElement
    {
        #region Private Fields
        private SpriteFont _font = null;

        /// <summary>
        /// 
        /// </summary>
        protected List<GridElement> _icons;

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
        private int _totalPages = 1;

        /// <summary>
        /// 
        /// </summary>
        private int _page = 0;

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

        public int Pos2Tile(int x, int y)
        {
            int result = -1;
            if (_displayRect.Contains(x, y))
            {
                x -= _displayRect.Left;
                y -= _displayRect.Top;
                x = x / (_width + 3);
                y = y / (_height + 3);
                result = y * _cols + x + (_page * _cols * _rows);
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
            _spriteBatch.Begin();
            int icon = _page * _cols * _rows;

            for (int y = 0; y < _rows; ++y)
            {
                for (int x = 0; x < _cols; ++x)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3), _displayRect.Top + y * (_height + 3), _width + 2, _height + 2), new Rectangle(39, 6, 1, 1), Color.White);
                    if (icon != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3) + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), new Rectangle(39, 6, 1, 1), Color.Black);

                    if ((icon < _icons.Count) && (_icons[icon] != null))
                    {
                        _spriteBatch.Draw(_icons[icon].icon.texture, new Rectangle(_displayRect.Left + x * (_width + 3) + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), _icons[icon].icon.clipRect, Color.White);
                        if (_icons[icon].isChecked)
                        {
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (x + 1) * (_width + 3) - 16, _displayRect.Top + y * (_height + 3) + 2, 16, 16), new Rectangle(48, 16, 16, 16), Color.White);
                        }
                        if (icon == _selected)
                        {
                            int textwidth = (int)_font.MeasureString(_icons[icon].tooltip).X + 1;
                            int textheight = (int)_font.MeasureString(_icons[icon].tooltip).Y + 1;

                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3)
                                - textwidth - 2

                                , _displayRect.Top + y * (_height + 3)

                                - textheight - 2
                                , textwidth + 5, textheight + 5), new Rectangle(39, 6, 1, 1), new Color(Color.DarkRed, 0.8f));
                            _spriteBatch.DrawString(_font, _icons[icon].tooltip, new Vector2(_displayRect.Left + x * (_width + 3)
                       - textwidth

                       , _displayRect.Top + y * (_height + 3)

                       - textheight), Color.Black);
                            _spriteBatch.DrawString(_font, _icons[icon].tooltip, new Vector2(_displayRect.Left + x * (_width + 3)
                                - textwidth +1

                                , _displayRect.Top + y * (_height + 3)

                                - textheight +1), Color.White);
                        }
                    }
                    ++icon;
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

        public override void OnMouseDown(int button)
        {

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
            _cols = (int)((_displayRect.Width - 35) / (_width + 3));
            _rows = (int)((_displayRect.Height) / (_height + 3));
            _icons = new List<GridElement>();
            _font = _content.Load<SpriteFont>("SmallFont");

        }
        #endregion
    }
}
