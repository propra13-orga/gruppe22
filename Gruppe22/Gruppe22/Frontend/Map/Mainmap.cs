using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        None = -1,

        LeftRightUpDiag = 20,
        LeftRightDownDiag = 21,
        UpDownLeftDiag = 22,
        UpDownRightDiag = 23,
        UpLeftDiag = 24,
        UpRightDiag = 25,
        DownLeftDiag = 26,
        DownRightDiag = 27,
        LeftRightDiag = 28,
        UpDownDiag = 29,
        FourWayDiag = 30,
        LeftCloseDiag = 31,
        DownCloseDiag = 32,
        RightCloseDiag = 33,
        UpCloseDiag = 34,
        FourDiag = 35,


        DiagUpClose,
        DiagDownClose,
        DiagUpDownClose,
        DiagUpClose2,
        DiagDownClose2,
        DiagUpDownClose2,

        DiagLeftClose,
        DiagRightClose,
        DiagLeftRightClose,

        DiagLeftClose2,
        DiagRightClose2,
        DiagLeftRightClose2

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
        private Texture2D _background = null;
        private Keys _lastKey;
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
                _spriteBatch.Begin();
                _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.Black);

                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);

                _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;
                _drawFloor(_map.width, _map.height);

                _drawWalls(gametime);

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
                    _spriteBatch.Draw(_environment[2].animationTexture, target, new Rectangle(1920, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.None: // No wall
                    break;





                /* Diagonale Mauern */





                case Direction.UpRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(681, 835, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(321, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDownLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDownDiag:// Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.FourDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.RightCloseDiag: // Done (Imperfect)
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(681, 820, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(257, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(385, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.DownCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(136, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRightUpDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRightDownDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.LeftRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case Direction.UpDownRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;



                case Direction.DiagUpClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagUpDownClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagDownClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;



                case Direction.DiagUpClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagUpDownClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagDownClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;


                    //TODO: Connectors


                case Direction.DiagLeftClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagLeftRightClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagRightClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                //TODO: Connectors

                case Direction.DiagLeftClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;
                case Direction.DiagLeftRightClose2: // Done

                   _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                   _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
 
                   break;
                case Direction.DiagRightClose2: // Done
                   _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
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
                            // No wall blocks way up, down, left or right => this is a freestanding wall surrounded by walkable space OR only connected by diagonals
                            if (!_map[x + 1, y + 1].canEnter)   // Down Right diagonal
                            {
                                if (!_map[x + 1, y - 1].canEnter) // Down Right + Up Right diagonal
                                {
                                    if (!_map[x - 1, y + 1].canEnter) // Down Right + Up Right + Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right +Up Right + Down Left + Up Left diagonal
                                        {
                                            return Direction.FourDiag;
                                        }
                                        else // (not down left) Down Right +Up Right + Down Left 
                                        {
                                            return Direction.LeftRightDownDiag;
                                        }
                                    }
                                    else // (not down left)
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right  + Up right + Up Left diagonal (not up right)
                                        {
                                            return Direction.LeftRightUpDiag;
                                        }
                                        else // Down Right  + Up right diagonal (not up right, up left)
                                        {
                                            return Direction.LeftRightDiag;
                                        }
                                    }
                                }
                                else // Not up right
                                {
                                    if (!_map[x - 1, y + 1].canEnter) // Down Right  + Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right + Down Left + Up Left diagonal
                                        {
                                            return Direction.UpDownLeftDiag;
                                        }
                                        else // Down Right + Down Left diagonal 
                                        {
                                            return Direction.UpDownDiag;

                                        }
                                    }
                                    else // Not down left
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right + Up Left diagonal
                                        {
                                            return Direction.UpLeftDiag;
                                        }
                                        else // Not up left: Down right only
                                        {
                                            return Direction.UpCloseDiag;
                                        }
                                    }
                                }
                            }

                            else // not down right
                            {
                                if (!_map[x + 1, y - 1].canEnter) //  Up Right diagonal
                                {
                                    if (!_map[x - 1, y + 1].canEnter) // Up Right + Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Up Right + Down Left + Up Left diagonal
                                        {
                                            return Direction.UpDownRightDiag;
                                        }
                                        else // Up Right + Down Left 
                                        {
                                            return Direction.UpRightDiag;
                                        }
                                    }
                                    else
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Up Right + Up Left diagonal
                                        {
                                            return Direction.DownLeftDiag;
                                        }
                                        else
                                        {
                                            return Direction.DownCloseDiag;
                                        }
                                    }
                                }
                                else // not up right
                                {

                                    if (!_map[x - 1, y + 1].canEnter) //  Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Left + Up Left diagonal
                                        {
                                            return Direction.DownRightDiag;
                                        }
                                        else
                                        {
                                            return Direction.RightCloseDiag;
                                        }
                                    }
                                    else
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) //  Up Left diagonal
                                        {
                                            return Direction.LeftCloseDiag;
                                        }
                                        else
                                        {
                                            return Direction.Free; // Keine Mauer weit und breit?
                                        }
                                    }
                                }
                            }














                        }
                        else // Wall Down (only)
                        {
                            // Wall only on current square and square above

                            // auf Diagonalen testen

                            if (!_map[x + 1, y - 1].canEnter)
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return Direction.DiagUpDownClose2;
                                }
                                else
                                {
                                    return Direction.DiagUpClose2;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return Direction.DiagDownClose2;
                                }
                                else
                                {
                                    return Direction.DownClose;
                                }
                            }

                        }
                    }
                    else // Wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Wall ony on current square and square below


                            // auf Diagonalen testen

                            if (!_map[x + 1, y + 1].canEnter)
                            {
                                if (!_map[x - 1, y + 1].canEnter)
                                {
                                    return Direction.DiagUpDownClose;
                                }
                                else
                                {
                                    return Direction.DiagUpClose;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y + 1].canEnter)
                                {
                                    return Direction.DiagDownClose;
                                }
                                else
                                {
                                    return Direction.UpClose;
                                }
                            }

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

                            // auf Diagonalen testen

                            if (!_map[x - 1, y + 1].canEnter)
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return Direction.DiagLeftRightClose2;
                                }
                                else
                                {
                                    return Direction.DiagLeftClose2;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return Direction.DiagRightClose2;
                                }
                                else
                                {
                                    return Direction.RightClose;
                                }
                            }
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

                            // auf Diagonalen testen

                            if (!_map[x + 1, y + 1].canEnter)
                            {
                                if (!_map[x + 1, y - 1].canEnter)
                                {
                                    return Direction.DiagLeftRightClose;
                                }
                                else
                                {
                                    return Direction.DiagLeftClose;
                                }
                            }
                            else
                            {
                                if (!_map[x + 1, y - 1].canEnter)
                                {
                                    return Direction.DiagRightClose;
                                }
                                else
                                {
                                    return Direction.LeftClose;
                                }
                            }
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
        private void _drawWalls(GameTime gametime)
        {

            for (int y = 0; y < _map.height; ++y)
            {


                for (int x = _map.width; x > -1; --x)
                {
                    _drawWall(GetWallStyle(x, y), _tileRect(new Vector2(x + 1, y - 1), true), false);

                    foreach (ActorView actor in _actors)
                    {
                        if (((int)actor.position.X == x) && ((int)actor.position.Y == y))
                        {
                            actor.Draw(gametime);
                        }
                    }
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
            //TODO: Reimplement rugged tiles
        }

        public override void Update(GameTime gameTime)
        {
            if (_actors[0].isMoving)
                _camera.position = new Vector2(-38 -
         ((_actors[0].position.X * 64 + _actors[0].position.Y * 64)), -30 -
         (((_actors[0].position.Y * 48 - _actors[0].position.X * 48))));
            if (Math.Abs(gameTime.TotalGameTime.Milliseconds / 10 - _lastCheck) > 1)
            {
                _lastCheck = gameTime.TotalGameTime.Milliseconds / 10;
                foreach (ActorView actor in _actors)
                {
                    actor.Update(gameTime);
                }
            }
        }

        public void MovePlayer(Direction dir)
        {
            //  if (!_actors[0].isMoving)
            //    {
            switch (dir)
            {
                case Direction.UpLeft:

                    if (((int)_actors[0].target.X > 0) && (_map[(int)_actors[0].target.X - 1, (int)_actors[0].target.Y].canEnter))
                        _actors[0].Move(new Vector2(-1.0f, 0));
                    break;
                case Direction.DownRight:
                    if (((int)_actors[0].target.X < _map.width - 1) && (_map[(int)_actors[0].target.X + 1, (int)_actors[0].target.Y].canEnter))
                        _actors[0].Move(new Vector2(1.0f, 0));
                    break;
                case Direction.DownLeft:
                    if (((int)_actors[0].target.Y < _map.height - 1) && (_map[(int)_actors[0].target.X, (int)_actors[0].target.Y + 1].canEnter))
                        _actors[0].Move(new Vector2(0, 1.0f));
                    break;
                case Direction.UpRight:
                    if (((int)_actors[0].target.Y > 0) && (_map[(int)_actors[0].target.X, (int)_actors[0].target.Y - 1].canEnter))
                        _actors[0].Move(new Vector2(0, -1.0f));
                    break;
            }


            // }

        }

        public override void MoveContent(Vector2 difference)
        {
            // if (!_actors[0].isMoving) 
            base.MoveContent(difference);
        }


        public override void HandleKey()
        {

            if (!_actors[0].isMoving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {

                    MovePlayer(Direction.UpRight);
                    _lastKey = Keys.W;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {

                    MovePlayer(Direction.UpLeft);
                    _lastKey = Keys.A;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    MovePlayer(Direction.DownRight);
                    _lastKey = Keys.D;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    MovePlayer(Direction.DownLeft);
                    _lastKey = Keys.S;
                }
            }
            base.HandleKey();
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
            _environment.Add(new TileObject(_content, 128, 192));
            _environment[2].AddAnimation("column", new Vector2(0, 0));

            // Create list of actors
            _actors = new List<ActorView>();
            TileObject player = new TileObject(_content, 96, 96);
            player.AddAnimation("Stand", new Vector2(0, 0), -1, 1, 1);


            player.AddAnimation("Walk", new Vector2(0, 192), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 576), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 480), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 288), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 768), -1, 8, 1);


            _actors.Add(new ActorView(spriteBatch, "Player", true, new Vector2(1, 1), player));
            _background = _content.Load<Texture2D>("Minimap");
        }
        #endregion

    }
}
