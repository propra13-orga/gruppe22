using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    public class MapEffect
    {
        private TileObject _animation;
        private Backend.Coords _position;
        private bool _finished = false;
        private int _count = 0;

        public bool finished
        {
            get
            {
                return _finished;
            }
        }
        public Backend.Coords position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public void Update(GameTime time)
        {
            _count += time.ElapsedGameTime.Milliseconds;
            if (_count > 80)
            {
                _count -= 80;
                _finished = _finished || _animation.NextAnimation();
                //  System.Diagnostics.Debug.WriteLine(_finished+ " "+_animation.animationID);

            }
        }

        public void Draw(SpriteBatch _spritebatch, GameTime time)
        {
            _spritebatch.Draw(_animation.animationTexture, new Rectangle(_position.x, _position.y, _animation.animationRect.Width, _animation.animationRect.Height), _animation.animationRect, Color.White);
        }

        public MapEffect(TileObject animation, Backend.Coords position, int scale = 3)
        {
            _position = position;
            _animation = animation;
        }
    }

    public class Projectile
    {
        private uint _id = 0;
        private Backend.Coords _current;
        private Backend.Coords _target;
        private ProjectileTile _tile;
        private Backend.Direction _dir;
        public uint _elapsed = 0;
        public bool _nomove = false;
        public Backend.IHandleEvent _parent;

        public uint id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public int direction
        {
            get
            {
                return (int)_dir;
            }
        }
        public ProjectileTile tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = tile;
            }
        }

        public void moveTo(Backend.Coords coord)
        {
            _target = Mainmap._map2screen(coord);
        }

        public void Draw()
        {

        }
        public void Update(GameTime gametime)
        {
            _elapsed += (uint)gametime.ElapsedGameTime.Milliseconds;

            if (_target != _current)
            {
                if (_elapsed > 10)
                {

                    if (!_nomove)
                    {
                        _nomove = true;
                    }
                    else
                    {
                        _nomove = false;
                    }

                    _elapsed -= 10;
                    // System.Diagnostics.Debug.WriteLine(_current + " " + _target);
                    if (_target.x > _current.x)
                    {
                        //   if (_id == 0) System.Diagnostics.Debug.Write(_xpertick.ToString());

                        _current.x += 4;

                    }
                    else
                    {
                        if (_target.x < _current.x)
                        {
                            //     if (_id == 0) System.Diagnostics.Debug.Write(-_xpertick);

                            _current.x -= 4;

                        }
                        //else
                        //     if (_id == 0) System.Diagnostics.Debug.Write("0");
                    }



                    if (_target.y > _current.y)
                    {
                        //   if (_id == 0) System.Diagnostics.Debug.WriteLine("/" + _ypertick.ToString());
                        _current.y += 3;
                    }
                    else
                        if (_target.y < _current.y)
                        {
                            // if (_id == 0) System.Diagnostics.Debug.WriteLine("/-" + _ypertick.ToString());

                            _current.y -= 3;
                        }
                        else
                        {
                            //  if (_id == 0) System.Diagnostics.Debug.WriteLine("/0");
                        }

                    if (_target == _current)
                    {
                        //                            _position = _target;
                        //                          _target = _cacheTarget;
                        _parent.HandleEvent(false, Backend.Events.FinishedProjectileMove, _tile, Mainmap._pos2Tile(_current.vector));



                    }
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch, TileObject animation)
        {
            _spriteBatch.Draw(animation.animationTexture, new Rectangle(_current.x + 48, _current.y + 16, animation.animationRect.Width, animation.animationRect.Height), animation.animationRect, Color.White);
        }

        public Projectile(uint id, Backend.IHandleEvent parent, Backend.Coords current, Backend.Direction dir, ProjectileTile tile)
        {
            _dir = dir;
            _id = id;
            _tile = tile;
            _current = Mainmap._map2screen(current);
            _target = Mainmap._map2screen(current);
            //     System.Diagnostics.Debug.WriteLine("Start at" + _current);
            _parent = parent;
        }
    }
    public class FloatNumber
    {
        Vector2 _pos;
        private float _counter = 10;
        private Color _color = Color.White;
        private string _text;
        private SpriteBatch _spritebatch;
        float _width = 0;
        float _height = 0;
        private SpriteFont _font;
        private int _timer = 0;
        private Camera _camera;
        private uint _delay = 0;

        public uint delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
            }
        }
        public void Draw()
        {
            if (_delay == 0)
            {
                _spritebatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, _camera.matrix);
                _spritebatch.DrawString(_font, _text, new Vector2(_pos.X, _pos.Y - (20 * (10 - _counter))), new Color(_color, (float)_counter / 10), 0f, new Vector2(_width / 2, _height / 2), (10 - _counter) / 3, SpriteEffects.None, 0);
                _spritebatch.End();
            }
        }

        public bool Update(GameTime gametime)
        {
            _timer += gametime.ElapsedGameTime.Milliseconds;
            if (_timer > 10)
            {
                _timer -= 10;
                if (_delay == 0)
                {
                    _counter -= 0.1f;
                    if (_counter < 0.1f) return true;
                }
                else
                {
                    _delay -= 1;
                }
            }
            return false;
        }

        public FloatNumber(ContentManager content, SpriteBatch batch, Backend.Coords coords, string text, Camera camera, Color color, int counter = 10, uint delay = 0)
            : this(content, batch, coords, text, camera)
        {
            _color = color;
            _counter = counter;
            _delay = (uint)delay;
        }

        public FloatNumber(ContentManager content, SpriteBatch batch, Backend.Coords coords, string text, Camera camera)
        {
            _text = text;
            _pos = new Vector2(Mainmap._map2screen(coords).x + 52, Mainmap._map2screen(coords).y - 16);
            _spritebatch = batch;
            _font = content.Load<SpriteFont>("font");
            _height = _font.MeasureString(_text).Y;
            _width = _font.MeasureString(_text).X;
            _camera = camera;
        }

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
        private List<Projectile> _projectiles;
        private uint _maxProjectile;
        private List<MapEffect> _effects;
        private Object _mylock = new Object();
        private uint _fireCount = 0;
        private bool _noMove = true;


        private string _bigText = "";
        private string _smallText = "";
        private int _visibility = -100;
        private int _titleTime = 0;
        /// <summary>
        /// List of actors on the map
        /// </summary>
        private List<ActorView> _actors;
        private bool _enabled = true;
        private int _playerID = 0;
        /// <summary>
        /// Internal reference to map data to be displayed
        /// </summary>
        private Backend.Map _map;

        private TileTooltip _tooltip = null;
        private SpriteFont _font;
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
        private Backend.Coords _highlightedTile;
        /// <summary>
        /// A tileset containing walls for all possible directions
        /// </summary>
        private WallTiles _walls;
        private WallTiles _floors;
        private List<FloatNumber> _floatnumbers = null;
        #endregion

        #region Public Fields

        public List<ActorView> actors
        {
            get
            {
                return _actors;
            }
        }
        public Matrix transformMatrix
        {
            get
            {
                return _camera.matrix;
            }
        }

        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        #endregion
        #region Public Methods

        public void addEffect(int animationID, Backend.Coords pos)
        {
            _effects.Add(new MapEffect(_environment[animationID][0], pos));
        }

        public void DisplaySubtitle(string main = "", string sub = "")
        {
            _titleTime = 300;
            _bigText = main;
            _smallText = sub;
            _visibility = -100;
        }


        public override void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            if (DownStream)
            {
                switch (eventID)
                {
                    case Backend.Events.MoveActor:
                        {
                            int id = (int)data[0];
                            Backend.Coords coords = (Backend.Coords)data[1];
                            _actors[id].target = _map2screen(coords);
                        };
                        break;
                    case Backend.Events.Player1:
                        _actors[0].effect = new MapEffect(_environment[2][2], new Backend.Coords(_actors[0].position.x + 7, _actors[0].position.y + 2));
                        break;
                    case Backend.Events.ExplodeProjectile:
                        if (data[2] != null)
                        {
                            int id = ((Backend.Actor)data[2]).id;
                            _actors[id].effect = new MapEffect(_environment[2][0], new Backend.Coords(_actors[id].position.x + 7, _actors[id].position.y + 2));
                        }
                        else
                        {
                            addEffect(2, _map2screen((Backend.Coords)data[1]));
                        }
                        RemoveProjectile(((ProjectileTile)data[0]).id);

                        break;
                    case Backend.Events.AnimateActor:
                        {
                            int id = (int)data[0];
                            Backend.Activity activity = (Backend.Activity)data[1];
                            bool delay = false;
                            bool isLock = true;
                            if (data.Length > 2) delay = (bool)data[2];
                            if (data.Length > 3) _actors[id].direction = (Backend.Direction)data[3];
                            if ((activity == Backend.Activity.Die) || (activity == Backend.Activity.Hit))
                            {
                                _actors[id].effect = new MapEffect(_environment[2][1], new Backend.Coords(_actors[id].position.x + 7, _actors[id].position.y + 2));
                            }
                            if (delay)
                            {
                                _actors[id].PlayNowOrAfterMove(activity, isLock);
                            }
                            else
                            {
                                _actors[id].EndMoveAndPlay(activity, isLock);
                            }
                            ;
                            /*bool waitForAnim = (bool)data[2];*/
                        }
                        break;
                }
            }
            else
            {
                _parent.HandleEvent(false, eventID, data);
            }

        }

        /// <summary>
        /// Draw the Map
        /// </summary>
        public override void Draw(GameTime gametime)
        {
            if (_enabled)
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
                _spriteBatch.Draw(_background, new Rectangle(_displayRect.X + 1, _displayRect.Y + 1, _displayRect.Width - 2, _displayRect.Height - 2), new Rectangle(39, 6, 1, 1), Color.Black);
                _spriteBatch.End();

                _spriteBatch.Begin(SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);

                _spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 10, _displayRect.Height - 10);

                _drawFloor(); // Draw the floow

                for (int i = 0; i < _projectiles.Count; ++i)
                {
                    _projectiles[i].Draw(_spriteBatch, _environment[1][(int)Math.Log((double)_projectiles[i].direction, 2)]);
                }

                for (int i = 0; i < _effects.Count; ++i)
                {
                    _effects[i].Draw(_spriteBatch, gametime);
                }

                _drawWalls(gametime); // Draw walls, other objects, player and enemies

                _spriteBatch.End();


                // Draw circle of light / fog of war
                _spriteBatch.Begin(SpriteSortMode.Texture, blendState, null,
                            null,
                            rstate,
                            null,
                            _camera.matrix);
                _spriteBatch.Draw(_circle, new Rectangle(
                    (int)(_actors[_playerID].position.x + 1) - 250 * Math.Max(_map.actors[_playerID].viewRange, _map.light),
                    (int)(_actors[_playerID].position.y + 1) - 250 * Math.Max(_map.actors[_playerID].viewRange, _map.light), 520 * Math.Max(_map.actors[_playerID].viewRange, _map.light), 520 * Math.Max(_map.actors[_playerID].viewRange, _map.light)), Color.White);
                _spriteBatch.End();


                _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;

                rstate.Dispose();
                blendState.Dispose();


                if ((_highlightedTile.x > -1)
                    && (_highlightedTile.x >= _map.actors[_playerID].tile.coords.x - Math.Max(_map.actors[_playerID].viewRange, _map.light))
                    && (_highlightedTile.x <= _map.actors[_playerID].tile.coords.x + Math.Max(_map.actors[_playerID].viewRange, _map.light))
                    && (_highlightedTile.y >= _map.actors[_playerID].tile.coords.y - Math.Max(_map.actors[_playerID].viewRange, _map.light))
                    && (_highlightedTile.y <= _map.actors[_playerID].tile.coords.y + Math.Max(_map.actors[_playerID].viewRange, _map.light)))
                    _tooltip.DisplayToolTip(_map[_highlightedTile.x, _highlightedTile.y]);
                for (int i = 0; i < _floatnumbers.Count; ++i)
                {
                    _floatnumbers[i].Draw();
                }

                if (_bigText != "")
                {
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    float opacity = ((float)((_visibility <= 0) ? (100 + _visibility) : (_visibility))) / 100f;
                    _spriteBatch.DrawString(_font, _bigText, new Vector2(_displayRect.Left + 10, _displayRect.Bottom - _font.MeasureString(_bigText).Y - _font.MeasureString(_smallText).Y), new Color(opacity, 0, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    _spriteBatch.DrawString(_font, _smallText, new Vector2(_displayRect.Left + 10, _displayRect.Bottom - _font.MeasureString(_smallText).Y), new Color(opacity, opacity, opacity), 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);

                    _spriteBatch.End();
                }
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
        private void _drawWall(WallDir dir, Rectangle target, bool transparent, bool active, Backend.WallType special = Backend.WallType.Normal)
        {
            if (special == Backend.WallType.OpenDoor)
            {
                System.Diagnostics.Debug.WriteLine("Door");
            }
            if (special == Backend.WallType.ClosedDoor)
            {
                System.Diagnostics.Debug.WriteLine("Door");
            }
            if ((special != Backend.WallType.Normal) && (dir != WallDir.LeftRight) && (dir != WallDir.UpDown) && (dir != WallDir.UpLeftDiag)) special = Backend.WallType.Normal;

            switch (dir)
            {


                case WallDir.DiagUpDownClose: // Done
                    _drawWall(WallDir.DiagUpClose, target, transparent, active);
                    _drawWall(WallDir.DiagDownClose, target, transparent, active);
                    break;

                case WallDir.DiagUpDownClose2: // Done
                    _drawWall(WallDir.DiagUpClose2, target, transparent, active);
                    _drawWall(WallDir.DiagDownClose2, target, transparent, active);
                    break;

                case WallDir.DiagLeftRightClose: // Done
                    _drawWall(WallDir.DiagRightClose, target, transparent, active);
                    _drawWall(WallDir.DiagLeftClose, target, transparent, active);
                    break;

                case WallDir.DiagLeftRightClose2: // Done
                    _drawWall(WallDir.DiagRightClose2, target, transparent, active);
                    _drawWall(WallDir.DiagLeftClose2, target, transparent, active);
                    break;

                case WallDir.None:
                    break;

                default:
                    Color color = active ? Color.Red : Color.White;
                    if ((dir == WallDir.UpLeftDiag) && (special != Backend.WallType.Normal))
                    {

                        // 32 PIXEL LINKS
                        // 32 PIXEL RECHTS
                        _spriteBatch.Draw(_walls[(int)dir].animationTexture, new Rectangle(
                            target.Left + _walls[(int)dir].offsetX,
                            target.Top + _walls[(int)dir].offsetY,
                            target.Width - _walls[(int)dir].offsetX - _walls[(int)dir].cropX - 96,
                            target.Height - _walls[(int)dir].offsetY - _walls[(int)dir].cropY),
                            new Rectangle(_walls[(int)dir].animationRect.Left, _walls[(int)dir].animationRect.Top, _walls[(int)dir].animationRect.Width - 96, _walls[(int)dir].animationRect.Height), transparent ? new Color(color, (float)0.5) : color);
                        _spriteBatch.Draw(_walls[(int)dir].animationTexture, new Rectangle(
    target.Left + _walls[(int)dir].offsetX + 96,
    target.Top + _walls[(int)dir].offsetY,
    target.Width - _walls[(int)dir].offsetX - _walls[(int)dir].cropX - 96,
    target.Height - _walls[(int)dir].offsetY - _walls[(int)dir].cropY),
    new Rectangle(_walls[(int)dir].animationRect.Left, _walls[(int)dir].animationRect.Top, _walls[(int)dir].animationRect.Width - 96, _walls[(int)dir].animationRect.Height), transparent ? new Color(color, (float)0.5) : color);

                    }

                    _spriteBatch.Draw(_walls[(int)dir + (int)special * 100].animationTexture, new Rectangle(
                        target.Left + _walls[(int)dir + (int)special * 100].offsetX,
                        target.Top + _walls[(int)dir + (int)special * 100].offsetY,
                        target.Width - _walls[(int)dir + (int)special * 100].offsetX - _walls[(int)dir + (int)special * 100].cropX,
                        target.Height - _walls[(int)dir + (int)special * 100].offsetY - _walls[(int)dir + (int)special * 100].cropY),
                        _walls[(int)dir + (int)special * 100].animationRect, transparent ? new Color(color, (float)0.5) : color);
                    break;

            }
        }

        public bool noMove
        {
            get
            {
                return _noMove;
            }
            set
            {
                _noMove = value;
            }
        }

        /// <summary>
        /// Determine wall style to use depending on surrounding squares
        /// </summary>
        /// <param name="x">horizontal coordinate of square to check</param>
        /// <param name="y">vertical coordinate of square to check</param>
        /// <returns>A direction to be used for the wall</returns>
        public WallDir GetWallStyle(int x = 0, int y = 0, bool CheckWall = true, int FloorStyle = -1)
        {
            if ((CheckWall && (!_map[x, y].hasWall)) || (!CheckWall && _map[x, y].floorStyle != FloorStyle)) return WallDir.None;


            if ((CheckWall && (!_map[x - 1, y].hasWall)) || (!CheckWall && _map[x - 1, y].floorStyle != FloorStyle)) // No wall left
            {

                // No wall blocks way to left

                if ((CheckWall && (!_map[x + 1, y].hasWall)) || (!CheckWall && _map[x + 1, y].floorStyle != FloorStyle)) // No wall right
                {

                    // No wall blocks way to left or right

                    if ((CheckWall && (!_map[x, y - 1].hasWall)) || (!CheckWall && _map[x, y - 1].floorStyle != FloorStyle))  // No wall up
                    {
                        // No wall blocks way up, left or right

                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle != FloorStyle)) // No wall down
                        {
                            // No wall blocks way up, down, left or right => this is a freestanding wall surrounded by walkable space OR only connected by diagonals
                            if ((CheckWall && (_map[x + 1, y + 1].hasWall)) || (!CheckWall && _map[x + 1, y + 1].floorStyle == FloorStyle))   // Down Right diagonal
                            {
                                if ((CheckWall && (_map[x + 1, y - 1].hasWall)) || (!CheckWall && _map[x + 1, y - 1].floorStyle == FloorStyle)) // Down Right + Up Right diagonal
                                {
                                    if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle)) // Down Right + Up Right + Down Left diagonal
                                    {
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle)) // Down Right +Up Right + Down Left + Up Left diagonal
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
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle)) // Down Right  + Up right + Up Left diagonal (not up right)
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
                                    if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle)) // Down Right  + Down Left diagonal
                                    {
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  // Down Right + Down Left + Up Left diagonal
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
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  // Down Right + Up Left diagonal
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
                                if ((CheckWall && (_map[x + 1, y - 1].hasWall)) || (!CheckWall && _map[x + 1, y - 1].floorStyle == FloorStyle))  //  Up Right diagonal
                                {
                                    if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle))  // Up Right + Down Left diagonal
                                    {
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  // Up Right + Down Left + Up Left diagonal
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
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  // Up Right + Up Left diagonal
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

                                    if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle))  //  Down Left diagonal
                                    {
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  // Down Left + Up Left diagonal
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
                                        if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))  //  Up Left diagonal
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

                            if ((CheckWall && (_map[x + 1, y - 1].hasWall)) || (!CheckWall && _map[x + 1, y - 1].floorStyle == FloorStyle))
                            {
                                if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))
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
                                if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))
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
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
                        {
                            // Wall ony on current square and square below


                            // auf Diagonalen testen

                            if ((CheckWall && (_map[x + 1, y + 1].hasWall)) || (!CheckWall && _map[x + 1, y + 1].floorStyle == FloorStyle))
                            {
                                if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle))
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
                                if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle))
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
                    if ((CheckWall && (!_map[x, y - 1].hasWall)) || (!CheckWall && _map[x, y - 1].floorStyle == FloorStyle)) // No wall up
                    {
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
                        {
                            // Wall on current tile and right only, but not up or down

                            // auf Diagonalen testen

                            if ((CheckWall && (_map[x - 1, y + 1].hasWall)) || (!CheckWall && _map[x - 1, y + 1].floorStyle == FloorStyle))
                            {
                                if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))
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
                                if ((CheckWall && (_map[x - 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))
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
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
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
                if ((CheckWall && (!_map[x + 1, y].hasWall)) || (!CheckWall && _map[x + 1, y].floorStyle == FloorStyle))  // No Wall right
                {
                    if ((CheckWall && (!_map[x, y - 1].hasWall)) || (!CheckWall && _map[x, y - 1].floorStyle == FloorStyle))  // No wall up
                    {
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
                        {
                            // Left and Right closed

                            // auf Diagonalen testen

                            if ((CheckWall && (_map[x + 1, y + 1].hasWall)) || (!CheckWall && _map[x + 1, y + 1].floorStyle == FloorStyle))
                            {
                                if ((CheckWall && (_map[x + 1, y - 1].hasWall)) || (!CheckWall && _map[x + 1, y - 1].floorStyle == FloorStyle))
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
                                if ((CheckWall && (_map[x + 1, y - 1].hasWall)) || (!CheckWall && _map[x - 1, y - 1].floorStyle == FloorStyle))
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
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
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
                    if ((CheckWall && (!_map[x, y - 1].hasWall)) || (!CheckWall && _map[x, y - 1].floorStyle == FloorStyle))  // No wall up
                    {
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
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
                        if ((CheckWall && (!_map[x, y + 1].hasWall)) || (!CheckWall && _map[x, y + 1].floorStyle == FloorStyle))  // No wall down
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
        public static Rectangle _tileRect(Vector2 coords, bool tall = false)
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
        public static Backend.Coords _map2screen(Backend.Coords mapC, bool tall = false)
        {

            return new Backend.Coords((int)mapC.x * 64 + ((int)mapC.y) * 64
                                    , (int)mapC.y * 48 - (int)mapC.x * 48);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        public static Backend.Coords _map2screen(int x, int y, bool tall = false)
        {
            return new Backend.Coords(x * 64 + y * 64
                                    , y * 48 - x * 48);
        }


        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        public static Backend.Coords _screen2map(Backend.Coords screenC, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!
            screenC.x -= 32;
            screenC.y -= 48;
            return new Backend.Coords((int)Math.Ceiling((float)screenC.x / 128 - (float)screenC.y / 96)
                                    , (int)Math.Ceiling((float)screenC.x / 128 + (float)screenC.y / 96));
        }


        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        public static Backend.Coords _screen2map(int x, int y, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!

            return new Backend.Coords((int)Math.Ceiling((float)x / 128 - (float)y / 96)
                                    , (int)Math.Ceiling((float)x / 128 + (float)y / 96));
        }

        /// <summary>
        /// Determine tile based on coordinates of point
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tall"></param>
        /// <returns></returns>
        public static Backend.Coords _pos2Tile(Vector2 coords, bool tall = false)
        {
            // TODO: This does not work perfectly yet. Check the formula!
            coords.X -= 32;
            coords.Y -= 48;
            return new Backend.Coords((int)(coords.X / 128 - coords.Y / 96)
                                    , (int)(coords.X / 128 + coords.Y / 96));
        }

        /// <summary>
        /// Display all walls on the current map
        /// </summary>
        private void _drawWalls(GameTime gametime)
        {
            Backend.Coords currentPos = _map.actors[_playerID].tile.coords;

            //            System.Diagnostics.Debug.WriteLine((Math.Max(currentPos.y - _renderScope, 0)) + " " + (Math.Min(currentPos.y + _renderScope, _map.height)));
            //          System.Diagnostics.Debug.WriteLine((Math.Max(currentPos.x - _renderScope, 0)) + " " + (Math.Min(currentPos.x + _renderScope, _map.height)));

            for (int y = (Math.Max(currentPos.y - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); y < (Math.Min(currentPos.y + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.height)); ++y)
            {
                for (int x = (Math.Min(currentPos.x + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.width)); x >= (Math.Max(currentPos.x - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); --x)
                {
                    _drawWall(GetWallStyle(x, y), _tileRect(new Vector2(x + 1, y - 1), true), false, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)), _map[x, y].wallType);
                    if (((x != 0) && (y != 0) && (x != _map.width - 1) && (y != _map.height - 1)) && (_map[x, y].hasTeleport))
                        if (_map[x, y].teleport.down)
                        {
                            _spriteBatch.Draw(_environment[0][12].animationTexture, new Rectangle(_map2screen(x, y).x, _map2screen(x, y).y - 92, _environment[0][12].animationRect.Width, _environment[0][12].animationRect.Height), _environment[0][12].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(_environment[0][18].animationTexture, new Rectangle(_map2screen(x, y).x - 16, _map2screen(x, y).y - 32, _environment[0][18].animationRect.Width, _environment[0][18].animationRect.Height), _environment[0][18].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                        }

                    // show special objects
                    Backend.ReservedTile temp = _map[x, y].reserved;
                    if (temp != null)
                    {
                        if (temp.envIndex == -1)
                        {
                            for (int i = 0; i < _environment.Count; ++i)
                            {
                                if (_environment[i].filename == temp.filename) { temp.envIndex = i; break; }
                            }
                            if (temp.envIndex == -1)
                            {
                                TileSet tmp = new TileSet(_content, 96, 96);
                                tmp.Load(temp.filename);
                                _environment.Add(tmp);
                                temp.envIndex = _environment.Count - 1;
                            }
                        }
                        _spriteBatch.Draw(_environment[temp.envIndex][temp.index].animationTexture, new Rectangle(_map2screen(x, y).x - 16, _map2screen(x, y).y - 32, _environment[temp.envIndex][temp.index].animationRect.Width, _environment[temp.envIndex][temp.index].animationRect.Height), _environment[temp.envIndex][temp.index].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                        _environment[temp.envIndex][temp.index].NextAnimation();

                    }
                    foreach (ActorView actor in _actors)
                    {
                        Backend.Coords apos = _screen2map((int)actor.position.x, (int)actor.position.y);
                        if (((int)apos.x == x) && ((int)apos.y == y))
                        {
                            if (actor.animationTexture != null)
                                _spriteBatch.Draw(actor.animationTexture, new Vector2((actor.position.x + actor.offsetX), (actor.position.y + actor.offsetY - 32)), actor.animationRect, ((_map.actors[actor.id].tile.coords.y == (int)_highlightedTile.y) && (_map.actors[actor.id].tile.coords.x == (int)_highlightedTile.x) && !(_map.actors[actor.id] is Backend.Player)) ? Color.Red : Color.White);
                            if (actor.effect != null)
                            {
                                actor.effect.position = new Backend.Coords((actor.position.x + actor.offsetX), (actor.position.y + actor.offsetY - 32));
                                actor.effect.Draw(_spriteBatch, gametime);
                            }
                            if (!_map.actors[actor.id].isDead)
                            {
                                _spriteBatch.Draw(_background, new Rectangle((actor.position.x + actor.offsetX + 25), (actor.position.y + actor.offsetY - 30), actor.animationRect.Width, 5), new Rectangle(39, 6, 1, 1), Color.Black);
                                _spriteBatch.Draw(_background, new Rectangle((actor.position.x + actor.offsetX + 26), (actor.position.y + actor.offsetY - 29), (_map.actors[actor.id].health * (actor.animationRect.Width - 2)) / _map.actors[actor.id].maxHealth, 3), new Rectangle(39, 6, 1, 1), Color.Red);

                            }
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
        private void _drawFloor()
        {
            Backend.Coords currentPos = _map.actors[_playerID].tile.coords;
            for (int y = (Math.Max(currentPos.y - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); y < (Math.Min(currentPos.y + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.height)); ++y)
            {
                for (int x = (Math.Max(currentPos.x - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); x < (Math.Min(currentPos.x + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.width)); ++x)
                {
                    WallDir dir = GetWallStyle(x, y, false, 0);
                    switch (dir)
                    {

                        case WallDir.DiagUpDownClose: // Done
                            break;

                        case WallDir.DiagUpDownClose2: // Done
                            break;

                        case WallDir.DiagLeftRightClose: // Done
                            break;

                        case WallDir.DiagLeftRightClose2: // Done
                            break;


                        default:
                            _spriteBatch.Draw(_floors[(int)dir].animationTexture, _tileRect(new Vector2(x, y)),
                                _floors[(int)dir].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                            break;

                    }
                }
            }

            for (int y = (Math.Max(currentPos.y - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); y < (Math.Min(currentPos.y + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.height)); ++y)
            {
                for (int x = (Math.Max(currentPos.x - Math.Max(_map.actors[_playerID].viewRange, _map.light), 0)); x < (Math.Min(currentPos.x + Math.Max(_map.actors[_playerID].viewRange, _map.light) + 1, _map.width)); ++x)
                {


                    if (_map[x, y].hasTrap)
                    {
                        if (_map[x, y].trap.status != Backend.TrapState.NoDisplay)
                        {
                            if (!_map[x, y].trap.visible)
                            {
                                _spriteBatch.Draw(_environment[0][9].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 64), _environment[0][9].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                            }
                            else
                            {
                                _spriteBatch.Draw(_environment[0][10].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 64), _environment[0][10].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                            }
                        }
                    }
                    if (_map[x, y].hasCheckpoint)
                    {
                        _spriteBatch.Draw(_environment[0][24].animationTexture, new Rectangle(_map2screen(x, y).x, _map2screen(x, y).y, 128, 96), _environment[0][24].animationRect, (_map[x, y].checkpoint.visited) ? new Color(Color.Black, 0.4f) : ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                    }
                    if (_map[x, y].hasTarget)
                    {
                        _spriteBatch.Draw(_environment[0][23].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][23].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                    }
                    if (_map[x, y].hasTeleport)
                    {
                        if (!_map[x, y].teleport.teleport)
                        {
                            if ((x == 0) && (y == 0))
                            {

                                _environment[0][8].NextAnimation();
                                _spriteBatch.Draw(_environment[0][8].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][8].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }

                            else if ((x == 0) && (y == _map.height - 1))
                            {
                                _environment[0][5].NextAnimation();
                                _spriteBatch.Draw(_environment[0][5].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][5].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }
                            else if ((x == _map.width - 1) && (y == 0))
                            {
                                _environment[0][2].NextAnimation();
                                _spriteBatch.Draw(_environment[0][2].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][2].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }
                            else if ((x == _map.width - 1) && (y == _map.height - 1))
                            {
                                _environment[0][1].NextAnimation();
                                _spriteBatch.Draw(_environment[0][1].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][1].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }
                            else if (y == _map.height - 1)
                            {
                                _environment[0][6].NextAnimation();
                                _spriteBatch.Draw(_environment[0][6].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][6].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                            }
                            else if (y == 0)
                            {

                                _environment[0][4].NextAnimation();
                                _spriteBatch.Draw(_environment[0][4].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][4].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }
                            else if (x == 0)
                            {
                                _environment[0][7].NextAnimation();
                                _spriteBatch.Draw(_environment[0][7].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][7].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                            }
                            else if (x == _map.width - 1)
                            {
                                _environment[0][3].NextAnimation();
                                _spriteBatch.Draw(_environment[0][3].animationTexture, new Rectangle(_map2screen(x, y).x + 32, _map2screen(x, y).y + 16, 64, 48), _environment[0][3].animationRect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);

                            }

                        }
                    }
                    if (_map[x, y].hasTreasure)
                    {
                        foreach (Backend.Item item in (_map[x, y].items))
                        {
                            _spriteBatch.Draw(item.icon.texture, new Rectangle(_map2screen(x, y).x + item.icon.offset.x + 32, _map2screen(x, y).y + 16 + item.icon.offset.y,
                                item.icon.rect.Width - item.icon.crop.x, item.icon.rect.Height - item.icon.crop.y), item.icon.rect, ((y == (int)_highlightedTile.y) && (x == (int)_highlightedTile.x)) ? Color.Red : Color.White);
                        }

                    }
                }
            }
            //TODO: Reimplement rugged tiles
        }

        public bool IsMoving(int id)
        {
            return _actors[id].isMoving;
        }

        public void ChangeDir(int id, Backend.Direction dir)
        {
            _actors[id].direction = dir;
        }

        public void resetActors()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _noMove = true;
            }
            if (_effects != null) _effects.Clear();

            if (_floatnumbers != null) _floatnumbers.Clear();
            if (_projectiles != null) _projectiles.Clear();
            _maxProjectile = 0;
            _walls.Load("Content\\" + _map.wallFile + ".xml");
            _floors.Load("Content\\" + _map.floorFile + ".xml");
            _actors.Clear();
            for (int count = 0; count < _map.actorPositions.Count; ++count)
            {
                switch (_map.actors[count].actorType)
                {
                    case Backend.ActorType.Player:
                        _actors.Add(new ActorView(_camera, this, count, _content, _map2screen(_map.actorPositions[count]), _map.actors[count].animationFile, 3, _map.actors[count].health > 0));
                        break;
                    case Backend.ActorType.Enemy:
                        _actors.Add(new ActorView(_camera, this, count, _content, _map2screen(_map.actorPositions[count]), _map.actors[count].animationFile, 3, _map.actors[count].health > 0));
                        break;
                    case Backend.ActorType.NPC:
                        _actors.Add(new ActorView(_camera, this, count, _content, _map2screen(_map.actorPositions[count]), _map.actors[count].animationFile, 12, _map.actors[count].health > 0));
                        break;
                }
            }
            _camera.position = new Vector2(-38 - _actors[_playerID].position.x, -30 - _actors[_playerID].position.y);
            DisplaySubtitle(_map.name, "Level " + _map.level + " of " + _map.dungeonname);
        }

        public void floatNumber(Backend.Coords tile, string text, Color color)
        {
            uint delay = 0;
            if (_floatnumbers.Count > 0)
            {
                delay = _floatnumbers[_floatnumbers.Count - 1].delay + 20;
            }
            _floatnumbers.Add(new FloatNumber(_content, _spriteBatch, tile, text, _camera, color, 10, delay));
        }
        /// <summary>
        /// Move camera, react to mouse
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            if (_enabled)
            {
                if (_titleTime >= 0)
                {
                    _titleTime -= 1;
                    if ((_titleTime == 0) && (_visibility > 0))
                    {
                        _bigText = "";
                        _smallText = "";
                        _visibility = 0;
                        _titleTime = -1;
                    }
                    if ((_visibility < 0))
                    {
                        _visibility += 1;
                    }
                    if (_titleTime == 0)
                    {
                        _titleTime = 99;
                        _visibility = 100;
                    }
                    if (_visibility > 0)
                    {
                        _visibility -= 1;
                    }
                }

                for (int i = 0; i < _effects.Count; ++i)
                {

                    if (_effects[i].finished)
                    {
                        _effects.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        _effects[i].Update(gameTime);
                    }
                }
                if (_fireCount > 0) _fireCount -= Math.Min((uint)gameTime.ElapsedGameTime.Milliseconds, _fireCount);
                for (int i = 0; i < _projectiles.Count; ++i)
                {
                    _projectiles[i].Update(gameTime);
                }
                for (int i = 0; i < _floatnumbers.Count; ++i)
                {
                    if (_floatnumbers[i].Update(gameTime))
                    {
                        _floatnumbers.RemoveAt(i);
                        i -= 1;
                    }
                }
                if (!_noMove)
                {
                    if (IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        _UpdateMouse(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));

                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            if (!_actors[_playerID].isMoving)
                            {
                                MovePlayer(Backend.Map.WhichWayIs(_highlightedTile, _map.actors[_playerID].tile.coords));
                            }
                        }

                        if (Mouse.GetState().RightButton == ButtonState.Pressed)
                        {
                            if (!_actors[_playerID].isMoving)
                            {
                                FireProjectile();
                            }
                        }
                    }
                }
                else
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        _noMove = false;
                    }
                }

                /*   if (Math.Abs(gameTime.TotalGameTime.Milliseconds / 10 - _lastCheck) > 1)
                   {
                       _lastCheck = gameTime.TotalGameTime.Milliseconds / 10;*/
                // Avoid asynchronous updates, makes for smoother appearance
                for (int i = 0; i < _actors.Count; ++i)
                {
                    _actors[i].Update(gameTime);
                }
                // }
            }
        }

        public uint AddProjectile(Backend.Coords coords, Backend.Direction dir, ProjectileTile tile)
        {
            uint id = 0;
            lock (_mylock)
            {
                id = _maxProjectile;
                _maxProjectile += 1;
            }
            // System.Diagnostics.Debug.WriteLine("Added at " + coords);
            _projectiles.Add(new Projectile(id, this, coords, dir, tile));
            return id;
        }
        public void RemoveProjectile(uint id)
        {
            _projectiles.Remove(GetProjectile(id));
        }

        public Projectile GetProjectile(uint id)
        {
            lock (_mylock)
            {
                for (int i = 0; i < _projectiles.Count; ++i)
                {
                    if (_projectiles[i].id == id)
                    {
                        return _projectiles[i];
                    }
                }
            }
            return null;
        }

        public void FireProjectile()
        {
            if ((!_actors[_playerID].isMoving) && (_fireCount == 0))
            {
                //  System.Diagnostics.Debug.WriteLine("Add to " + _map.actors[_playerID].tile.coords);

                _parent.HandleEvent(false, Backend.Events.MoveProjectile, null, _map.actors[_playerID].tile.parent, _actors[_playerID].direction);
                _fireCount = 800;
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
        public void MovePlayer(Backend.Direction dir)
        {
            if (!_actors[_playerID].isMoving)
                _parent.HandleEvent(false, Backend.Events.MoveActor, 0, dir);
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
        public Mainmap(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayArea, Backend.Map map, bool enabled = true)
            : base(parent, spriteBatch, content, displayArea)
        {
            _font = _content.Load<SpriteFont>("font");
            _map = map;
            _background = _content.Load<Texture2D>("Minimap");
            _circle = _content.Load<Texture2D>("Light2");
            _highlightedTile = new Backend.Coords(-1, -1);
            _tooltip = new TileTooltip(this, _spriteBatch, _content, _displayRect);
            // Load textures to use in environment
            // 1. Walls and floor
            _walls = new WallTiles(_content, 128, 192, "");
            _floors = new WallTiles(_content, 128, 192, "");


            // 2. Environmental objects (floor, items, traps, teleporters, chest...)
            _environment = new List<TileSet>();
            _environment.Add(new TileSet(_content, 128, 192));
            _environment[0].Load("Content\\misc.xml");
            _environment.Add(new TileSet(_content, 64, 48));
            _environment[1].Load("Content\\Arrow.xml");
            _environment.Add(new TileSet(_content, 55, 55));
            _environment[2].Load("Content\\explosion.xml");

            // 3. Moving entities (player, NPCs, enemies)
            _actors = new List<ActorView>();
            _effects = new List<MapEffect>();

            resetActors();
            _floatnumbers = new List<FloatNumber>();
            _projectiles = new List<Projectile>();
            _enabled = enabled;
        }
        #endregion

    }
}
