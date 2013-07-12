using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22.Client
{

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
}
