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
    public class AbilityChoice : Grid
    {
        int _width = 120;
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


        public override int Pos2Tile(int x, int y)
        {
            int result = -1;
            if (_displayRect.Contains(x, y))
            {
                x -= _displayRect.Top;
                x = x / (_width + 3);
                result = x;
            }
            return result;
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
                _spriteBatch.DrawString(_font, "Select an ability:", new Vector2(_displayRect.Left, _displayRect.Top), Color.White);
                for (int x = 0; x < 4; ++x)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x), _displayRect.Top + 20, _width - 2, _displayRect.Height - 20), new Rectangle(39, 6, 1, 1), Color.White);
                    if (x != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + (_width * x) + 1, _displayRect.Top + 21, _width - 4, _displayRect.Height - 22), new Rectangle(39, 6, 1, 1), Color.Black);

                    /*

                    if ((x < _actor.abilities.Count) && (_actor.abilities[icon].icon != null))
                    {
                        _spriteBatch.Draw(_actor.abilities[icon].icon.texture, new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _width, _height), _actor.abilities[icon].icon.clipRect, Color.White);
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
                     */
                }
            }
            _spriteBatch.End();
        }



        #region Constructor
        public AbilityChoice(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion

    }
}
