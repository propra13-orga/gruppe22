using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
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
    public class Mainmap
    {
        Texture2D _wall1, _wall2, _floor;
        /// <summary>
        /// Output device
        /// </summary>
        GraphicsDeviceManager _graphics;
        /// <summary>
        /// Main Sprite drawing algorithm
        /// </summary>
        SpriteBatch _spriteBatch;
        Rectangle _displayRect;
        Camera _camera;
        Map _map;

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

        private void _drawWalls()
        {

            _drawWall(Direction.FourWay, 3, 3, false);
            _drawWall(Direction.UpRight, 3, 4, false);
            _drawWall(Direction.LeftClose, 4, 4, false);
        }
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

            _spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        rstate,
                        
                        null,
                        _camera.GetMatrix(_graphics));

            _spriteBatch.GraphicsDevice.ScissorRectangle = _displayRect;


            /*   _*/
            _drawFloor();
            _drawWalls();

            _spriteBatch.End();
            _spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            rstate.Dispose();


        }

        public Mainmap(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Rectangle displayArea, Texture2D floor, Texture2D wall1, Texture2D wall2, Map map)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _displayRect = displayArea;
            _camera = new Camera();
            _floor = floor;
            _wall1 = wall1;
            _wall2 = wall2;
            _map = map;

        }
    }
}
