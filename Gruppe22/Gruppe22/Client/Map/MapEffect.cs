using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

}
