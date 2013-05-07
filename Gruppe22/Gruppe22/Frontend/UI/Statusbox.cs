using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Gruppe22
{
    public class Statusbox : UIElement
    {
        #region Private Fields
        SpriteFont _font = null;
        Texture2D _background = null;
        List<string> _text = null;
        int _startPos = 0;
        int _numLines = 0;
        int _lineHeight = 0;
        Texture2D _arrows = null;
        int _lastTime = 0;
        Keys _lastKey = Keys.A;
        #endregion

        #region Public Methods

        /// <summary>
        /// Append a new line of text to the status box; word wrap if necessary
        /// </summary>
        /// <param name="text"></param>
        public void AddLine(string text)
        {
            string remains = "";
            text = text.Trim();
            if (text.IndexOf("\n") > -1)
            {
                AddLine(text.Substring(0, text.IndexOf("\n")));
                AddLine(text.Substring(text.IndexOf("\n") + 1));
            }
            else
            {
                while (_font.MeasureString(text).X > _displayRect.Width - 40)
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
                if (remains != "") _text.Add(remains);
                if (text != "") _text.Add(text);
            }
            _startPos = Math.Max(_text.Count - _numLines, 0);
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
            try
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null);
                _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.Black);
                //_startPos = 2;
                for (int count = _startPos; count < Math.Min(_numLines + _startPos, _text.Count); ++count)
                {
                    _spriteBatch.DrawString(_font, _text[count], new Vector2(_displayRect.Left + 20, _displayRect.Top +
                        (count - _startPos) * _lineHeight), Color.White);
                }
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Top + 5, 28, 28), new Rectangle(32, 0, 28, 28), Color.White);
                _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 35, _displayRect.Bottom - 35, 28, 28), new Rectangle(0, 0, 28, 28), Color.White);

                _spriteBatch.End();
                _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            }
            finally
            {
                rstate.Dispose();
            }

        }


        public override void MouseClick(int X, int Y, int _lastCheck)
        {
            if ((_lastKey != Keys.Zoom) || (_lastCheck > 90))
            {
                if ((new Rectangle(_displayRect.Right - 35, _displayRect.Top + 5, 28, 28).Contains(X, Y)) && (_startPos > 0))
                {
                    _startPos -= 1;
                }
                if ((new Rectangle(_displayRect.Right - 35, _displayRect.Bottom - 35, 28, 28).Contains(X, Y)) && (_startPos < _text.Count - 1))
                {
                    _startPos += 1;
                }
                _lastKey = Keys.Zoom;
            }
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
        public override void HandleKey(int _lastCheck = -1)
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            int temp = _startPos;

            foreach (Keys key in keys)
            {
                switch (key)
                {
                    case Keys.PageUp:
                        if ((_lastKey != Keys.PageUp) || (_lastCheck > 90))
                        {
                            temp = _startPos - 10;
                            _lastKey = Keys.PageUp;

                        }
                        break;
                    case Keys.PageDown:
                        if ((_lastKey != Keys.PageDown) || (_lastCheck > 90))
                        {
                            temp = _startPos + 10;
                            _lastKey = Keys.PageDown;

                        }
                        break;
                    case Keys.Home:
                        if ((_lastKey != Keys.Home) || (_lastCheck > 90))
                        {
                            temp = 0;
                            _lastKey = Keys.Home;

                        }
                        break;
                    case Keys.End:
                        if ((_lastKey != Keys.End) || (_lastCheck > 90))
                        {
                            temp = _text.Count - _numLines;
                            _lastKey = Keys.End;
                        }
                        break;
                    case Keys.Down:
                        if ((_lastKey != Keys.Down) || (_lastCheck > 90))
                        {
                            _lastKey = Keys.Down;
                            temp = _startPos + 1;
                            System.Diagnostics.Debug.WriteLine("D");
                        }
                        break;
                    case Keys.Up:
                        if ((_lastKey != Keys.Up) || (_lastCheck > 90))
                        {
                            _lastKey = Keys.Up;
                            temp = _startPos - 1;

                        }
                        break;
                    default:
                        _lastKey = Keys.A;
                        break;
                }
            }

            if (temp < 0) temp = 0;
            if (temp > _text.Count - 1) temp = _text.Count - 1;
            _startPos = temp;
        }
        #endregion

        public Statusbox(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
            _font = _content.Load<SpriteFont>("Font");
            _background = _content.Load<Texture2D>("Minimap");
            _lineHeight = (int)(_font.MeasureString("WgjITt").Y) + 1;
            _numLines = (int)(displayRect.Height / _lineHeight);
            _arrows = _content.Load<Texture2D>("Arrows");
            _text = new List<string>();
        }

    }
}
