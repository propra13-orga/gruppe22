using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public class Window : UIElement
    {
        #region Private Fields

        /// <summary>
        /// Single pixel to fill background
        /// </summary>
        private Texture2D _background;

        /// <summary>
        /// UI-Children
        /// </summary>
        protected List<UIElement> _children;
        protected int _focusID = 0;
        #endregion

        #region Public Fields

        public override bool ignorePause
        {
            get { return true; }
        }

        public override bool holdFocus
        {
            get { return true; }
        }

        public List<UIElement> children
        {
            get { return _children; }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Main-UI action, GameTime for effects
            for (int i = 0; i < _children.Count; ++i)
            {
                _children[i].Update(gameTime);
            }
        }

        public override bool OnKeyDown(Keys k)
        {
            for (int i = 0; i < _children.Count; ++i)
            {
                if (_children[i].OnKeyDown(k)) return true;
            };
            switch (k)
            {
                case Keys.Tab:
                    if ((Keyboard.GetState().IsKeyDown(Keys.LeftShift)) || (Keyboard.GetState().IsKeyDown(Keys.RightShift)))
                    {
                        ChangeFocus(false);
                    }
                    else
                    {
                        ChangeFocus(true);
                    }
                    break;
                case Keys.Left:
                case Keys.Up:
                    ChangeFocus(false);
                    break;
                case Keys.Down:
                case Keys.Right:
                    ChangeFocus(true);
                    break;
            }
            return true;
        }

        public override bool OnMouseDown(int button)
        {
            for (int i = 0; i < _children.Count; ++i)
            {
                if (_children[i].OnMouseDown(button)) return true;
            }
            return true;
        }

        public void AddChild(UIElement child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X, _displayRect.Y, _displayRect.Width, 2), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + _displayRect.Width - 2, _displayRect.Y, 2, _displayRect.Height), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X, _displayRect.Y, 2, _displayRect.Height), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X, _displayRect.Y + _displayRect.Height - 2, _displayRect.Width, 2), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), new Color(Color.Black, 0.6f));
            _spriteBatch.End();

            foreach (UIElement child in _children)
            {
                child.Draw(gameTime);
            }
        }

        public void ChangeFocus(bool forward = true)
        {
            if (_children.Count > _focusID)
            {
                _children[_focusID].focus = false;
                int current = _focusID;
                for (int count = 0; count < _children.Count; ++count)
                {
                    if (forward)
                    {
                        current += 1;
                    }
                    else
                    {
                        current -= 1;
                    }
                    if (current == _children.Count)
                        current = 0;
                    if (current < 0)
                    {
                        current = _children.Count - 1;
                    }
                    if (_children[current].canFocus)
                    {
                        _children[current].focus = true;
                        _focusID = current;
                        return;
                    }
                }
            }
        }

        public override void Dispose()
        {
            //  if (_background != null) _background.Dispose(); // Kills minimap (reused picture!)
            while ((_children != null) && (_children.Count > 0)) { _children[0].Dispose(); _children.RemoveAt(0); }
            base.Dispose();
        }

        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (DownStream)
            {
                foreach (UIElement child in _children)
                {
                    switch (eventID)
                    {
                        default:
                            child.HandleEvent(true, eventID, data);
                            break;
                    }
                }
            }
            else
            {
                switch (eventID)
                {
                    case Events.ButtonPressed:
                            switch ((Buttons)data[0])
                            {
                                case Buttons.Close:
                                    _parent.HandleEvent(false, Events.ContinueGame, null);
                                    break;
                                case Buttons.Credits:
                                    _parent.HandleEvent(false, Events.About, null);
                                    break;
                                case Buttons.LAN:
                                    _parent.HandleEvent(false, Events.LAN, null);
                                    break;
                                case Buttons.Load:
                                    _parent.HandleEvent(false, Events.LoadFromCheckPoint, null);
                                    break;
                                case Buttons.Local:
                                    _parent.HandleEvent(false, Events.Local, null);
                                    break;
                                case Buttons.NewMap:
                                    _parent.HandleEvent(false, Events.NewMap, null);
                                    break;
                                case Buttons.Quit:
                                    _parent.HandleEvent(false, Events.EndGame, null);
                                    break;
                                case Buttons.Reset:
                                    _parent.HandleEvent(false, Events.ResetGame, null);
                                    break;
                                case Buttons.Restart:
                                    _parent.HandleEvent(false, Events.ResetGame, null);
                                    break;
                                case Buttons.Settings:
                                    _parent.HandleEvent(false, Events.Settings, null);
                                    break;
                                case Buttons.SinglePlayer:
                                    _parent.HandleEvent(false, Events.Player1, null);
                                    break;
                                case Buttons.TwoPlayers:
                                    _parent.HandleEvent(false, Events.Player2, null);
                                    break;
                                default:
                                    _parent.HandleEvent(false, eventID, data);
                                    break;
                            }
                        break;
                    case Events.RequestFocus:
                        _children[_focusID].focus = false;
                        ((UIElement)data[0]).focus = true;
                        for (int i = 0; i < _children.Count; ++i)
                        {
                            if (_children[i] == data[0]) { _focusID = i; break; }
                        }
                        break;
                    default:
                        _parent.HandleEvent(false, eventID, data);
                        break;
                }
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Window(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
            _children = new List<UIElement>();
            _background = _content.Load<Texture2D>("Minimap");
        }
        #endregion
    }
}
