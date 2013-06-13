using System;
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
    public class Abilities : Grid
    {
        private Actor _actor;
        public Actor actor
        {
            get
            {
                return _actor;
            }
            set
            {
                _actor = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
                _spriteBatch.Begin();
                int icon = _page * _cols * _rows;

                for (int y = 0; y < _rows; ++y)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left, _displayRect.Top + y * (_height + 3), _displayRect.Width, _height + 2), new Rectangle(39, 6, 1, 1), Color.White);
                    if (icon != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _displayRect.Width - 2, _height), new Rectangle(39, 6, 1, 1), Color.Black);

                    if ((icon < _icons.Count) && (_icons[icon] != null))
                    {
                        _spriteBatch.Draw(_icons[icon].icon.texture, new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), _icons[icon].icon.clipRect, Color.White);
                        if (_icons[icon].check)
                        {
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Right - 16, _displayRect.Top + y * (_height + 3) + 2, 8, 8), new Rectangle(48, 16, 16, 16), Color.White);
                        }
                        if (icon == _selected)
                        {
                            int textwidth = (int)_font.MeasureString(_icons[icon].tooltip).X + 1;
                            int textheight = (int)_font.MeasureString(_icons[icon].tooltip).Y + 1;
                            DisplayToolTip(icon, 0, y);
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



        #region Constructor
        public Abilities(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion
    }
}
