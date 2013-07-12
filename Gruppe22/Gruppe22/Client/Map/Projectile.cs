using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{

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


        /// <summary>
        /// Refresh all objects on the map (animation etc)
        /// </summary>
        /// <param name="gametime">elapsed time since game start</param>
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
}
