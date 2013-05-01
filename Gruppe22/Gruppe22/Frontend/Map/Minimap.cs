using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class Minimap
    {
        #region Private Fields
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _mapIcon;
        Rectangle _paintRegion;
        float _zoom = (float)1.0;
        Rectangle _mapRegion;
        Map _map;
        #endregion

        #region Public Methods


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (_map != null)
            {
                for (int y = 0; y < _map.height; ++y)
                {
                    for (int x = 0; x < _map.height; ++x)
                    {
                        if (!_map[x, y].canEnter)
                        {
                            _spriteBatch.Draw(_mapIcon, new Rectangle(x*16, y*16, 16, 16), new Rectangle(32, 0, 16, 16), Color.White);
                        }
                        else
                        {
                            if (_map[x, y].hasPlayer)
                            {
                                _spriteBatch.Draw(_mapIcon, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(32, 16, 16, 16), Color.White);
                            }
                            else
                            {
                                if (_map[x, y].hasEnemy)
                                {
                                    _spriteBatch.Draw(_mapIcon, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(64, 16, 16, 16), Color.White);
                                }
                                else
                                {
                                    if (_map[x, y].hasTreasure)
                                    {
                                        _spriteBatch.Draw(_mapIcon, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(64, 0, 16, 16), Color.White);
                                    }
                                    else
                                    {
                                        if (_map[x, y].hasTeleport)
                                        {
                                            _spriteBatch.Draw(_mapIcon, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(0, 16, 16, 16), Color.White);
                                        }
                                        else
                                        {
                                            if (_map[x, y].hasSpecial)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(x * 16, y * 16, 16, 16), new Rectangle(48, 0, 16, 16), Color.White);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            _spriteBatch.End();
        }
        #endregion


        public Minimap(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle region, Texture2D mapIcons, Map map)
        {
            _mapIcon = mapIcons;
            _paintRegion = region;
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _map = map;
        }
    }
}
