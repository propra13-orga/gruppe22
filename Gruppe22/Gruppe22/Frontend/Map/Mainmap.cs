using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    /// <summary>
    /// Different wall-directions
    /// </summary>
    public enum Direction
    {
        LeftRightUp = 0,
        LeftRightDown = 1,
        UpDownLeft = 5,
        UpDownRight = 6,
        UpLeft = 7,
        UpRight = 8,
        DownLeft = 9,
        DownRight = 10,
        LeftRight = 2,
        UpDown = 3,
        FourWay = 4,
        LeftClose = 13,
        DownClose = 14
    }

    /// <summary>
    /// The core display of the current part of the dungeon
    /// </summary>
    public class Mainmap
    {
        #region Private Fields
        /// <summary>
        /// A graphical effect used depending on player health
        /// </summary>
        private Effect _desaturateEffect;
        /// <summary>
        /// Textures used under and on the map
        /// </summary>
        private Texture2D _wall1, _wall2, _floor;
        /// <summary>
        /// Output device
        /// </summary>
        private GraphicsDeviceManager _graphics;
        /// <summary>
        /// Main Sprite drawing algorithm
        /// </summary>
        private SpriteBatch _spriteBatch;
        /// <summary>
        /// The area used for the map (employed e.g. in splitscreen-mode)
        /// </summary>
        private Rectangle _displayRect;
        /// <summary>
        /// The transformation-matrix used for zooming and panning the map
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// Internal reference to map data to be displayed
        /// </summary>
        private Map _map;
        #endregion

        #region Public Fields
        /// <summary>
        /// Current zoom level
        /// </summary>
        public float Zoom
        {
            get
            {
                return _camera.zoom;
            }
            set
            {
                _camera.zoom = value;
            }
        }

        /// <summary>
        /// CUrrent position of the map
        /// </summary>
        public Vector2 Pos
        {
            get
            {
                return _camera.position;
            }
            set
            {
                _camera.position = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Move the camera by a specified number of pixels (pass through to camera)
        /// </summary>
        /// <param name="target"></param>
        public void Move(Vector2 target)
        {
            _camera.Move(target);
        }

        /// <summary>
        /// Draw the Map
        /// </summary>
        public void Draw()
        {
            RasterizerState rstate = new RasterizerState();

            rstate.ScissorTestEnable = true;
            try
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            _desaturateEffect,
                            _camera.GetMatrix(_graphics));

                _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;

                _drawFloor();
                _drawWalls();

                _spriteBatch.End();
                _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            }
            finally
            {
                rstate.Dispose();
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Display a wall
        /// </summary>
        /// <param name="dir">Squares the wall connects to</param>
        /// <param name="x">Horizontal position</param>
        /// <param name="y">Vertical position</param>
        /// <param name="transparent"></param>
        private void _drawWall(Direction dir, int x, int y, bool transparent)
        {
            switch (dir)
            {
                case Direction.FourWay:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                        y * 48 - 96, 128, 192), new Rectangle(384, 782, 128, 192), Color.White);
                    break;
                case Direction.LeftRight:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                        y * 48 - 96, 128, 192), new Rectangle(0, 590, 128, 192), Color.White);
                    break;
                case Direction.UpDown:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                        y * 48 - 96, 128, 192), new Rectangle(128, 590, 128, 192), Color.White);
                    break;
                case Direction.DownLeft:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(256, 590, 128, 192), Color.White);
                    break;
                case Direction.DownRight:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(384, 590, 128, 192), Color.White);
                    break;
                case Direction.UpRight:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(0, 782, 128, 192), Color.White);
                    break;
                case Direction.UpLeft:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(128, 782, 128, 192), Color.White);
                    break;
                case Direction.UpDownRight:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(512, 590, 128, 192), Color.White);
                    break;
                case Direction.UpDownLeft:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(640, 590, 128, 192), Color.White);
                    break;
                case Direction.LeftRightUp:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(768, 590, 128, 192), Color.White);
                    break;
                case Direction.LeftRightDown:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                       y * 48 - 96, 128, 192), new Rectangle(896, 590, 128, 192), Color.White);
                    break;
                case Direction.LeftClose:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                        y * 48 - 96, 128, 192), new Rectangle(128, 206, 128, 192), Color.White);
                    break;
                case Direction.DownClose:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 + (y % 2) * 64,
                        y * 48 - 96, 128, 192), new Rectangle(256, 205, 128, 192), Color.White);
                    break;

            }
        }

        /// <summary>
        /// Display all walls on the current map
        /// </summary>
        private void _drawWalls()
        {
            _drawWall(Direction.FourWay, 3, 3, false);
            _drawWall(Direction.UpRight, 3, 4, false);
            _drawWall(Direction.LeftClose, 4, 4, false);
        }

        /// <summary>
        /// Display the floor (using "rugged edges" to hide isometric-pattern)
        /// </summary>
        /// <param name="hTiles">Number of vertical Tiles</param>
        /// <param name="vTiles">Number of horizontal Tiles</param>
        private void _drawFloor(int hTiles = 52, int vTiles = 25)
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
        #endregion


        #region Constructor
        /// <summary>
        /// Create the visible version of the game map
        /// </summary>
        /// <param name="graphics">The core graphics device manager</param>
        /// <param name="spriteBatch">A sprite batch used for drawing</param>
        /// <param name="displayArea">The area on wich the map will be placed</param>
        /// <param name="floor">The textures used for the floor</param>
        /// <param name="wall1">A set of tiles for the walls</param>
        /// <param name="wall2">A set of tiles for doors</param>
        /// <param name="map">Internal storage of map data</param>
        public Mainmap(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle displayArea, Texture2D floor, Texture2D wall1, Texture2D wall2, Effect desaturate, Map map)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _displayRect = displayArea;
            _desaturateEffect = desaturate;
            _camera = new Camera();
            _floor = floor;
            _wall1 = wall1;
            _wall2 = wall2;
            _map = map;

        }
        #endregion

    }
}
