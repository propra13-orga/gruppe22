using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public class Button : UIElement, IDisposable
    {
        #region private Fields
        /// <summary>
        /// 
        /// </summary>
        SpriteFont _font = null;
        /// <summary>
        /// 
        /// </summary>
        Texture2D _background = null;
        /// <summary>
        /// 
        /// </summary>
        private ButtonStatus _bstat = ButtonStatus.normal;
        /// <summary>
        /// 
        /// </summary>
        private enum ButtonStatus
        {
            normal = 0,
            mouseon = 1,
            pressed = 2
        }
        private ButtonState lmb = ButtonState.Pressed;
        private string _label = "";
        private Events _id = 0;

        private List<Texture2D> _buttonStates = null;
        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if ((lmb == ButtonState.Pressed) && (Mouse.GetState().LeftButton == ButtonState.Released))
                    lmb = ButtonState.Released;

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && _bstat != ButtonStatus.pressed && lmb == ButtonState.Released)
                {
                    _bstat = ButtonStatus.pressed;
                    _parent.HandleEvent(this, _id, 0);
                }
                else _bstat = ButtonStatus.mouseon;
            }
            else
            {
                _bstat = ButtonStatus.normal;
                lmb = ButtonState.Pressed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (_label != "")
            {
                Vector2 _textPos = _font.MeasureString(_label);
                switch (_bstat)
                {
                    case ButtonStatus.mouseon:
                        _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.Black);
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.White);
                        _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.Left + (_displayRect.Width - _textPos.X) / 2, _displayRect.Top + (_displayRect.Height - _textPos.Y) / 2), Color.Black);

                        break;
                    default:
                        _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
                        _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.Black);
                        _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.Left + (_displayRect.Width - _textPos.X) / 2, _displayRect.Top + (_displayRect.Height - _textPos.Y) / 2), Color.White);
                        break;
                }
            }
            else
            {
                switch (_bstat)
                {
                    case ButtonStatus.mouseon:
                        _spriteBatch.Draw(_buttonStates[1], new Rectangle(_displayRect.X, _displayRect.Y, Math.Min(_displayRect.Width, _buttonStates[1].Width), Math.Min(_displayRect.Height, _buttonStates[1].Height)), new Rectangle(0, 0, _buttonStates[1].Width, _buttonStates[1].Height), Color.White);
                        break;
                    case ButtonStatus.pressed:
                        _spriteBatch.Draw(_buttonStates[2], new Rectangle(_displayRect.X, _displayRect.Y, Math.Min(_displayRect.Width, _buttonStates[2].Width), Math.Min(_displayRect.Height, _buttonStates[2].Height)), new Rectangle(0, 0, _buttonStates[2].Width, _buttonStates[2].Height), Color.White);
                        break;
                    default:
                        _spriteBatch.Draw(_buttonStates[0], new Rectangle(_displayRect.X, _displayRect.Y, Math.Min(_displayRect.Width, _buttonStates[0].Width), Math.Min(_displayRect.Height, _buttonStates[0].Height)), new Rectangle(0, 0, _buttonStates[0].Width, _buttonStates[0].Height), Color.White);
                        break;
                }
            }

            _spriteBatch.End();
        }

        public override void Dispose()
        {
            //  if (_background != null) _background.Dispose(); // Kills minimap (reused picture)
            //  while ((_buttonStates != null) && (_buttonStates.Count > 0)) { _buttonStates[0].Dispose(); _buttonStates.RemoveAt(0); } // prevents reloading by content-manager: Button appears black
            base.Dispose();
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Create a button using a bitmap
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        /// <param name="button"></param>
        /// <param name="bpressed"></param>
        /// <param name="bmouseon"></param>
        public Button(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string normal, string active, string pressed, Events id)
            : base(parent, spriteBatch, content, displayRect)
        {
            _buttonStates = new List<Texture2D>();
            _buttonStates.Add(_content.Load<Texture2D>(normal));
            _buttonStates.Add(_content.Load<Texture2D>(active));
            _buttonStates.Add(_content.Load<Texture2D>(pressed));
            _id = id;
            _bstat = ButtonStatus.normal;
        }

        /// <summary>
        /// Create a button using a text label
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        /// <param name="button"></param>
        /// <param name="bpressed"></param>
        /// <param name="bmouseon"></param>
        public Button(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string label, Events id)
            : base(parent, spriteBatch, content, displayRect)
        {
            _background = _content.Load<Texture2D>("Minimap");
            _font = _content.Load<SpriteFont>("font");
            _label = label;
            _id = id;
            _bstat = ButtonStatus.normal;
        }

        #endregion


    }
}
