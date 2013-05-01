using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class Mainmap
    {
        Texture2D _wall1, _wall2, _wall3, _floor;
        /// <summary>
        /// Output device
        /// </summary>
        GraphicsDeviceManager _graphics;
        /// <summary>
        /// Main Sprite drawing algorithm
        /// </summary>
        SpriteBatch _spriteBatch;
        Rectangle _displayRect;
        Map _map;

        public void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            _spriteBatch.Draw(_floor, new Rectangle(1, 1, 128, 96), new Rectangle(512, 384, 128, 96), Color.White);
            _spriteBatch.End();
        }

        public Mainmap(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle displayArea, Texture2D floor, Texture2D wall1, Texture2D wall2, Texture2D wall3, Map map)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _displayRect = displayArea;
            _floor = floor;
            _wall1 = wall1;
            _wall2 = wall2;
            _wall3 = wall3;
            _map = map;

        }
    }
}
