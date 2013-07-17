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
    public enum BubbleElement{
        Left,
        Right,
        Bottom,
        Top,
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
        PointerTopLeft,
        PointerTopRight,
        PointerBottomLeft,
        PointerBottomRight
    }
    public class Bubble
    {
        private Texture2D _bubble = null;
        private SpriteFont _font = null;
        private string _text = "";
        private Gruppe22.Backend.Coords _position = Gruppe22.Backend.Coords.Zero;
        private Rectangle _maxRect = Rectangle.Empty;
        private Rectangle _drawRect = Rectangle.Empty;
        private Gruppe22.Backend.Direction _direction = Gruppe22.Backend.Direction.None;
        private ContentManager _content = null;

        public string text
        {
            get { return _text; }
            set { _text = value; _SplitString(); }
        }

        public Backend.Coords position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Rectangle maxRect
        {
            get { return _maxRect; }
            set { _maxRect = value; }
        }

        public Rectangle drawRect
        {
            get
            {
                return _drawRect;
            }
            set { _drawRect = value; }
        }

        public Backend.Direction direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        private void DrawElement(BubbleElement element, Rectangle Rect){
        
        }

        public void Draw(SpriteBatch _spritebatch)
        {

        }

        private void _SplitString()
        {
            _text = "";
        }

        public Bubble(ContentManager content, Rectangle maxRect, string text = "", Backend.Direction dir = Backend.Direction.None)
        {
            _content = content;
            _maxRect = maxRect;
            _text = text;
            _direction = dir;
            _SplitString();
            _bubble = _content.Load<Texture2D>("Bubble");
            _font = _content.Load<SpriteFont>("Smallfont");
        }
    }
}
