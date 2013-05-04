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
    /// <summary>
    /// 
    /// </summary>
    public class UIElement
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

        /// <summary>
        /// A reference to the game's Contentmanager
        /// </summary>
        protected ContentManager _content;

        #endregion

        #region Public Methods

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
            System.Diagnostics.Debug.WriteLine("1");
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
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public UIElement(SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
        {
            _displayRect = displayRect;
            _spriteBatch = spriteBatch;
            _content = content;
        }
        #endregion
    }
}
