using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{

    public interface IHandleEvent
    {
        void HandleEvent(bool DownStream, Events eventID, params object[] data);
    }
    /// <summary>
    /// 
    /// </summary>
    public class UIElement : IHandleEvent, IDisposable, IKeyHandler
    {
        #region Private (Protected) Fields
        /// <summary>
        /// Central Sprite drawing algorithm
        /// </summary>
        protected SpriteBatch _spriteBatch = null;

        /// <summary>
        /// The area used for the element
        /// </summary>
        protected Rectangle _displayRect;

        protected bool _visible = true;


        protected IHandleEvent _parent;

        /// <summary>
        /// A reference to the game's Contentmanager
        /// </summary>
        protected ContentManager _content;

        protected bool _focus = false;

        #endregion

        #region Implementation of IKeyHandler-Interface
        public virtual bool OnKeyDown(Keys k)
        {
            return false;
        }

        public virtual bool OnKeyUp(Keys k)
        {
            return false;
        }

        public virtual bool OnMouseDown(int button)
        {
            if ((_displayRect.Contains(Mouse.GetState().X, Mouse.GetState().Y)) && (canFocus && !_focus)) _parent.HandleEvent(false, Events.RequestFocus, this);
            return false;
        }
        public virtual bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public virtual void Show()
        {
            _visible = true;
        }

        public virtual void Hide()
        {
            _visible = false;
        }
        public virtual bool OnMouseUp(int button)
        {
            return false;
        }

        public virtual bool OnMouseHeld(int button)
        {
            return false;
        }

        public virtual bool OnKeyHeld(Keys k)
        {
            return false;
        }
        #endregion
        #region Public Fields

        public virtual bool ignorePause
        {
            get { return false; }
        }

        public virtual void Resize(Rectangle rect)
        {
            _displayRect = rect;
        }

        public virtual bool holdFocus
        {
            get { return false; }
        }

        public bool focus
        {
            get
            {
                return _focus;
            }
            set
            {
                _focus = value;
            }
        }

        public virtual bool canFocus
        {
            get
            {
                return _visible;
            }
        }
        #endregion

        #region Public Methods

        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            if (!DownStream)
            {
                _parent.HandleEvent(false, eventID, data);
            }
        }


        /// <summary>
        /// Check whether a pixel is part of the window
        /// </summary>
        /// <param name="x">Horizontal coordinate</param>
        /// <param name="y">Vertical coordinate</param>
        /// <returns>true if pixel is part of the window</returns>
        public virtual bool IsHit(int x, int y)
        {
            return _displayRect.Contains(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Button"></param>
        public virtual void Click(int Button)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="difference"></param>
        public virtual void MoveContent(Vector2 difference, int _lastCheck = 0)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Difference"></param>
        public virtual void ScrollWheel(int Difference)
        {

        }


        public virtual void Dispose()
        {

        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public UIElement(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
        {
            _displayRect = displayRect;
            _spriteBatch = spriteBatch;
            _content = content;
            _parent = parent;
        }

        #endregion
    }
}
