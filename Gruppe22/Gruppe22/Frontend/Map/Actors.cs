using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class ActorView
    {
        private Vector2 _position = Vector2.Zero;
        private Vector2 _target = Vector2.Zero;

        private SpriteBatch _spriteBatch = null;
        private TileObject _sprite = null;

        public Vector2 position
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

        public Vector2 target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        public bool isMoving
        {
            get
            {
                return (!_position.Equals(_target));
            }
        }

        public int animationStyle
        {
            get
            {
                return _sprite.currentAnimation;
            }
            set { _sprite.currentAnimation = value; }
        }

        public void Move(Vector2 difference)
        {
            if (_target.X == -1)
            {
                _target = _position;
            }
            _target.X += difference.X;
            _target.Y += difference.Y;
        }

        public void Update(GameTime gametime)
        {
            if (!_target.Equals(_position))
            {
                if (Math.Abs(_target.X - _position.X) < 0.1) { _position.X = (int)_target.X; _target.X = _position.X; };
                if (Math.Abs(_target.Y - _position.Y) < 0.1) { _position.Y = (int)_target.Y; _target.Y = _position.Y; };
                if (!_target.Equals(_position))
                {
                    if (_target.X > _position.X) { _position.X += 0.05f; animationStyle = 1; }
                    else
                        if (_target.X < _position.X) { _position.X -= 0.05f; animationStyle = 2; }
                        else
                            if (_target.Y > _position.Y) { _position.Y += 0.05f; animationStyle = 3; }
                            else
                                if (_target.Y < _position.Y) { _position.Y -= 0.05f; animationStyle = 4; }
                    _sprite.NextAnimation();

                }
            }
        }

        public void Draw(GameTime gametime)
        {
            if (_sprite.isValid)
            {

                _spriteBatch.Draw(_sprite.animationTexture, new Rectangle((int)((_position.X) * 64 + (_position.Y - 1) * 64) + 30, (int)((_position.Y) * 48 - (_position.X) * 48) - 50, 144, 144), _sprite.animationRect, Color.White);
            }
            //            _position.X += 0.1f;
        }

        public ActorView(SpriteBatch spritebatch, string name, bool controllable, Vector2 position, TileObject sprite)
        {
            _position = position;
            _target = _position;
            _spriteBatch = spritebatch;
            _sprite = sprite;
        }
    }
}
