﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
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
        private int _renderScope = 7;
        private Texture2D _background = null;
        private Keys _lastKey = Keys.None;
        private Texture2D _circle = null;
        private Coords _highlightedTile;
        private List<Vector2> _path;
        #endregion


        #region Public Methods


        /// <summary>
        /// Draw the Map
        /// </summary>
        public override void Draw(GameTime gametime)
        {
            RasterizerState rstate = new RasterizerState();
            rstate.ScissorTestEnable = true;
            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;
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

                _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 10, _displayRect.Height - 10);
                _drawFloor(_map.width, _map.height);

                _drawWalls(gametime);
                _spriteBatch.End();


                _spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);
                _spriteBatch.Draw(_circle, new Rectangle(
                    (int)(_actors[0].position.X * 64 + _actors[0].position.Y * 64) - 1060,
                    (int)(_actors[0].position.Y * 48 - _actors[0].position.X * 48) - 1200, 2300, 2300), Color.White);


                /* _spriteBatch.Draw(_background, new Rectangle( // von oben
                                (int)(_actors[0].position.X * 64 + _actors[0].position.Y * 64) - 980,
                                (int)(_actors[0].position.Y * 48 - _actors[0].position.X * 48) - 2000, 1500, 1500), new Rectangle(0, 0, 1, 1), Color.White);
                            _spriteBatch.Draw(_background, new Rectangle( // von unten
                                (int)(_actors[0].position.X * 64 + _actors[0].position.Y * 64) - 980,
                                (int)(_actors[0].position.Y * 48 - _actors[0].position.X * 48) + 500, 1500, 1500), new Rectangle(0, 0, 1, 1), Color.White);
                            _spriteBatch.Draw(_background, new Rectangle(
                                (int)(_actors[0].position.X * 64 + _actors[0].position.Y * 64) - 1580,
                                (int)(_actors[0].position.Y * 48 - _actors[0].position.X * 48) - 1000, 1100, 3000), new Rectangle(0, 0, 1, 1), Color.White);
                            _spriteBatch.Draw(_background, new Rectangle(
                                (int)(_actors[0].position.X * 64 + _actors[0].position.Y * 64) + 520,
                                (int)(_actors[0].position.Y * 48 - _actors[0].position.X * 48) - 1000, 1500, 3000), new Rectangle(0, 0, 1, 1), Color.White);*/

                _spriteBatch.End();


                _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            }
            finally
            {
                rstate.Dispose();
                blendState.Dispose();
            }

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        private void _UpdateMouse(Vector2 coords)
        {
            Vector2 realPos = Vector2.Transform(coords, Matrix.Invert(_camera.matrix));
            _highlightedTile = _pos2Tile(realPos);
        }

        /// <summary>
        /// Display a wall
        /// </summary>
        /// <param name="dir">Squares the wall connects to</param>
        /// <param name="x">Horizontal position</param>
        /// <param name="y">Vertical position</param>
        /// <param name="transparent"></param>
        private void _drawWall(WallDir dir, Rectangle target, bool transparent)
        {
            switch (dir)
            {
                case WallDir.UpRight: // Walls on Up and left square
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpLeft: // Walls on up and right square
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownLeft: // Walls on left and down squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownRight: // Walls on right and down squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRight: // Walls on left and right neighboring squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDown:// Walls on up and down neighboring squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.FourWay: // Walls on all surrounding squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.RightClose:
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpClose:
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftClose: // Wall on current square connected to square to the left
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownClose: // Wall on current square connected to square below
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRightUp: // Walls connected left, right and up
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRightDown: // Wall connected left right and down
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDownLeft: // Walls on Up, Down and Left suqares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDownRight: // Walls on Up, Down and Right squares
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 576, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.Free: // Free standing wall (no connecting squares)
                    _spriteBatch.Draw(_environment[2].animationTexture, target, new Rectangle(1920, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(681, 835, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(321, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(384, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(128, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDownLeftDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDownDiag:// Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(0, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.FourDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 768, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.RightCloseDiag: // Done (Imperfect)
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(681, 820, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(257, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(385, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DownCloseDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(136, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRightUpDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRightDownDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.LeftRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(256, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.UpDownRightDiag: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 384, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagUpClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagUpDownClose: // Done
                    _drawWall(WallDir.DiagUpClose, target, transparent);
                    _drawWall(WallDir.DiagDownClose, target, transparent);
                    break;

                case WallDir.DiagDownClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagUpClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagUpDownClose2: // Done
                    _drawWall(WallDir.DiagUpClose2, target, transparent);
                    _drawWall(WallDir.DiagDownClose2, target, transparent);
                    break;

                case WallDir.DiagDownClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagLeftClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(640, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagLeftRightClose: // Done
                    _drawWall(WallDir.DiagRightClose, target, transparent);
                    _drawWall(WallDir.DiagLeftClose, target, transparent);
                    break;

                case WallDir.DiagRightClose: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(896, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagLeftClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(512, 0, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.DiagLeftRightClose2: // Done
                    _drawWall(WallDir.DiagRightClose2, target, transparent);
                    _drawWall(WallDir.DiagLeftClose2, target, transparent);
                    break;

                case WallDir.DiagRightClose2: // Done
                    _spriteBatch.Draw(_environment[0].animationTexture, target, new Rectangle(768, 192, 128, 192), transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

                case WallDir.None: // No wall
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public WallDir GetWallStyle(int x = 0, int y = 0)
        {
            if (_map[x, y].canEnter) return WallDir.None;


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
                                            return WallDir.FourDiag;
                                        }
                                        else // (not down left) Down Right +Up Right + Down Left 
                                        {
                                            return WallDir.LeftRightDownDiag;
                                        }
                                    }
                                    else // (not down left)
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right  + Up right + Up Left diagonal (not up right)
                                        {
                                            return WallDir.LeftRightUpDiag;
                                        }
                                        else // Down Right  + Up right diagonal (not up right, up left)
                                        {
                                            return WallDir.LeftRightDiag;
                                        }
                                    }
                                }
                                else // Not up right
                                {
                                    if (!_map[x - 1, y + 1].canEnter) // Down Right  + Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right + Down Left + Up Left diagonal
                                        {
                                            return WallDir.UpDownLeftDiag;
                                        }
                                        else // Down Right + Down Left diagonal 
                                        {
                                            return WallDir.UpDownDiag;

                                        }
                                    }
                                    else // Not down left
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Right + Up Left diagonal
                                        {
                                            return WallDir.UpLeftDiag;
                                        }
                                        else // Not up left: Down right only
                                        {
                                            return WallDir.UpCloseDiag;
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
                                            return WallDir.UpDownRightDiag;
                                        }
                                        else // Up Right + Down Left 
                                        {
                                            return WallDir.UpRightDiag;
                                        }
                                    }
                                    else
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Up Right + Up Left diagonal
                                        {
                                            return WallDir.DownLeftDiag;
                                        }
                                        else
                                        {
                                            return WallDir.DownCloseDiag;
                                        }
                                    }
                                }
                                else // not up right
                                {

                                    if (!_map[x - 1, y + 1].canEnter) //  Down Left diagonal
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) // Down Left + Up Left diagonal
                                        {
                                            return WallDir.DownRightDiag;
                                        }
                                        else
                                        {
                                            return WallDir.RightCloseDiag;
                                        }
                                    }
                                    else
                                    {
                                        if (!_map[x - 1, y - 1].canEnter) //  Up Left diagonal
                                        {
                                            return WallDir.LeftCloseDiag;
                                        }
                                        else
                                        {
                                            return WallDir.Free; // Keine Mauer weit und breit?
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
                                    return WallDir.DiagUpDownClose2;
                                }
                                else
                                {
                                    return WallDir.DiagUpClose2;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return WallDir.DiagDownClose2;
                                }
                                else
                                {
                                    return WallDir.DownClose;
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
                                    return WallDir.DiagUpDownClose;
                                }
                                else
                                {
                                    return WallDir.DiagUpClose;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y + 1].canEnter)
                                {
                                    return WallDir.DiagDownClose;
                                }
                                else
                                {
                                    return WallDir.UpClose;
                                }
                            }

                        }
                        else // Wall up and down
                        {
                            // Wall on current square and squares above and below
                            return WallDir.UpDown;
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
                                    return WallDir.DiagLeftRightClose2;
                                }
                                else
                                {
                                    return WallDir.DiagLeftClose2;
                                }
                            }
                            else
                            {
                                if (!_map[x - 1, y - 1].canEnter)
                                {
                                    return WallDir.DiagRightClose2;
                                }
                                else
                                {
                                    return WallDir.RightClose;
                                }
                            }
                        }
                        else // Wall down
                        {
                            // Wall right and down, but not left and up
                            return WallDir.DownRight;
                        }
                    }
                    else // Wall up
                    {
                        // Wall up and right, but not left
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Wall up, right, but not left and down
                            return WallDir.UpRight;
                        }
                        else // Wall down
                        {
                            // Wall up, right and down, but not left
                            return WallDir.UpDownRight;
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
                                    return WallDir.DiagLeftRightClose;
                                }
                                else
                                {
                                    return WallDir.DiagLeftClose;
                                }
                            }
                            else
                            {
                                if (!_map[x + 1, y - 1].canEnter)
                                {
                                    return WallDir.DiagRightClose;
                                }
                                else
                                {
                                    return WallDir.LeftClose;
                                }
                            }
                        }
                        else  // Wall down
                        {
                            // Left and bottom closed
                            return WallDir.DownLeft;
                        }
                    }
                    else // Wall up
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // Left and Up closed
                            return WallDir.UpLeft;
                        }
                        else // Wall down
                        {
                            // Left, Up and Down closed
                            return WallDir.UpDownLeft;
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
                            return WallDir.LeftRight;
                        }
                        else // wall up
                        {
                            // All walls but not up
                            return WallDir.LeftRightDown;

                        }
                    }
                    else
                    {
                        if (_map[x, y + 1].canEnter) // No wall down
                        {
                            // All walls but not down
                            return WallDir.LeftRightUp;
                        }
                        else // wall down
                        {
                            // Surrounded by walls
                            return WallDir.FourWay;
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
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Coords _pos2Tile(Vector2 coords, bool tall = false)
        {
            coords.X -= 32;
            coords.Y -= 48;
            return new Coords((int)(coords.X / 128 - coords.Y / 96)
                                    , (int)(coords.X / 128 + coords.Y / 96));
        }

        /// <summary>
        /// Display all walls on the current map
        /// </summary>
        private void _drawWalls(GameTime gametime)
        {

            for (int y = (Math.Max((int)_actors[0].position.Y - _renderScope, 0)); y <= (Math.Min((int)_actors[0].position.Y + _renderScope, _map.height)); ++y)
            {
                for (int x = (Math.Min((int)_actors[0].position.X + _renderScope, _map.width)); x >= (Math.Max((int)_actors[0].position.X - _renderScope, 0)); --x)
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
            for (int y = (Math.Max((int)_actors[0].position.Y - _renderScope, 0)); y <= (Math.Min((int)_actors[0].position.Y + _renderScope, vTiles)); ++y)
            {
                for (int x = (Math.Max((int)_actors[0].position.X - _renderScope, 0)); x <= (Math.Min((int)_actors[0].position.X + _renderScope, hTiles)); ++x)
                {
                    if ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x))
                    {
                        _spriteBatch.Draw(_environment[1].animationTexture, _tileRect(new Vector2(x, y)), new Rectangle(512, 384, 128, 96), Color.Red);
                    }
                    else
                    {
                        _spriteBatch.Draw(_environment[1].animationTexture, _tileRect(new Vector2(x, y)), new Rectangle(512, 384, 128, 96), Color.White);

                    }
                }
            }
            //TODO: Reimplement rugged tiles
        }

        public override void Update(GameTime gameTime)
        {
            if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _UpdateMouse(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!_actors[0].isMoving)
                    {
                        if (_highlightedTile.x < _actors[0].position.X)
                        {
                            if (_highlightedTile.y < _actors[0].position.Y)
                            {
                                MovePlayer(Direction.UpLeft);
                            }
                            else
                            {
                                if (_highlightedTile.y > _actors[0].position.Y)
                                {
                                    MovePlayer(Direction.DownLeft);
                                }
                                else
                                {
                                    MovePlayer(Direction.Left);
                                }
                            }
                        }
                        else
                        {
                            if (_highlightedTile.x > _actors[0].position.X)
                            {

                                if (_highlightedTile.y < _actors[0].position.Y)
                                {
                                    MovePlayer(Direction.UpRight);
                                }
                                else
                                {
                                    if (_highlightedTile.y > _actors[0].position.Y)
                                    {
                                        MovePlayer(Direction.DownRight);
                                    }
                                    else
                                    {
                                        MovePlayer(Direction.Right);
                                    }
                                }
                            }
                            else
                            {
                                if (_highlightedTile.y < _actors[0].position.Y)
                                {
                                    MovePlayer(Direction.Up);
                                }
                                else
                                {
                                    if (_highlightedTile.y > _actors[0].position.Y)
                                    {
                                        MovePlayer(Direction.Down);
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
                case Direction.Left:
                    if (((int)_actors[0].target.X > 0) && (_map[(int)_actors[0].target.X - 1, (int)_actors[0].target.Y].canEnter))
                        _actors[0].Move(new Vector2(-1.0f, 0));
                    break;
                case Direction.Right:
                    if (((int)_actors[0].target.X < _map.width - 1) && (_map[(int)_actors[0].target.X + 1, (int)_actors[0].target.Y].canEnter))
                        _actors[0].Move(new Vector2(1.0f, 0));
                    break;
                case Direction.Down:
                    if (((int)_actors[0].target.Y < _map.height - 1) && (_map[(int)_actors[0].target.X, (int)_actors[0].target.Y + 1].canEnter))
                        _actors[0].Move(new Vector2(0, 1.0f));
                    break;
                case Direction.Up:
                    if (((int)_actors[0].target.Y > 0) && (_map[(int)_actors[0].target.X, (int)_actors[0].target.Y - 1].canEnter))
                        _actors[0].Move(new Vector2(0, -1.0f));
                    break;

                // Diagonal movement
                // TODO: Check for diagonal walls - doh :-(
                case Direction.DownLeft:
                    if (((int)_actors[0].target.X > 0) && ((int)_actors[0].target.Y < _map.height - 1) && (_map[(int)_actors[0].target.X - 1, (int)_actors[0].target.Y + 1].canEnter)
                        )
                        _actors[0].Move(new Vector2(-1.0f, 1.0f));
                    break;
                case Direction.UpRight:
                    if (((int)_actors[0].target.X < _map.width - 1) && ((int)_actors[0].target.Y > 0) &&
                        (_map[(int)_actors[0].target.X + 1, (int)_actors[0].target.Y - 1].canEnter)
                        )
                        _actors[0].Move(new Vector2(1.0f, -1.0f));
                    break;
                case Direction.DownRight:
                    if (((int)_actors[0].target.Y < _map.height - 1) && ((int)_actors[0].target.X < _map.width - 1) &&
                        (_map[(int)_actors[0].target.X + 1, (int)_actors[0].target.Y + 1].canEnter))
                        _actors[0].Move(new Vector2(1.0f, 1.0f));
                    break;
                case Direction.UpLeft:
                    if (((int)_actors[0].target.Y > 0) &&
                        ((int)_actors[0].target.X > 0) &&
                        (_map[(int)_actors[0].target.X - 1, (int)_actors[0].target.Y - 1].canEnter))
                        _actors[0].Move(new Vector2(-1.0f, -1.0f));
                    break;
            }
        }

        public override void MoveContent(Vector2 difference, int _lastCheck = 0)
        {
            // if (!_actors[0].isMoving) 
            base.MoveContent(difference);
        }


        /// <summary>
        /// Temporary function - work on dynamic tileset description syntax
        /// </summary>
        public void CreateTextureList()
        {
            WallTiles _tiles = new WallTiles(_content, 128, 192, "");
            _tiles.Add("Wall1", WallDir.UpRight, new Rectangle(0, 768, 128, 192));
            _tiles.Add("Wall1", WallDir.UpLeft, new Rectangle(128, 768, 128, 192));
            _tiles.Add("Wall1", WallDir.DownLeft,
                    new Rectangle(256, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.DownRight, new Rectangle(384, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRight, new Rectangle(0, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDown, new Rectangle(128, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.FourWay, new Rectangle(384, 768, 128, 192));
            _tiles.Add("Wall1", WallDir.RightClose, new Rectangle(256, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.UpClose, new Rectangle(128, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftClose, new Rectangle(384, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.DownClose, new Rectangle(0, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRightUp,
            new Rectangle(640, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRightDown,
            new Rectangle(768, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDownLeft,
            new Rectangle(896, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDownRight,
            new Rectangle(512, 576, 128, 192));
            _tiles.Add("Wall1", WallDir.UpRightDiag,
            new Rectangle(681, 835, 128, 192));
            _tiles.Add("Wall1", WallDir.UpLeftDiag,
            new Rectangle(321, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DownLeftDiag,
            new Rectangle(384, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.DownRightDiag,
            new Rectangle(128, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDownLeftDiag,
            new Rectangle(640, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDownDiag,
            new Rectangle(0, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.FourDiag,
            new Rectangle(256, 768, 128, 192));
            _tiles.Add("Wall1", WallDir.RightCloseDiag,
            new Rectangle(681, 820, 128, 192));
            _tiles.Add("Wall1", WallDir.UpCloseDiag,
            new Rectangle(257, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftCloseDiag,
            new Rectangle(385, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DownCloseDiag,
            new Rectangle(136, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRightUpDiag,
            new Rectangle(896, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRightDownDiag,
            new Rectangle(768, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.LeftRightDiag,
            new Rectangle(256, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.UpDownRightDiag,
            new Rectangle(512, 384, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagUpClose,
            new Rectangle(640, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagDownClose,
            new Rectangle(896, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagUpClose2,
            new Rectangle(512, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagDownClose2,
            new Rectangle(768, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagLeftClose,
            new Rectangle(640, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagRightClose,
            new Rectangle(896, 192, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagLeftClose2,
            new Rectangle(512, 0, 128, 192));
            _tiles.Add("Wall1", WallDir.DiagRightClose2,
            new Rectangle(768, 192, 128, 192));
            _tiles.Add("Column", WallDir.Free, new Rectangle(1920, 0, 128, 192));
            _tiles.Save();
        }

        public override void HandleKey(int _lastCheck = -1)
        {

            if (!_actors[0].isMoving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {

                    MovePlayer(Direction.Up);
                    _lastKey = Keys.W;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {

                    MovePlayer(Direction.Left);
                    _lastKey = Keys.A;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    MovePlayer(Direction.Right);
                    _lastKey = Keys.D;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    MovePlayer(Direction.Down);
                    _lastKey = Keys.S;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    MovePlayer(Direction.UpLeft);
                    _lastKey = Keys.Q;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    MovePlayer(Direction.UpRight);
                    _lastKey = Keys.E;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Y))
                {
                    MovePlayer(Direction.DownLeft);
                    _lastKey = Keys.Y;
                }



                if (Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    MovePlayer(Direction.DownRight);
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


            player.AddAnimation("Walk", new Vector2(0, 0), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 96), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 384), -1, 8, 1);

            player.AddAnimation("Walk", new Vector2(0, 672), -1, 8, 1);


            _actors.Add(new ActorView(spriteBatch, "Player", true, new Vector2(1, 1), player));
            _background = _content.Load<Texture2D>("Minimap");
            _circle = _content.Load<Texture2D>("Light");
            _highlightedTile = new Coords(-1, -1);
            //CreateTextureList();
        }
        #endregion

    }
}
