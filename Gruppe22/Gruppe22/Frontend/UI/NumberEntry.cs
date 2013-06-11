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
    public class NumberEntry : TextInput
    {
        private int _value = 0;
        private bool _allowIncrease = false;
        private bool _allowDecrease = false;
        private int _originalvalue = 0;
        private Texture2D _arrows = null;

        public int value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }


        public bool allowIncrease
        {
            get { return _allowIncrease; }
            set { _allowIncrease = value; }
        }

        public int originalValue
        {
            get
            {
                return _originalvalue;
            }
        }


        public bool allowDecrease
        {
            get { return _allowDecrease; }
            set { _allowDecrease = value; }
        }
        public override bool canFocus
        {
            get { return ((_allowIncrease || _allowDecrease) && (_visible)); }
        }
        public override void Draw(GameTime gameTime)
        {
            if (_visible)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.X, _displayRect.Y + 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 12, _displayRect.Y, _textWidth + 12, _displayRect.Height), new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 11, _displayRect.Y + 1, _textWidth + 10, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _focus ? Color.DarkBlue : (_allowIncrease || _allowDecrease) ? Color.DarkGreen : Color.Black);
                _spriteBatch.DrawString(_font, _value.ToString(), new Vector2(_displayRect.X + _displayRect.Width - _textWidth - 10, _displayRect.Y + 2), Color.White);
                if (_allowIncrease)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 12, _displayRect.Top + 2, 10, 10), new Rectangle(32, 0, 28, 28), _focus ? Color.Blue : Color.White);
                if (_allowDecrease)
                    _spriteBatch.Draw(_arrows, new Rectangle(_displayRect.Right - 12, _displayRect.Bottom - 12, 10, 10), new Rectangle(0, 0, 28, 28), _focus ? Color.Blue : Color.White);
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public NumberEntry(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string label, int value, string toolTip, int inputWidth, bool canEdit)
            : base(parent, spriteBatch, content, displayRect, label, value.ToString(), toolTip, inputWidth, canEdit)
        {
            _arrows = _content.Load<Texture2D>("Arrows");
            _value = value;
            _originalvalue = value;
            if (canEdit)
            {
                _allowIncrease = true;
                _allowDecrease = true;
            }
        }

    }
}
