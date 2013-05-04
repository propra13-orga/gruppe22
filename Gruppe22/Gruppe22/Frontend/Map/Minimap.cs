using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class Minimap : Zoomable
    {
        #region Private Fields
        private Texture2D _mapIcon;
        private Map _map;
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

            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                  BlendState.AlphaBlend,
                  null,
                  null,
                  rstate,

                  null,
                  _camera.matrix);
            if (_map != null)
            {
                for (int y = 0; y < _map.height; ++y)
                {
                    for (int x = 0; x < _map.width; ++x)
                    {
                        if (!_map[x, y].canEnter)
                        {
                            _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 15, 15), new Rectangle(32, 0, 16, 16), Color.White);
                        }
                        else
                        {
                            if (_map[x, y].hasPlayer)
                            {
                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(32, 16, 16, 16), Color.White);
                            }
                            else
                            {
                                if (_map[x, y].hasEnemy)
                                {
                                    _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(64, 16, 16, 16), Color.White);
                                }
                                else
                                {
                                    if (_map[x, y].hasTreasure)
                                    {
                                        _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(64, 0, 16, 16), Color.White);
                                    }
                                    else
                                    {
                                        if (_map[x, y].hasTeleport)
                                        {
                                            _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(0, 16, 16, 16), Color.White);
                                        }
                                        else
                                        {
                                            if (_map[x, y].hasSpecial)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(48, 0, 16, 16), Color.White);
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
        public Minimap(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle region, Map map)
            : base(parent, spriteBatch, content, region)
        {
            _map = map;
            _mapIcon = _content.Load<Texture2D>("Minimap");
        }
    }
}
