﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Gruppe22
{
    public class Statusbox : UIElement
    {
        #region Private Fields
        GraphicsDeviceManager _graphics = null;
        SpriteBatch _spriteBatch = null;
        SpriteFont _font = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;
            RasterizerState rstate = new RasterizerState();
            rstate.ScissorTestEnable = true;
            try
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null);
                _spriteBatch.DrawString(_font, "Arrows to move map, PgUp/PgDown to zoom, Esc to quit", new Vector2(_displayRect.Left + 5, _displayRect.Top + 5), Color.White);
                _spriteBatch.End();
                _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            }
            finally
            {
                rstate.Dispose();
            }

        }
        public bool IsHit(int x, int y)
        {
            return _displayRect.Contains(x, y);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="region"></param>
        /// <param name="font"></param>
        public Statusbox(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle region, SpriteFont font)
        {
            _font = font;
            _displayRect = region;
            _spriteBatch = spriteBatch;
            _graphics = graphics;
        }
        #endregion
    }
}
