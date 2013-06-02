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
    public class TextInput : UIElement
    {
        protected string _label = "";
        protected string _text = "";
        protected string _toolTip = "";
        protected SpriteFont _font = null;
        protected Texture2D _background = null;
        protected int _textWidth = 0;
        protected bool _canEdit = false;

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


        public bool canFocus
        {
            get
            {
                return _canEdit;
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

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.X, _displayRect.Y + 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 6, _displayRect.Y, _textWidth + 7, _displayRect.Height), new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.Width + _displayRect.X - _textWidth - 5, _displayRect.Y + 1, _textWidth + 5, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _focus ? Color.DarkBlue : Color.Black);
            _spriteBatch.DrawString(_font, _text, new Vector2(_displayRect.X + _displayRect.Width - _textWidth, _displayRect.Y + 2), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
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
            _toolTip = toolTip;
            _canEdit = canEdit;
            _font = _content.Load<SpriteFont>("SmallFont");
            _background = _content.Load<Texture2D>("Minimap");
            _textWidth = ((int)(_font.MeasureString("W").X) + 1) * inputWidth;

        }
    }
}
