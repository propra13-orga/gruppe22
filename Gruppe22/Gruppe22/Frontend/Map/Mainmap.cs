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
    /// Enumeration of eight ways of movement
    /// </summary>
    public enum Direction
    {
        Up = 0,
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
        private List<TileSet> _environment;
        /// <summary>
        /// List of actors on the map
        /// </summary>
        private List<ActorView> _actors;
        /// <summary>
        /// Internal reference to map data to be displayed
        /// </summary>
        private Map _map;
        /// <summary>
        /// Counter for last updated (to avoid repeated execution of keypresses, etc.)
        /// </summary>
        private int _lastCheck = 0;
        /// <summary>
        /// Number of tiles to render (Square with player in center)
        /// </summary>
        private int _renderScope = 7;
        /// <summary>
        /// Basic texture set (for drawing lines
        /// </summary>
        private Texture2D _background = null;
        /// <summary>
        /// The circle of light surrounding the player
        /// </summary>
        private Texture2D _circle = null;
        /// <summary>
        /// The tile currently hightlighted by the mouse pointer
        /// </summary>
        private Coords _highlightedTile;
        /// <summary>
        /// A tileset containing walls for all possible directions
        /// </summary>
        private WallTiles _walls;
        #endregion


        #region Public Methods


        /// <summary>
        /// Draw the Map
        /// </summary>
        public override void Draw(GameTime gametime)
        {

            // Rasterizer: Enable cropping at borders (otherwise map would be overlapping everything else)
            RasterizerState rstate = new RasterizerState();
            rstate.ScissorTestEnable = true;

            // Blendstate used for light circle / fog of war
            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;


            // Draw border of window (black square in white square)
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, _displayRect, new Rectangle(39, 6, 1, 1), Color.White);
            _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 2, _displayRect.Y + 2, _displayRect.Width - 4, _displayRect.Height - 4), new Rectangle(39, 6, 1, 1), Color.Black);
            _spriteBatch.End();


            try // This might throw exceptions, so be careful to avoid memory leaks
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);

                _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 10, _displayRect.Height - 10);

                _drawFloor(_map.width, _map.height); // Draw the floow
                _drawWalls(gametime); // Draw walls, other objects, player and enemies

                _spriteBatch.End();


                // Draw circle of light / fog of war
                _spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);
                _spriteBatch.Draw(_circle, new Rectangle(
                    (int)(_actors[0].position.x) - 1060,
                    (int)(_actors[0].position.y) - 1200, 2300, 2300), Color.White);
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
        /// Highlight tile based on mouse position; note inverted matrix (since map is zoomed / panned)
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

                case WallDir.DiagUpDownClose: // Done
                    _drawWall(WallDir.DiagUpClose, target, transparent);
                    _drawWall(WallDir.DiagDownClose, target, transparent);
                    break;

                case WallDir.DiagUpDownClose2: // Done
                    _drawWall(WallDir.DiagUpClose2, target, transparent);
                    _drawWall(WallDir.DiagDownClose2, target, transparent);
                    break;

                case WallDir.DiagLeftRightClose: // Done
                    _drawWall(WallDir.DiagRightClose, target, transparent);
                    _drawWall(WallDir.DiagLeftClose, target, transparent);
                    break;

                case WallDir.DiagLeftRightClose2: // Done
                    _drawWall(WallDir.DiagRightClose2, target, transparent);
                    _drawWall(WallDir.DiagLeftClose2, target, transparent);
                    break;

                case WallDir.None:
                    break;

                default:
                    _spriteBatch.Draw(_walls[(int)dir].animationTexture, new Rectangle(target.Left + _walls[(int)dir].offsetX, target.Top + _walls[(int)dir].offsetY,
    target.Width - _walls[(int)dir].offsetX - _walls[(int)dir].cropX, target.Height - _walls[(int)dir].offsetY - _walls[(int)dir].cropY
    ), _walls[(int)dir].animationRect, transparent ? new Color(Color.White, (float)0.5) : Color.White);
                    break;

            }
        }

        /// <summary>
        /// Determine wall style to use depending on surrounding squares
        /// </summary>
        /// <param name="x">horizontal coordinate of square to check</param>
        /// <param name="y">vertical coordinate of square to check</param>
        /// <returns>A direction to be used for the wall</returns>
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
        private Coords _map2screen(Coords mapC, bool tall = false)
        {

            return new Coords((int)mapC.x * 64 + ((int)mapC.y) * 64
                                    , (int)mapC.y * 48 - (int)mapC.x * 48);
        }


        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Coords _screen2map(Coords screenC, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!
            //     screenC.x -= 32;
            //     screenC.y -= 48;
            return new Coords((int)(screenC.x / 128 - screenC.y / 96)
                                    , (int)(screenC.x / 128 + screenC.y / 96));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Coords _map2screen(int x, int y, bool tall = false)
        {

            return new Coords(x * 64 + y * 64
                                    , y * 48 - x * 48);
        }


        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Coords _screen2map(int x, int y, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!
            //    x -= 32;
            //  y -= 48;
            return new Coords((int)(x / 128 - y / 96)
                                    , (int)(x / 128 + y / 96));
        }

        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        private Coords _pos2Tile(Vector2 coords, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!
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
            Coords currentPos = _screen2map(_actors[0].position.x, _actors[0].position.y);

            for (int y = (Math.Max(currentPos.y - _renderScope, 0)); y <= (Math.Min(currentPos.y + _renderScope, _map.height)); ++y)
            {
                for (int x = (Math.Min(currentPos.x + _renderScope, _map.width)); x >= (Math.Min(currentPos.x - _renderScope, 0)); --x)
                {
                    _drawWall(GetWallStyle(x, y), _tileRect(new Vector2(x + 1, y - 1), true), false);

                    foreach (ActorView actor in _actors)
                    {
                        currentPos = _pos2Tile(new Vector2(actor.position.x, actor.position.y));
                        if (((int)currentPos.x == x) && ((int)currentPos.y == y))
                        {

                            _spriteBatch.Draw(actor.animationTexture,
                                new Rectangle(actor.position.x + actor.offsetX, actor.position.y + actor.offsetY,
    actor.width - actor.offsetX - actor.cropX, actor.height - actor.offsetY - actor.cropY
    ), actor.animationRect, Color.White);
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
            Coords currentPos = _screen2map(_actors[0].position.x, _actors[0].position.y);
            for (int y = (Math.Max(currentPos.y - _renderScope, 0)); y <= (Math.Min(currentPos.y + _renderScope, vTiles)); ++y)
            {
                for (int x = (Math.Max(currentPos.x - _renderScope, 0)); x <= (Math.Min(currentPos.x + _renderScope, hTiles)); ++x)
                {
                    if ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x))
                    {
                        _spriteBatch.Draw(_environment[0][0].animationTexture, _tileRect(new Vector2(x, y)), new Rectangle(512, 384, 128, 96), Color.Red);
                    }
                    else
                    {
                        _spriteBatch.Draw(_environment[0][0].animationTexture, _tileRect(new Vector2(x, y)), new Rectangle(512, 384, 128, 96), Color.White);

                    }
                }
            }
            //TODO: Reimplement rugged tiles
        }

        /// <summary>
        /// Move camera, react to mouse
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Coords currentPos = _screen2map(_actors[0].target.x, _actors[0].target.y);

            if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _UpdateMouse(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!_actors[0].isMoving)
                    {
                        if (_highlightedTile.x < currentPos.x)
                        {
                            if (_highlightedTile.y < currentPos.y)
                            {
                                MovePlayer(Direction.UpLeft);
                            }
                            else
                            {
                                if (_highlightedTile.y > currentPos.y)
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
                            if (_highlightedTile.x > currentPos.x)
                            {

                                if (_highlightedTile.y < currentPos.y)
                                {
                                    MovePlayer(Direction.UpRight);
                                }
                                else
                                {
                                    if (_highlightedTile.y > currentPos.y)
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
                                if (_highlightedTile.y < currentPos.y)
                                {
                                    MovePlayer(Direction.Up);
                                }
                                else
                                {
                                    if (_highlightedTile.y > currentPos.y)
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
                _camera.position = new Vector2(-38 - _actors[0].position.x, -30 - _actors[0].position.y);
            if (Math.Abs(gameTime.TotalGameTime.Milliseconds / 10 - _lastCheck) > 1)
            {
                _lastCheck = gameTime.TotalGameTime.Milliseconds / 10;
                foreach (ActorView actor in _actors)
                {
                    actor.Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Disable moving map by mouse drag to avoid conflicts with move by click
        /// </summary>
        /// <param name="difference"></param>
        /// <param name="_lastCheck"></param>
        public override void MoveContent(Vector2 difference, int _lastCheck = 0)
        {

        }

        /// <summary>
        /// Check whether player can move to a certain square from current position
        /// </summary>
        /// <param name="dir">Direction to move to</param>
        public void MovePlayer(Direction dir)
        {
            // TODO: This should be somewhere in the game logic, not in the map
            Coords currentPos = _screen2map(_actors[0].target.x, _actors[0].target.y);
            switch (dir)
            {
                case Direction.Left:
                    if ((currentPos.x > 0) && (_map[currentPos.x - 1, currentPos.y].canEnter))
                        _actors[0].target = _map2screen(currentPos.x - 1, currentPos.y);
                    break;
                case Direction.Right:
                    if ((currentPos.x < _map.width - 1) && (_map[currentPos.x + 1, currentPos.y].canEnter))
                        _actors[0].target = _map2screen(currentPos.x + 1, currentPos.y);
                    break;
                case Direction.Down:
                    if ((currentPos.y < _map.height - 1) && (_map[currentPos.x, currentPos.y + 1].canEnter))
                        _actors[0].target = _map2screen(currentPos.x, currentPos.y + 1);
                    break;
                case Direction.Up:
                    if ((currentPos.y > 0) && (_map[currentPos.x, currentPos.y - 1].canEnter))
                        _actors[0].target = _map2screen(currentPos.x, currentPos.y - 1);
                    break;

                // Diagonal movement
                // TODO: Check for diagonal walls - doh :-(
                case Direction.DownLeft:
                    if ((currentPos.x > 0) && (currentPos.y < _map.height - 1) && (_map[currentPos.x - 1, currentPos.y + 1].canEnter)
                        )
                        _actors[0].target = _map2screen(currentPos.x - 1, currentPos.y + 1);
                    break;
                case Direction.UpRight:
                    if ((currentPos.x < _map.width - 1) && (currentPos.y > 0) &&
                        (_map[currentPos.x + 1, currentPos.y - 1].canEnter)
                        )
                        _actors[0].target = _map2screen(currentPos.x + 1, currentPos.y - 1);
                    break;
                case Direction.DownRight:
                    if ((currentPos.y < _map.height - 1) && (currentPos.x < _map.width - 1) &&
                        (_map[currentPos.x + 1, currentPos.y + 1].canEnter))
                        _actors[0].target = _map2screen(currentPos.x + 1, currentPos.y + 1);
                    break;
                case Direction.UpLeft:
                    if ((currentPos.y > 0) &&
                        (currentPos.x > 0) &&
                        (_map[currentPos.x - 1, currentPos.y - 1].canEnter))
                        _actors[0].target = _map2screen(currentPos.x - 1, currentPos.y - 1);
                    break;
            }
        }




        /// <summary>
        /// Temporary function - work on dynamic tileset description syntax
        /// </summary>
        public void CreateTextureList()
        {
            /*
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
            _tiles.Add("Column", WallDir.Free, new Rectangle(1920, 0, 128, 192)); */
            WallTiles _tiles = new WallTiles(_content, 128, 192, "");
            _tiles.Load();
            _tiles.Save("neu.xml");
        }

        /// <summary>
        /// React to keypress
        /// </summary>
        /// <param name="_lastCheck"></param>
        public override void HandleKey(int _lastCheck = -1)
        {
            // TODO: This should be in mainwindow (as it should not depend on focus)
            if (!_actors[0].isMoving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {

                    MovePlayer(Direction.Up);

                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {

                    MovePlayer(Direction.Left);

                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    MovePlayer(Direction.Right);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    MovePlayer(Direction.Down);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    MovePlayer(Direction.UpLeft);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    MovePlayer(Direction.UpRight);
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Y))
                {
                    MovePlayer(Direction.DownLeft);
                }



                if (Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    MovePlayer(Direction.DownRight);
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
            _background = _content.Load<Texture2D>("Minimap");
            _circle = _content.Load<Texture2D>("Light");
            _highlightedTile = new Coords(-1, -1);

            // Load textures to use in environment

            // 1. Walls
            _walls = new WallTiles(_content, 128, 192, "");
            _walls.Load("Content\\wall1.xml");

            // 2. Environmental objects (floor, items, traps, teleporters, chest...)
            _environment = new List<TileSet>();
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[0].Add("floor", 0, new Rectangle(512, 384, 128, 96));
            _environment[0].Save("Content\\floor.xml");
            _environment[0].Load("Content\\floor.xml");
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[1].Save("Content\\items.xml");
            _environment[1].Load("Content\\items.xml");
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[2].Save("Content\\spikefield.xml");
            _environment[2].Load("Content\\spikefield.xml");
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[3].Save("Content\\field.xml");
            _environment[3].Load("Content\\field.xml");
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[4].Save("Content\\chest.xml");
            _environment[4].Load("Content\\chest.xml");

            // 3. Moving entities (player, NPCs, enemies)
            _actors = new List<ActorView>();
            Rectangle tilerect = _tileRect(new Vector2(1, 1), true);
            _actors.Add(new ActorView(_content, _map2screen(1, 1)));
            _actors[0].Add(Activity.Walk, Direction.DownRight, "Walk", new Coords(0, 0), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.UpRight, "Walk", new Coords(0, 96), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.Right, "Walk", new Coords(0, 192), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.Up, "Walk", new Coords(0, 288), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.DownLeft, "Walk", new Coords(0, 384), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.Down, "Walk", new Coords(0, 480), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.Left, "Walk", new Coords(0, 576), 8, 1);
            _actors[0].Add(Activity.Walk, Direction.UpLeft, "Walk", new Coords(0, 672), 8, 1);
            _actors[0].Save("Content\\player.xml");

            //    _actors[0].Load("Content\\player.xml");
            //  _actors.Add(new ActorView(_content, new Vector2(20, 20)));
            // _actors[1].Load("Content\\skeleton.xml");


        }
        #endregion

    }
}
