using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Gruppe22
{
    public class Statusbox
    {
        #region Private Fields
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Rectangle _paintRegion;
        float _zoom = (float)1.0;
        Rectangle _mapRegion;
        Map _map;
        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Draw(GameTime gameTime)
        {
            SpriteFont font;
            _spriteBatch.DrawString(font, "Press Escape to quit", new Vector2(_paintRegion.Left + 5, _paintRegion.Top + 5), Color.White);
        }

        public Statusbox(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle region)
        {
            _paintRegion = region;
            _spriteBatch = spriteBatch;
            _graphics = graphics;
        }
    }
}
