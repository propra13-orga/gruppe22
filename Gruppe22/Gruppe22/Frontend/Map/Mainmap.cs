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


        private void _drawFloor(int hTiles = 5, int vTiles = 5)
        {
            for (int y = 0; y < vTiles; ++y)
            {
                for (int x = 0; x < hTiles; ++x)
                {
                    // Special case top row (fuzzy border)
                    if (y == 0)
                    {
                        _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 65, -47, 128, 96), new Rectangle(256, 767, 128, 96), Color.White);

                        // Top left corner
                        if (x == 0)
                        {
                            _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48 + 1, 128, 96), new Rectangle(128, 671, 128, 96), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48 + 1, 128, 96), new Rectangle(128, 863, 128, 96), Color.White);

                        }
                    }

                    // Special case bottom row (fuzzy border)
                    if ((y == vTiles - 1))
                    {
                        _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (vTiles % 2) * 64, vTiles * 48, 128, 96), new Rectangle(128, 767, 128, 96), Color.White);

                        if ((x == hTiles - 1) && (vTiles % 2 == 0))
                        {
                            _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(256, 671, 128, 96), Color.White);
                        }
                        else
                        {
                            if ((x == 0) && (vTiles % 2 == 1))
                            {
                                _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(384, 671, 128, 96), Color.White);
                            }
                            else
                            {
                                _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(256, 863, 128, 96), Color.White);
                            }
                        }
                    }

                    // Normal tiles
                    if ((y != 0) && (y != vTiles - 1))
                    {
                        if ((x == hTiles - 1) && (y % 2 != 0))
                        {
                            _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(0, 863, 128, 96), Color.White);

                        }
                        else
                        {
                            if ((x == 0) && (y % 2 == 0))
                            {
                                _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(384, 863, 128, 96), Color.White);

                            }
                            else
                            {

                                _spriteBatch.Draw(_floor, new Rectangle(x * 128 + 1 + (y % 2) * 64, y * 48 + 1, 128, 96), new Rectangle(512, 384, 128, 96), Color.White);

                            }
                        }
                    }


                }

                // Left and right side (fuzzy border)
                if (y % 2 == 1)
                {
                    //   if (y != vTiles - 1)
                    _spriteBatch.Draw(_floor, new Rectangle(-64, y * 48, 128, 96), new Rectangle(0, 767, 128, 96), Color.White);
                }
                else
                {
                    _spriteBatch.Draw(_floor, new Rectangle(hTiles * 128 + 1 + (y % 2) * 64, y * 48, 128, 96), new Rectangle(384, 767, 128, 96), Color.White);

                }
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            _drawFloor();
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
