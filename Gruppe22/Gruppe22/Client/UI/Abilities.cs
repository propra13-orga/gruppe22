using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    public class Abilities : Grid
    {
        private Backend.Actor _actor;
        public Backend.Actor actor
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
                y -= _displayRect.Top;
                y = y / (_height + 3);
                if (y >= 0)
                    result = y + (_page * _rows);
            }
            if ((x > _displayRect.Right - 30) || (result > _actor.abilities.Count)) return -1;

            return result;
        }



        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public void DisplayToolTip(int icon, int y)
        {
            int x = 0;
            string text = _actor.abilities[icon].name + "\n Strength:" + _actor.abilities[icon].intensity + "\n Cooldown:" + _actor.abilities[icon].cooldown + "\n Cost: " + _actor.abilities[icon].cost + "MP" + ((_actor.abilities[icon].duration > 1) ? ("\n Duration:" + _actor.abilities[icon].duration) : "");

            int textwidth = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).X + 1;
            int textheight = (int)_font.MeasureString(text.Replace("<red>", "").Replace("<green>", "")).Y + 1;
            int lineHeight = (int)_font.MeasureString("Wgj").Y + 1;
            Color color = Color.White;
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + x * (_width + 3)
    - textwidth - 2

    , _displayRect.Top + y * (_height + 3)

    - textheight - 2
    , textwidth + 5, textheight + 5), new Rectangle(39, 6, 1, 1), new Color(Color.Black, 0.9f));
            int line = 0;
            while (text.IndexOf("\n") > -1)
            {
                color = Color.White;
                text = text.TrimStart();
                if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
                if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }
                text = text.TrimStart();
                string next = text.Substring(text.IndexOf("\n") + 1);
                text = text.Substring(0, text.IndexOf("\n"));
                text.TrimEnd();
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + x * (_width + 3)
                      - textwidth

                      , _displayRect.Top + y * (_height + 3)

                      - textheight + line), Color.Black);
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + x * (_width + 3)
                    - textwidth + 1

                    , _displayRect.Top + y * (_height + 3)

                    - textheight + 1 + line), color);
                text = next;

                line += lineHeight;
            }
            if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
            if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }

            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + x * (_width + 3)
                - textwidth + 1

                , _displayRect.Top + y * (_height + 3)

                - textheight + 1 + line), color);

        }

        public override bool OnMouseDown(int button)
        {
            if (_visible)
            {
                int x = Mouse.GetState().X;
                int y = Mouse.GetState().Y;
                _totalPages = (int)Math.Ceiling((float)_actor.abilities.Count / (float)_rows);

                if (new Rectangle(_displayRect.Right - 70, _displayRect.Top, 90, _displayRect.Height / 2).Contains(new Point(x, y)))
                {
                    if (_page > 0)
                        _page -= 1;
                    return true;
                }

                if (new Rectangle(_displayRect.Right - 70, _displayRect.Bottom - _displayRect.Height / 2, 90, _displayRect.Height / 2 + 50).Contains(new Point(x, y)))
                {
                    if (_page < _totalPages)
                        _page += 1;
                    return true;
                }

                int selected = Pos2Tile(x, y);
                if (selected > -1)
                {
                    _parent.HandleEvent(false, Backend.Events.AddDragItem, new GridElement(selected + 1,
                        _actor.abilities[selected].name + "\n Strength:" + _actor.abilities[selected].intensity + "\n Cooldown:" + _actor.abilities[selected].cooldown + "\n Cost: " + _actor.abilities[selected].cost + "MP" + ((_actor.abilities[selected].duration > 1) ? ("\n Duration:" + _actor.abilities[selected].duration) : ""),
                        _actor.abilities[selected].icon, _content, false, true, 0));
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _totalPages = (int)Math.Ceiling((float)_actor.abilities.Count / (float)_rows);

            if (_visible)
            {
                int _selected = Pos2Tile(Mouse.GetState().X, Mouse.GetState().Y);
                _spriteBatch.Begin();
                int icon = _page * _rows;

                for (int y = 0; y < _rows; ++y)
                {
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left, _displayRect.Top + y * (_height + 3), _displayRect.Width - 38, _height + 2), new Rectangle(39, 6, 1, 1), Color.White);
                    if (icon != _selected)
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _displayRect.Width - 40, _height), new Rectangle(39, 6, 1, 1), Color.Black);



                    if ((icon < _actor.abilities.Count) && (_actor.abilities[icon].icon != null))
                    {
                        _spriteBatch.Draw(TextureFromData.Convert(_actor.abilities[icon].icon, _content), new Rectangle(_displayRect.Left + 1, _displayRect.Top + y * (_height + 3) + 1, _height, _height), _actor.abilities[icon].icon.rect, Color.White);
                        _spriteBatch.DrawString(_font, _actor.abilities[icon].name + " (" + _actor.abilities[icon].cost + "MP," + _actor.abilities[icon].cooldown + " Cooldown) - Strength: " + _actor.abilities[icon].intensity + " Duration: " + _actor.abilities[icon].duration, new Vector2(_displayRect.Left + _height + 5, _displayRect.Top + y * (_height + 3) + 1), Color.Black);
                        _spriteBatch.DrawString(_font, _actor.abilities[icon].name + " (" + _actor.abilities[icon].cost + "MP," + _actor.abilities[icon].cooldown + " Cooldown) - Strength: " + _actor.abilities[icon].intensity + " Duration: " + _actor.abilities[icon].duration, new Vector2(_displayRect.Left + _height + 6, _displayRect.Top + y * (_height + 3) + 2), Color.White);

                        if (icon == _selected)
                        {
                            DisplayToolTip(icon, y);
                        }
                    }
                    ++icon;
                }

                if (_totalPages > 1)
                {
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Top + 5, 22, 22), new Rectangle(32, 0, 28, 28), Color.White);
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Bottom - 35, 22, 22), new Rectangle(0, 0, 28, 28), Color.White);
                }
                _spriteBatch.End();

            }

        }



        #region Constructor
        public Abilities(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Actor actor = null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion
    }
}
