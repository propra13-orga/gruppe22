using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{
    /// <summary>
    /// Appearance of the progress indicator
    /// </summary>
    public enum ProgressStyle
    {
        /// <summary>
        /// Normal fluid progressbar
        /// </summary>
        Default = 0,
        /// <summary>
        /// "Marquee-style" indefinite progressbar (animated)
        /// </summary>
        Undefined = 1,
        /// <summary>
        /// Precise progress indicator (including text "x/total")
        /// </summary>
        Precise = 2,
        /// <summary>
        /// Show progress as blocks
        /// </summary>
        Block = 3,
        /// <summary>
        /// Vertically aligned progress bar
        /// </summary>
        Vertical = 4
    }

    /// <summary>
    /// A progress bar
    /// </summary>
    public class ProgressBar : UIElement, Backend.IHandleEvent
    {
        #region Private Fields
        /// <summary>
        /// The current value displayed by the progressbar
        /// </summary>
        private int _value = 0;

        /// <summary>
        /// The maximum amount available to the progressbar
        /// </summary>
        private int _total = 0;

        /// <summary>
        /// The style of the progressbar
        /// </summary>
        private ProgressStyle _style = ProgressStyle.Default;

        /// <summary>
        /// The color of the progressbar
        /// </summary>
        private Color _color = Color.Green;

        /// <summary>
        /// An object used to draw lines and rectangles
        /// </summary>
        private Texture2D _background = null;

        /// <summary>
        /// The font used to display the progress as a number
        /// </summary>
        private SpriteFont _font = null;

        /// <summary>
        /// Intermediate value used when animating
        /// </summary>
        int _pixels = 0;


        /// <summary>
        /// Number of pixels per unit
        /// </summary>
        int _pixelsPerUnit = 0;

        /// <summary>
        /// Height of text used to display progress 
        /// </summary>
        int _fontHeight = 0;
        #endregion

        #region Public Fields

        /// <summary>
        /// Current value of the progress bar (minimum 0, maximum set by total)
        /// </summary>
        public int value
        {
            set
            {
                if ((value <= _total) && (value > -1))
                {
                    if (_style != ProgressStyle.Undefined)
                    {
                        _pixels = -(_pixelsPerUnit * (value - _value));
                    }
                    _value = value;
                }
            }
            get { return _value; }
        }

        /// <summary>
        /// Total amount available on the progress bar (resets value)
        /// </summary>
        public int total
        {
            set
            {
                _total = value;
                if (_value > _total)
                    _value = total;
                if (_total > 0)
                    if (_style != ProgressStyle.Vertical)
                    {
                        _pixelsPerUnit = ((_displayRect.Width - 8) / _total);
                    }
                    else
                    {

                        _pixelsPerUnit = ((_displayRect.Height - 8) / _total);
                    }
                _pixels = 0;
            }
            get { return _total; }

        }

        /// <summary>
        /// Progress bars animate and display even if program is paused
        /// </summary>
        public override bool ignorePause
        {
            get
            {
                return true;
            }
        }

        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        #endregion

        #region Public Methods

        public override bool IsHit(int x, int y)
        {
            return false;
        }
        /// <summary>
        /// Display the progressbar using appropriate position,style and color
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (_total > 0)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, _displayRect.Width - 2, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), Color.Black);
                switch (_style)
                {
                    case ProgressStyle.Undefined:
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + Math.Abs(_pixels), _displayRect.Y + 1, 20, _displayRect.Height - 2), new Rectangle(39, 6, 2, 2), _color);
                        break;
                    case ProgressStyle.Block:
                        for (int i = 0; ((i <= _total / 24) && (_value > i * 24)); ++i)
                        {
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X +
                            i * 24 + 4
                            , _displayRect.Y + 4, 20, _displayRect.Height - 8), new Rectangle(39, 6, 1, 1), _color);
                        }
                        break;
                    case ProgressStyle.Default:
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, (_value * (_displayRect.Width - 2)) / total + _pixels, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _color);
                        break;
                    case ProgressStyle.Precise:
                        if (_value > 0)
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, (_value * (_displayRect.Width - 2)) / total + _pixels, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _color);

                        string state = _value.ToString() + "/" + _total.ToString();
                        int width = (int)_font.MeasureString(state).X;
                        _spriteBatch.DrawString(_font, state, new Vector2(_displayRect.X + (_displayRect.Width - width) / 2 - 2, _displayRect.Y + (_displayRect.Height - _fontHeight) / 2 + 2), Color.Black);
                        _spriteBatch.DrawString(_font, state, new Vector2(_displayRect.X + (_displayRect.Width - width) / 2, _displayRect.Y + (_displayRect.Height - _fontHeight) / 2), Color.White);
                        break;
                    case ProgressStyle.Vertical:
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1 + ((_total - _value) * (_displayRect.Height - 2)) / total + _pixels, _displayRect.Width - 2, (_value * (_displayRect.Height - 2)) / total + _pixels), new Rectangle(39, 6, 1, 1), _color);
                        break;
                }
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Move progress bar (if animated)
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (_style != ProgressStyle.Undefined) // Animation (fluidly move to next number)
            {
                if (_pixels < 0)
                {
                    _pixels += 1;
                }
                else
                    if (_pixels > 0)
                    {
                        _pixels -= 1;
                    }
            }
            else // Animation (unspecific value)
            {

                if (_pixels < _displayRect.Width - 24)
                {
                    _pixels += 2;
                }
                else
                {
                    _pixels = -_displayRect.Width + 24;
                }

            }
            base.Update(gameTime);
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Create a new progress bar
        /// </summary>
        /// <param name="parent">A parent object to pass events to</param>
        /// <param name="spriteBatch">A spritebatch used for drawing</param>
        /// <param name="content">A Contentmanager used for loading resources</param>
        /// <param name="displayRect">The area of the progress bar</param>
        /// <param name="style">The design of the progress bar</param>
        /// <param name="total">A maximum amount to be displayed on the progress bar</param>
        /// <param name="start">The start value</param>
        /// <param name="color">The color of the progress bar</param>
        public ProgressBar(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, ProgressStyle style = ProgressStyle.Default, int total = 100, int start = 0)
            : base(parent, spriteBatch, content, displayRect)
        {
            this.total = total;
            _value = start;
            _style = style;
            _color = color;
            _background = _content.Load<Texture2D>("Minimap");
            _font = _content.Load<SpriteFont>("SmallFont");
            _fontHeight = (int)_font.MeasureString("1234567890").Y;
        }
        #endregion
    }
}
