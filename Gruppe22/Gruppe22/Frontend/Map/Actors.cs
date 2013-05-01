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
        private int _animPhaseX = 0;
        private int _animPhaseY = 1;
        private Texture2D _actor1;
        private Texture2D _actor2;
        private int _x;
        private int _y;

        public void Update()
        {
            _animPhaseX += 1;
            if (_animPhaseX > 15)
            {
                _animPhaseX = 0;
                _animPhaseY += 1;

            }
            if (_animPhaseY > 7)
            {
                _animPhaseY = 0;
            }
        }

        public void Draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_actor2, new Rectangle(_x * 128 + 1, _y * 48 - 96, 192, 192), new Rectangle(_animPhaseX * 128, _animPhaseY * 128, 128, 128), Color.White);
        }

        public ActorView(string name, bool controllable, int x, int y,Texture2D actor1,Texture2D actor2)
        {
            _x = x;
            _y = y;
            _actor1 = actor1;
            _actor2 = actor2;
        }
    }
}
