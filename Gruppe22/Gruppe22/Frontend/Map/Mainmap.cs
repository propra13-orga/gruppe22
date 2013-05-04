using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public class Mainmap : Zoomable
    {
        #region Private Fields
        /// <summary>
        /// Textures used under and on the map
        /// </summary>
        private List<TileObject> _environment;
        /// <summary>
        /// List of actors on the map
        /// </summary>
        private List<ActorView> _actors;
        /// <summary>
        /// Internal reference to map data to be displayed
        /// </summary>
        private Map _map;
        private int _lastCheck = 0;
        #endregion


        #region Public Methods


        /// <summary>
        /// Draw the Map
        /// </summary>
        public override void Draw(GameTime gametime)
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
                            null,
                            _camera.matrix);

                _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;

                _drawFloor(_map.width, _map.height);
                foreach (ActorView actor in _actors)
                {
                    actor.Draw(gametime);
                }
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
        private void _drawWall(Direction dir, Rectangle target, bool transparent)
        {
            switch (dir)
            {
                case Direction.UpRight: // Walls on Up and left square
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpLeft: // Walls on up and right square
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownLeft: // Walls on left and down squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownRight: // Walls on right and down squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRight: // Walls on left and right neighboring squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDown:// Walls on up and down neighboring squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.FourWay: // Walls on all surrounding squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.RightClose:
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpClose:
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftClose: // Wall on current square connected to square to the left
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownClose: // Wall on current square connected to square below
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRightUp: // Walls connected left, right and up

                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);

                    break;

                case Direction.LeftRightDown: // Wall connected left right and down
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDownLeft: // Walls on Up, Down and Left suqares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDownRight: // Walls on Up, Down and Right squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.Free: // Free standing wall (no connecting squares)
                    //  _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.None: // No wall
                    break;


            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Rectangle _tileRect(Vector2 coords, bool tall = false)
        {

            return new Rectangle((int)coords.X * 64 + ((int)coords.Y) * 64
                                    , (int)coords.Y * 48 - (int)coords.X * 48, 130, tall ? 194 : 98);
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
                    _drawWall(GetWallStyle(x, y), _tileRect(new Vector2(x + 1, y - 1), true), false);
                }
            }
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
                    _spriteBatch.Draw(_environment[1].animationTexture, _tileRect(new Vector2(x, y)), new Rectangle(512, 384, 128, 96), Color.White);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
//            System.Diagnostics.Debug.WriteLine();
            if (Math.Abs(gameTime.TotalGameTime.Milliseconds/10-_lastCheck) >7)
            {
                _lastCheck = gameTime.TotalGameTime.Milliseconds/10;
                foreach (ActorView actor in _actors)
                {
                    actor.Update(gameTime);
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
        public Mainmap(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayArea, Map map)
            : base(parent, spriteBatch, content, displayArea)
        {
            _map = map;

            // Load textures to use in environment
            _environment = new List<TileObject>();
            _environment.Add(new TileObject(_content, 128, 192));
            _environment[0].AddAnimation("Wall1", new Vector2(0, 0));
            _environment.Add(new TileObject(_content, 128, 192));
            _environment[1].AddAnimation("Floor", new Vector2(0, 0));

            // Create list of actors
            _actors = new List<ActorView>();
            TileObject player = new TileObject(_content, 96, 96);
            player.AddAnimation("Walk", new Vector2(0, 0), -1, 8, 1);
            player.AddAnimation("Walk", new Vector2(0, 96), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 192), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 288), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 384), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 480), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 576), -1, 8, 1);
            player.AddAnimation("Walk", new Vector2(0, 672), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 768), -1, 8, 1);


            _actors.Add(new ActorView(spriteBatch, "Player", true, new Vector2(2, 2), player));
        }
        #endregion

    }
}
