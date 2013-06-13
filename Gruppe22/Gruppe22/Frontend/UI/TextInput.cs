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
    public class TextInput : UIElement
    {
        protected string _label = "";
        protected string _text = "";
        protected string _tooltip = "";
        protected SpriteFont _font = null;
        protected Texture2D _background = null;
        protected int _textWidth = 0;
        private int _cursor = 0;
        private bool _overwrite = false;
        private int _counter;
        protected bool _canEdit = false;
        private int _color = 0;

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }


        public override bool OnKeyDown(Microsoft.Xna.Framework.Input.Keys k)
        {
            if ((_visible) && (_focus))
            {
                switch (k)
                {
                    case Keys.Back:
                        if ((_text.Length + 1 > _cursor) && (_cursor > 0))
                            _text = _text.Remove(_cursor - 1, 1);
                        _cursor -= 1;
                        return true;
                        break;
                    case Keys.Delete:
                        if (_text.Length+1 > _cursor + 1)
                            _text = _text.Remove(_cursor, 1);
                        return true;
                        break;
                    case Keys.Space:
                        _text = _text.Insert(_cursor, " ");
                        return true;
                        break;
                    case Keys.Insert:
                        _overwrite = !_overwrite;
                        return true;
                        break;
                    case Keys.Enter:
                        base.OnKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab);
                        return true;
                        break;
                    case Keys.Tab:
                        base.OnKeyDown(k);
                        break;
                    case Keys.Left:
                        if (_cursor > 0)
                            _cursor -= 1;
                        return true;
                        break;
                    case Keys.Right:
                        if (_cursor < _text.Length)
                            _cursor += 1;
                        return true;
                        break;
                    case Keys.Home:

                        _cursor = 0;
                        return true;
                        break;
                    case Keys.End:
                        _cursor = _text.Length - 1;
                        return true;
                        break;
                    default:
                        if (k.ToString().Length == 1)
                        {
                            if ((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift)))
                                _text = _text.Insert(_cursor, k.ToString().ToUpper());
                            else
                                _text = _text.Insert(_cursor, k.ToString().ToLower());
                            _cursor += 1;
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public override bool canFocus
        {
            get
            {
                return _visible && _canEdit;
            }
        }


        public bool canEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                _canEdit = value;
            }
        }
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {

            base.HandleEvent(DownStream, eventID, data);
        }

        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        protected void _DisplayToolTip()
        {

            int textwidth = (int)_font.MeasureString(_tooltip.Replace("<red>", "").Replace("<green>", "")).X + 1;
            int textheight = (int)_font.MeasureString(_tooltip.Replace("<red>", "").Replace("<green>", "")).Y + 1;
            int lineHeight = (int)_font.MeasureString("Wgj").Y + 1;
            Color color = Color.White;
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Left
    , _displayRect.Top - textheight - 9
    , textwidth + 6, textheight + 6), new Rectangle(39, 6, 1, 1), new Color(Color.Black, 0.9f));
            int line = 0;
            string text = _tooltip;
            while (text.IndexOf("\n") > -1)
            {
                color = Color.White;
                text = _tooltip.TrimStart();
                if (_tooltip.StartsWith("<red>")) { color = Color.Red; text = _tooltip.Substring(5); }
                if (_tooltip.StartsWith("<green>")) { color = Color.Green; text = _tooltip.Substring(7); }
                text = _tooltip.TrimStart();
                string next = _tooltip.Substring(_tooltip.IndexOf("\n") + 1);
                text = _tooltip.Substring(0, _tooltip.IndexOf("\n"));
                _tooltip.TrimEnd();
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 4
    , _displayRect.Top - textheight - 3 + 4 + line), Color.Black);
                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 3
    , _displayRect.Top - textheight - 3 + 3 + line), color);
                text = next;

                line += lineHeight;
            }
            if (_tooltip.StartsWith("<red>")) { color = Color.Red; text = _tooltip.Substring(5); }
            if (_tooltip.StartsWith("<green>")) { color = Color.Green; text = _tooltip.Substring(7); }

            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 4
, _displayRect.Top - textheight - 3 + 4 + line), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + 3
, _displayRect.Top - textheight - 3 + 3 + line), color);

        }
        public override void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.X, _displayRect.Y + 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 6, _displayRect.Y, _textWidth + 7, _displayRect.Height), new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 5, _displayRect.Y + 1, _textWidth + 5, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _focus ? Color.DarkBlue : Color.Black);
                _spriteBatch.DrawString(_font, _text, new Vector2(_displayRect.X + _displayRect.Width - _textWidth, _displayRect.Y + 2), Color.White);
                if (_focus && (_cursor > -1) && (_cursor < _text.Length + 1)
                    )
                {
                    _counter += gameTime.ElapsedGameTime.Milliseconds;
                    if (_counter > 500)
                    {
                        _counter -= 500;
                        if (_color == 0) _color = 1; else _color = 0;

                    }
                    _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + _displayRect.Width - _textWidth - 1 + (int)_font.MeasureString((_cursor < text.Length) ? _text.Substring(0, _cursor) : text).X, _displayRect.Y + 2, 2, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), (_color == 1) ? Color.White : Color.Transparent);


                }
                Point pos = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                if (_displayRect.Contains(pos))
                {
                    _DisplayToolTip();
                }
                _spriteBatch.End();

                base.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public TextInput(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string label, string text, string toolTip, int inputWidth, bool canEdit)
            : base(parent, spriteBatch, content, displayRect)
        {
            _displayRect = displayRect;

            _label = label;
            _text = text;
            _tooltip = toolTip;
            _canEdit = canEdit;
            _font = _content.Load<SpriteFont>("SmallFont");
            _background = _content.Load<Texture2D>("Minimap");
            _textWidth = ((int)(_font.MeasureString("W").X) + 1) * inputWidth;

        }
    }
}
