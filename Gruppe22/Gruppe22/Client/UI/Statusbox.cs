using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Gruppe22.Client
{
    public class Statusbox : TextOutput
    {
        #region Private Fields
        private SpriteFont _font = null;
        private Texture2D _background = null;
        private List<string> _text = null;
        private int _startPos = 0;
        private int _numLines = 0;
        private int _lineHeight = 0;
        private Texture2D _arrows = null;
        private bool _hasBorder = true;
        private bool _center = false;
        private List<Color> _color = null;
        #endregion

        #region Public Methods

        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public override void AddLine(string text, object color = null)
        {
            if (color == null) { color = new Color(); color = Color.White; }
            string remains = "";
            if (text.StartsWith("<red>")) { color = Color.Red; text = text.Substring(5); }
            if (text.StartsWith("<green>")) { color = Color.Green; text = text.Substring(7); }
            text = text.Trim();
            if (text.IndexOf("\n") > -1)
            {
                AddLine(text.Substring(0, text.IndexOf("\n")), color);
                AddLine(text.Substring(text.IndexOf("\n") + 1), color);
            }
            else
            {
                while (_font.MeasureString(text).X > _displayRect.Width - 55)
                {
                    if (text.LastIndexOf(' ') > 0)
                    {
                        remains = text.Substring(text.LastIndexOf(' ')) + remains;
                        text = text.Remove(text.LastIndexOf(' '));
                    }
                    else
                    {
                        remains = text.Substring(text.Length - 2) + remains;
                        text = text.Remove(text.Length - 2);
                    }
                }
                if (text != "")
                {
                    _text.Add(text);
                    _color.Add((Color)color);
                }

                if (remains != "")
                {
                    _text.Add(remains);
                    _color.Add((Color)color);
                }
            }
            _startPos = Math.Max(_text.Count - _numLines, 0);
        }

        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;
            RasterizerState rstate = new RasterizerState();
            rstate.ScissorTestEnable = true;

            _spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        rstate,
                        null);
            if (_hasBorder || _focus)
            {
                _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 1, _displayRect.Width - 3, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), Color.Black);
            }
            //_startPos = 2;
            for (int count = _startPos; count < Math.Min(_numLines + _startPos, _text.Count); ++count)
            {
                int centerX = _displayRect.Left + 5;
                if (_center)
                {
                    centerX = 5 + _displayRect.X + ((int)(_displayRect.Width - 55 - _font.MeasureString(_text[count]).X) / 2);
                }

                _spriteBatch.DrawString(_font, _text[count], new Vector2(centerX, _displayRect.Top + 5 +
                    (count - _startPos) * _lineHeight), _color[count]);

            }
            if (_numLines < _text.Count)
            {
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 22, _displayRect.Top + 2, 20, 20), new Rectangle(32, 0, 28, 28), Color.White);
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 22, _displayRect.Bottom - 22, 20, 20), new Rectangle(0, 0, 28, 28), Color.White);
            }
            _spriteBatch.End();
            _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;

            rstate.Dispose();


        }


        public override bool OnMouseDown(int button)
        {
            if ((new Rectangle(_displayRect.Right - 35, _displayRect.Top + 5, 28, 28).Contains(Mouse.GetState().X, Mouse.GetState().Y)) && (_startPos > 0))
            {
                _startPos -= 1;
                return true;
            }
            if ((new Rectangle(_displayRect.Right - 35, _displayRect.Bottom - 35, 28, 28).Contains(Mouse.GetState().X, Mouse.GetState().Y)) && (_startPos < _text.Count - 1))
            {
                _startPos += 1;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="difference"></param>
        public override void MoveContent(Vector2 difference, int _lastCheck = 0)
        {
            int temp = _startPos - (int)(Math.Abs(difference.Y) / difference.Y);

            if ((temp > 0) && (temp < _text.Count))
            {
                _startPos = temp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Difference"></param>
        public override void ScrollWheel(int Difference)
        {
            if ((_startPos + Difference > 0) && (_startPos + Difference < _text.Count))
            {
                _startPos += Difference;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool OnKeyDown(Keys k)
        {
            int temp = _startPos;
            if (_focus)
            {
                switch (k)
                {
                    case Keys.PageUp:
                        temp = _startPos - 10;
                        break;
                    case Keys.PageDown:
                        temp = _startPos + 10;
                        break;
                    case Keys.Home:
                        temp = 0;
                        break;
                    case Keys.End:
                        temp = _text.Count - _numLines;
                        break;
                    case Keys.Down:
                        temp = _startPos + 1;
                        break;
                    case Keys.Up:
                        temp = _startPos - 1;
                        break;
                }
                if (temp < 0) temp = 0;
                if (temp > _text.Count - 1) temp = _text.Count - 1;
                _startPos = temp;
                if (temp != _startPos) return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Creates a scrollable area to output text
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        /// <param name="hasBorder"></param>
        /// <param name="center"></param>
        public Statusbox(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, bool hasBorder = true, bool center = false)
            : base(parent, spriteBatch, content, displayRect)
        {
            _font = _content.Load<SpriteFont>("SmallFont");
            _background = _content.Load<Texture2D>("Minimap");
            _lineHeight = (int)(_font.MeasureString("WgjITt").Y) + 1;
            _numLines = (int)((displayRect.Height - 10) / _lineHeight);
            _arrows = _content.Load<Texture2D>("Arrows");
            _text = new List<string>();
            _color = new List<Color>();
            _hasBorder = hasBorder;
            _center = center;
        }

    }
}
