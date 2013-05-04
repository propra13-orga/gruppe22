﻿using System;
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
        void HandleEvent(UIElement sender, Events eventID, int data);
    }
    /// <summary>
    /// 
    /// </summary>
    public class UIElement : IHandleEvent, IDisposable
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


        protected IHandleEvent _parent;

        /// <summary>
        /// A reference to the game's Contentmanager
        /// </summary>
        protected ContentManager _content;

        #endregion

        #region Public Fields

        public virtual bool ignorePause
        {
            get { return false; }
        }

        public virtual bool holdFocus
        {
            get { return false; }
        }
        #endregion

        #region Public Methods

        public virtual void HandleEvent(UIElement sender, Events eventID, int data)
        {
            _parent.HandleEvent(sender, eventID, data);
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
        public virtual void MoveContent(Vector2 difference)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Difference"></param>
        public virtual void ScrollWheel(int Difference)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void HandleKey()
        {

        }

        public virtual void Dispose(){
        
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