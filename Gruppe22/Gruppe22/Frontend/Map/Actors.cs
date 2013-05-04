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

        public int animationStyle
        {
            get
            {
                return _sprite.currentAnimation;
            }
            set { _sprite.currentAnimation = value; }
        }


        public void Update(GameTime gametime)
        {
            _sprite.NextAnimation();
        }

        public void Draw(GameTime gametime)
        {
            if (_sprite.isValid)
            {
                _spriteBatch.Draw(_sprite.animationTexture, new Rectangle((int)_position.X * 128 + 1, (int)_position.Y * 48 - 96, 192, 192), _sprite.animationRect, Color.White);
            }
        }

        public ActorView(SpriteBatch spritebatch, string name, bool controllable, Vector2 position, TileObject sprite)
        {
            _position = position;
            _spriteBatch = spritebatch;
            _sprite = sprite;
        }
    }
}
