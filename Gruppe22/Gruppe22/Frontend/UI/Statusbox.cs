using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Gruppe22
{
    public class Statusbox : UIElement
    {
        #region Private Fields
        SpriteFont _font = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
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
        public override bool IsHit(int x, int y)
        {
            return _displayRect.Contains(x, y);
        }

        #endregion

        public Statusbox(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent,spriteBatch, content, displayRect)
        {
            _font = _content.Load<SpriteFont>("Font");
        }

    }
}
