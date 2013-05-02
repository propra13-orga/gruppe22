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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _mapIcon;
        private Rectangle _paintRegion;
        private float _zoom = (float)1.0;
        private Rectangle _mapRegion;
        private Map _map;
        private Camera _camera;
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
            _spriteBatch.GraphicsDevice.ScissorRectangle = _paintRegion;
            RasterizerState rstate = new RasterizerState();
            rstate.ScissorTestEnable = true;

            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                  BlendState.AlphaBlend,
                  null,
                  null,
                  rstate,

                  null,
                  _camera.GetMatrix(_graphics));
            if (_map != null)
            {
                for (int y = 0; y < _map.height; ++y)
                {
                    for (int x = 0; x < _map.width; ++x)
                    {
                        if (!_map[x, y].canEnter)
                        {
                            _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 15, 15), new Rectangle(32, 0, 16, 16), Color.White);
                        }
                        else
                        {
                            if (_map[x, y].hasPlayer)
                            {
                                _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 16, 16), new Rectangle(32, 16, 16, 16), Color.White);
                            }
                            else
                            {
                                if (_map[x, y].hasEnemy)
                                {
                                    _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 16, 16), new Rectangle(64, 16, 16, 16), Color.White);
                                }
                                else
                                {
                                    if (_map[x, y].hasTreasure)
                                    {
                                        _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 16, 16), new Rectangle(64, 0, 16, 16), Color.White);
                                    }
                                    else
                                    {
                                        if (_map[x, y].hasTeleport)
                                        {
                                            _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 16, 16), new Rectangle(0, 16, 16, 16), Color.White);
                                        }
                                        else
                                        {
                                            if (_map[x, y].hasSpecial)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_paintRegion.Left + x * 16, _paintRegion.Top + y * 16, 16, 16), new Rectangle(48, 0, 16, 16), Color.White);
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
            _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            rstate.Dispose();
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="region"></param>
        /// <param name="mapIcons"></param>
        /// <param name="map"></param>
        public Minimap(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle region, Texture2D mapIcons, Map map)
        {
            _mapIcon = mapIcons;
            _paintRegion = region;
            _graphics = graphics;
            _camera = new Camera();
            _spriteBatch = spriteBatch;
            _map = map;
            _camera.zoom = (float)0.7;
            _camera.Move(new Vector2(500, 20));
        }
    }
}
