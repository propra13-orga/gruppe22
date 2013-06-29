using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    /// <summary>
    /// 
    /// </summary>
    public enum ButtonStatus
    {
        normal = 0,
        mouseon = 1,
        down = 3,
        downon = 4
    }
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
        private bool _hidden = false;
        private string _label = "";
        private int _id = 0;

        private List<Texture2D> _buttonStates = null;
        #endregion

        #region Public Fields


        public override bool canFocus
        {
            get
            {
                return !_hidden;
            }
        }
        /// <summary>
        /// True if the button should stay down
        /// </summary>
        public bool stayDown
        {
            get
            {
                return ((_bstat == ButtonStatus.downon) || (_bstat == ButtonStatus.down));
            }
            set
            {
                if (value == true)
                {

                    if (_bstat == ButtonStatus.normal)
                    {
                        _bstat = ButtonStatus.down;
                    }
                    if (_bstat == ButtonStatus.mouseon)
                    {
                        _bstat = ButtonStatus.downon;
                    }
                }
                else
                {
                    if (_bstat == ButtonStatus.downon)
                    {
                        _bstat = ButtonStatus.mouseon;
                    }
                    if (_bstat == ButtonStatus.down)
                    {
                        _bstat = ButtonStatus.normal;
                    }
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override bool OnMouseDown(int button)
        {
            if (!_hidden) { 
            if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _parent.HandleEvent(false, Backend.Events.ButtonPressed, _id);
                return true;
            }
            }
            return false;
        }
        public override bool OnKeyDown(Keys k)
        {
            if (!_hidden) { 
            if (_focus && ((k == Keys.Space) || (k == Keys.Enter)))
            {
                _parent.HandleEvent(false, Backend.Events.ButtonPressed, _id);
                return true;
            }
            }
            return false;
        }


        public override void HandleEvent(bool downstream, Backend.Events eventID, params object[] data)
        {
          /*  if ((eventID == Backend.Events.ToggleButton) && ((int)data[0] == _id))
            {
                stayDown = (bool)data[1];
            } */
        }

        public bool hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
            }
        }

        public override void Show()
        {
            _hidden = false;
        }
        public override void Hide()
        {
            _hidden = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime)
        {
            if (!_hidden)
            {
                _spriteBatch.Begin();
                if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    if (_bstat == ButtonStatus.down)
                    {
                        _bstat = ButtonStatus.downon;
                    }
                    else
                        if (_bstat == ButtonStatus.normal)
                        {
                            _bstat = ButtonStatus.mouseon;
                        }
                }
                else
                {
                    if (_bstat == ButtonStatus.downon)
                    {
                        _bstat = ButtonStatus.down;
                    }
                    else if (_bstat == ButtonStatus.mouseon)
                    {
                        _bstat = ButtonStatus.normal;
                    }
                }

                if (_label != "")
                {
                    Vector2 _textPos = _font.MeasureString(_label);
                    switch (_bstat)
                    {
                        case ButtonStatus.down:
                        case ButtonStatus.mouseon:
                        case ButtonStatus.downon:
                            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.Black);
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, _displayRect.Width - 2, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
                            _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.Left + (_displayRect.Width - _textPos.X) / 2, _displayRect.Top + (_displayRect.Height - _textPos.Y) / 2), Color.Black);
                            break;
                        default:
                            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), _focus ? Color.Blue : Color.White);
                            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, _displayRect.Width - 2, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), _focus ? Color.DarkBlue : Color.Black);
                            _spriteBatch.DrawString(_font, _label, new Vector2(_displayRect.Left + (_displayRect.Width - _textPos.X) / 2, _displayRect.Top + (_displayRect.Height - _textPos.Y) / 2), Color.White);
                            break;
                    }
                }
                else
                {
                    switch (_bstat)
                    {
                        case ButtonStatus.mouseon:
                        case ButtonStatus.down:
                            _spriteBatch.Draw(_buttonStates[1], new Rectangle(_displayRect.X, _displayRect.Y, Math.Min(_displayRect.Width, _buttonStates[1].Width), Math.Min(_displayRect.Height, _buttonStates[1].Height)), new Rectangle(0, 0, _buttonStates[1].Width, _buttonStates[1].Height), Color.White);
                            break;
                        case ButtonStatus.downon:
                        default:
                            _spriteBatch.Draw(_buttonStates[0], new Rectangle(_displayRect.X, _displayRect.Y, Math.Min(_displayRect.Width, _buttonStates[0].Width), Math.Min(_displayRect.Height, _buttonStates[0].Height)), new Rectangle(0, 0, _buttonStates[0].Width, _buttonStates[0].Height), Color.White);
                            break;
                    }
                }

                _spriteBatch.End();
            }
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
        public Button(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string normal, string active, string pressed, int id)
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
        public Button(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string label, int id, bool staydown = false)
            : base(parent, spriteBatch, content, displayRect)
        {
            _background = _content.Load<Texture2D>("Minimap");
            _font = _content.Load<SpriteFont>("font");
            _label = label;
            _id = id;
            _bstat = ButtonStatus.normal;
            this.stayDown = staydown;
        }

        #endregion


    }
}
