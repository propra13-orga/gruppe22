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
            _spriteBatch.Begin();
            _spriteBatch.Draw(_mapIcon, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, _displayRect.Width - 2, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), Color.Black);
            _spriteBatch.End();
            _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(_displayRect.Left + 3, _displayRect.Top + 3, _displayRect.Width - 6, _displayRect.Height - 6);
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
                            _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + 2 + x * 16, _displayRect.Top + 2 + y * 16, 15, 15), new Rectangle(32, 0, 16, 16), Color.White);
                        }
                        else
                        {
                            if (_map[x, y].hasPlayer)
                            {
                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + 2 + x * 16, _displayRect.Top + 2 + y * 16, 16, 16), new Rectangle(48, 16, 16, 16), Color.White);

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
                                            if (x == 0)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);
                                            }
                                            if (y == 0)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(16, 0, 16, 16), Color.White);
                                            }
                                            if (x == _map.width - 1)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(16, 16, 16, 16), Color.White);
                                            }
                                            if (y == _map.height - 1)
                                            {
                                                _spriteBatch.Draw(_mapIcon, new Rectangle(_displayRect.Left + x * 16, _displayRect.Top + y * 16, 16, 16), new Rectangle(0, 16, 16, 16), Color.White);
                                            }
                                        }
                                        else
                                        {
                                            if (_map[x, y].hasTarget)
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

        /// <summary>
        /// Verschiebung der gedrehten Minimap. 
        /// </summary>
        /// <param name="difference"></param>
        /// <param name="_lastCheck"></param>
        public override void MoveContent(Vector2 difference, int _lastCheck = 0)
        {
            float tmp = 0.70710678118654752440084436210485f; //berechnete rückverschiebung (Rot.matrix für 45°), wurzel 2 halbe
            difference = new Vector2(tmp * difference.X + (-tmp) * difference.Y, tmp * difference.X + tmp * difference.Y);
            Move(difference);
            base.MoveContent(difference);
        }

        public void MoveCamera(Coords coords)
        {
            _camera.position = new Vector2(-(_displayRect.Left + coords.x * 16) - 8, -(_displayRect.Top + coords.y * 16) - 8);
        }

        public new float Zoom
        {
            get
            {
                return _camera.zoom;
            }
            set
            {
                if ((value > 0.3f) && (value < 1.5f))
                    _camera.zoom = value;
            }
        }

        public override void ScrollWheel(int Difference)
        {
            float diff = (float)Difference / 10;
            Zoom -= diff;
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
            _camera.rotate = -45.0f;
            Zoom = 0.9f;

        }
    }
}
