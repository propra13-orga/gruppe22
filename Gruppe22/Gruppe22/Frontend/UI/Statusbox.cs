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
        Texture2D _background = null;
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
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null);
              _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.Black);

                string text="Use Arrow-keys to move map, PgUp/PgDown to zoom, W/A/S/D to walk, Esc to open menu";
                Vector2 _textPos = _font.MeasureString(text);

                _spriteBatch.DrawString(_font, text, new Vector2(_displayRect.Left + (_displayRect.Width - _textPos.X) / 2, _displayRect.Top + (_displayRect.Height - _textPos.Y) / 2), Color.White);
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
            _background = _content.Load<Texture2D>("Minimap");

        }

    }
}
