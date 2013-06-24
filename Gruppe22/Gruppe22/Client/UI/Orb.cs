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
    public class Orb : UIElement
    {
        private Texture2D _orb = null;
        private SpriteFont _font = null;
        private Color _color = Color.Red;
        private int _cutoff = 50;
        private int _value = 0;
        private int _max = 100;
        private int _lastCheck = 0;
        private Vector2 _center = Vector2.Zero;
        private Backend.Actor _actor;
        private bool _isMagic = false;

        public Backend.Actor actor
        {
            set
            {
                _actor = value;
            }
            get
            {
                return _actor;
            }
        }

        public int value
        {
            set
            {
                _value = value;
            }
            get
            {
                return _value;
            }
        }

        public int max
        {
            set
            {
                _max = value;
            }
            get
            {
                return _max;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (actor != null)
            {
                _max = !_isMagic ? actor.maxHealth : actor.maxMana;
                _value = !_isMagic ? actor.health : actor.mana;
            }
            _lastCheck += gameTime.ElapsedGameTime.Milliseconds;
            if (_lastCheck > 70)
            {
                _lastCheck -= 70;
                if ((_cutoff < (int)Math.Round(((float)(_max - _value) / (float)_max) * 135f)) && (_cutoff < 135))
                {
                    _cutoff += 1;
                }
                else
                {
                    if ((_cutoff > (int)Math.Round(((float)(_max - _value) / (float)_max) * 135f)) && (_cutoff > -1))
                        _cutoff -= 1;
                }
            }
        }

        public override void Draw(GameTime gametime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_orb, new Rectangle(_displayRect.Left + 2, _displayRect.Top + 2 + _cutoff, 135, 135 - _cutoff), new Rectangle(3, 145 + _cutoff, 135, 135 - _cutoff), _color);
            _spriteBatch.Draw(_orb, new Rectangle(_displayRect.Left, _displayRect.Top, 139, 139), new Rectangle(0, 0, 139, 139), _color);
            _spriteBatch.DrawString(_font, _value.ToString("000") + "/" + _max.ToString("000"), _center, Color.Black, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(_font, _value.ToString("000") + "/" + _max.ToString("000"), new Vector2(_center.X - 1, _center.Y - 1), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        public Orb(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, int max, int value, Color color)
            : base(parent, spriteBatch, content, displayRect)
        {
            _font = _content.Load<SpriteFont>("SmallFont");
            _orb = _content.Load<Texture2D>("Orbs");
            _color = color;
            _max = max;
            _actor = null;
            _value = value;
            _center = new Vector2(_displayRect.Left + (_displayRect.Width - _font.MeasureString("100/100").X * 1.5f) / 2, _displayRect.Top + (_displayRect.Height - _font.MeasureString("100/100").Y * 1.5f) / 2);
        }


        public Orb(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Actor actor, bool isMagic)
            : base(parent, spriteBatch, content, displayRect)
        {
            _displayRect.Width = 139;
            _displayRect.Height = 139;
            _font = _content.Load<SpriteFont>("SmallFont");
            _orb = _content.Load<Texture2D>("Orbs");
            _color = isMagic ? Color.DarkBlue : Color.DarkRed;
            _actor = actor;
            _isMagic = isMagic;
            _max = !isMagic ? actor.maxHealth : actor.maxMana;
            _value = !isMagic ? actor.health : actor.mana;
            _center = new Vector2(_displayRect.Left + (_displayRect.Width - _font.MeasureString("100/100").X * 1.5f) / 2, _displayRect.Top + (_displayRect.Height - _font.MeasureString("100/100").Y * 1.5f) / 2);
        }

    }
}
