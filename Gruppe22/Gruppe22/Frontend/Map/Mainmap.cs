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
        DownClose = 14,
        RightClose = 16,
        UpClose = 17,
        Free = 15,
        None = -1
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
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            _desaturateEffect,
                            _camera.GetMatrix(_graphics));

                _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;

                _drawFloor(_map.width, _map.height);
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

                case Direction.RightClose:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
y * 48 - 96, 128, 192), new Rectangle(0, 206, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpClose:
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 53,
y * 48 - 52, 22, 30), new Rectangle(148, 53, 22, 30), transparent ? new Color(Color.White, (float)0.5) : Color.White); 
                    break;

                case Direction.LeftClose: // Wall on current square connected to square to the left
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
    y * 48 - 96, 128, 192), new Rectangle(128, 206, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;

                case Direction.DownClose: // Wall on current square connected to square below
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 53,
y * 48 - 52, 23, 30), new Rectangle(148, 53, 22, 30), transparent ? new Color(Color.White, (float)0.5) : Color.White); 

                    break;

                case Direction.LeftRightUp: // Walls connected left, right and up
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(768, 590, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.LeftRightDown: // Wall connected left right and down
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(640, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.UpDownLeft: // Walls on Up, Down and Left suqares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(512, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.UpDownRight: // Walls on Up, Down and Right squares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(768, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.UpRight: // Walls on Up and left square
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(256, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.UpLeft: // Walls on up and right square
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
y * 48 - 96, 128, 192), new Rectangle(384, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DownLeft: // Walls on left and down squares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(128, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;
                case Direction.DownRight: // Walls on right and down squares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(0, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRight: // Walls on left and right neighboring squares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
      y * 48 - 96, 128, 192), new Rectangle(322, 0, 126, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    /*_spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 ,
    y * 48 - 96, 128, 192), new Rectangle(0, 590, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);*/


                    break;
                case Direction.UpDown:// Walls on up and down neighboring squares
                    //  _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1 ,
                    //    y * 48 - 96, 128, 192), new Rectangle(128, 590, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White); 

                     _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 53,
                    y * 48 - 82, 23, 30), new Rectangle(148, 53, 22, 30), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                     _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 53,
y * 48 - 52, 23, 30), new Rectangle(148, 53, 22, 30), transparent ? new Color(Color.White, (float)0.5) : Color.White); 

                    break;

                case Direction.FourWay: // Walls on all surrounding squares
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
   y * 48 - 96, 128, 192), new Rectangle(256, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;

                case Direction.Free: // Free standing wall (no connecting squares)
                    _spriteBatch.Draw(_wall1, new Rectangle(x * 128 + 1,
    y * 48 - 96, 128, 192), new Rectangle(0, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;

                case Direction.None: // No wall
                    break;


            }
        }

        public Direction GetWallStyle(int x = 0, int y = 0)
        {
            if (_map[x, y].canEnter) return Direction.None;


            if (_map[x - 1, y].canEnter) // No wall left
            {

                // No wall blocks way to left

                if (_map[x + 1, y].canEnter) // No wall right
                {

                    // No wall blocks way to left or right

                    if (_map[x, y - 1].canEnter) // No wall up
                    {
                        // No wall blocks way up, left or right

                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // No wall blocks way up, down, left or right => this is a freestanding wall surrounded by walkable space
                            return Direction.Free;
                        }
                        else // Wall Down (only)
                        {
                            // Wall only on current square and square above
                            return Direction.DownClose;
                        }
                    }
                    else // Wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Wall ony on current square and square below
                            return Direction.UpClose;
                        }
                        else // Wall up and down
                        {
                            // Wall on current square and squares above and below
                            return Direction.UpDown;
                        }
                    }
                }
                else // Wall right
                {
                    if (_map[x, y - 1].canEnter) // No wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Wall on current tile and right only, but not up or down
                            return Direction.RightClose;
                        }
                        else // Wall down
                        {
                            // Wall right and down, but not left and up
                            return Direction.DownRight;
                        }
                    }
                    else // Wall up
                    {
                        // Wall up and right, but not left
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Wall up, right, but not left and down
                            return Direction.UpRight;
                        }
                        else // Wall down
                        {
                            // Wall up, right and down, but not left
                            return Direction.UpDownRight;
                        }
                    }
                }
            }
            else
            {
                if (_map[x + 1, y].canEnter) // No Wall right
                {
                    if (_map[x, y - 1].canEnter) // No wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Left and Right closed
                            return Direction.LeftClose;
                        }
                        else  // Wall down
                        {
                            // Left and bottom closed
                            return Direction.DownLeft;
                        }
                    }
                    else // Wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Left and Up closed
                            return Direction.UpLeft;
                        }
                        else // Wall down
                        {
                            // Left, Up and Down closed
                            return Direction.UpDownLeft;
                        }
                    }
                }
                else // Wall Left and Right
                {
                    if (_map[x, y - 1].canEnter) // No wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Walls left and right only
                            return Direction.LeftRight;
                        }
                        else // wall up
                        {
                            // All walls but not up
                            return Direction.LeftRightDown;

                        }
                    }
                    else
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // All walls but not down
                            return Direction.LeftRightUp;
                        }
                        else // wall down
                        {
                            // Surrounded by walls
                            return Direction.FourWay;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Display all walls on the current map
        /// </summary>
        private void _drawWalls()
        {

            for (int y = 0; y < _map.height; ++y)
            {


                for (int x = 0; x < _map.width; ++x)
                {
                    _drawWall(GetWallStyle(x, y), x, y, true);
                }
            }
            /*
               _drawWall(Direction.FourWay, 1, 3, false);
               _drawWall(Direction.DownLeft, 2, 1, false);
               _drawWall(Direction.LeftRight, 3, 1, false);
               _drawWall(Direction.LeftRight, 2, 2, false);
               _drawWall(Direction.UpRight, 3, 2, false);
               */

            //_drawWall(Direction.UpRight, 3, 4, false);
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
            _camera.zoom = (float)0.5;
            _camera.Move(new Vector2(70, 110));
            _floor = floor;
            _wall1 = wall1;
            _wall2 = wall2;
            _map = map;

        }
        #endregion

    }
}
