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
        private List<UIElement> _children;
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

        #endregion


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //TODO: Main-UI action, GameTime for effects
            foreach (UIElement child in _children)
            {
                child.Update(gameTime);
            }
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
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X,_displayRect.Y,_displayRect.Width,2), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + _displayRect.Width - 2, _displayRect.Y, 2, _displayRect.Height), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X, _displayRect.Y, 2, _displayRect.Height), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X, _displayRect.Y + _displayRect.Height - 2, _displayRect.Width, 2), new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), new Color(Color.DarkSlateGray, 0.6f));
            _spriteBatch.End();

            foreach (UIElement child in _children)
            {
                child.Draw(gameTime);
            }
        }


        public override void Dispose()
        {         
          //  if (_background != null) _background.Dispose(); // Kills minimap (reused picture!)
            while ((_children != null) && (_children.Count > 0)) { _children[0].Dispose(); _children.RemoveAt(0); }
            base.Dispose();
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Window(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent,spriteBatch, content, displayRect)
        {
            _children = new List<UIElement>();
            _background = _content.Load<Texture2D>("Minimap");
        }
        #endregion
    }
}
