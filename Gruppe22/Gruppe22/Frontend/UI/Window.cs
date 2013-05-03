using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Gruppe22
{
    public class Window
    {
        private GraphicsDeviceManager _graphics;
        private Texture2D _background;

        private Rectangle _dest;
        private Rectangle _src;

        private int _width, _heigth;

        /// <summary>
        /// Constructor
        /// </summary>
        public Window(GraphicsDeviceManager graphics)
        {
            this._graphics = graphics;
            this._src = new Rectangle(0, 0, 1, 1);
            this._width = _graphics.PreferredBackBufferWidth;
            this._heigth = _graphics.PreferredBackBufferHeight;
            this._dest = new Rectangle(0, 0, _width, _heigth);
        }

#region Public Gamesystem-Methods

        public void LoadContent(ContentManager Content)
        {
            _background = Content.Load<Texture2D>("startwindow");
        }

        public void Update(GameTime gameTime)
        { 
            //TODO: Main-UI action, GameTime for effects
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, _dest, _src, Color.DarkSlateGray);
            spriteBatch.End();
        }

#endregion

    }
}
