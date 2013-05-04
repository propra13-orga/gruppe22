using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Gruppe22
{
    public class Button
    {
        private GraphicsDeviceManager _graphics;
        private string _button, _bpressed, _bmouseon;
        private Texture2D _tButton, _tBpressed, _tBmouseon;

        private Rectangle _dest;
        private Rectangle _src;

        private int _width, _heigth;

        private ButtonStatus _bstat;

        private enum ButtonStatus
        {
            normal = 0,
            mouseon,
            pressed
        }

        public Button(GraphicsDeviceManager graphics,string button, string bpressed, string bmouseon, Vector2 pos, int width, int heigth)
        {
            this._button = button;
            this._bpressed = bpressed;
            this._bmouseon = bmouseon;
            this._graphics = graphics;
            this._width = width;
            this._heigth = heigth;
            this._src = new Rectangle(0, 0, width, heigth);
            this._dest = new Rectangle((int)pos.X, (int)pos.Y, width, heigth);
            this._bstat = ButtonStatus.normal;
        }

        public void LoadContent(ContentManager Content)
        {
            this._tButton = Content.Load<Texture2D>(_button);
            this._tBpressed = Content.Load<Texture2D>(_bpressed);
            this._tBmouseon = Content.Load<Texture2D>(_bmouseon);
        }

        public void UnloadContent()
        {
            _tBmouseon.Dispose();
            _tBpressed.Dispose();
            _tButton.Dispose();
        }

        public bool Update(GameTime gameTime)
        {
            if (Mouse.GetState().X > _dest.Location.X && Mouse.GetState().X < _dest.Location.X + _width &&
                Mouse.GetState().Y > _dest.Location.Y && Mouse.GetState().Y < _dest.Location.Y + _heigth)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    return false;
                    
                _bstat = ButtonStatus.pressed;
            }
            else
                _bstat = ButtonStatus.normal;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if(_bstat == ButtonStatus.normal)
                spriteBatch.Draw(_tButton, _dest, _src, Color.White);
            else if (_bstat == ButtonStatus.mouseon)
                spriteBatch.Draw(_tBmouseon, _dest, _src, Color.White);
            else if (_bstat == ButtonStatus.pressed)
                spriteBatch.Draw(_tBpressed, _dest, _src, Color.White);
            spriteBatch.End();
        }

    }
}
