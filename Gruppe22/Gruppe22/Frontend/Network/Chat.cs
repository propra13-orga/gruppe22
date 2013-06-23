using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    class Chat : TextOutput
    {
        private Statusbox _output;
        private TextInput _input;

        public override void AddLine(string text, object color = null)
        {
            _output.AddLine(text, color);
        }

        public override void Update(GameTime gameTime)
        {
            _output.Update(gameTime);
            _input.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _output.Draw(gameTime);
            _input.Draw(gameTime);
        }

        public override bool focus
        {
            set
            {
                _input.focus = true;
            }
            get
            {
                return _input.focus;
            }
        }

        public override bool OnKeyDown(Microsoft.Xna.Framework.Input.Keys k)
        {
            if (_input.focus)
            {
                switch (k)
                {
                    case Keys.Enter:
                        if (_input.text.Trim() != "")
                        {
                            _parent.HandleEvent(false, Events.Chat, _input.text.Trim());
                            _input.text = "";
                        }
                        return true;
                    case Keys.Escape:
                        _input.focus = false;
                        return true;
                }

            };
            return (_input.OnKeyDown(k) || _output.OnKeyDown(k));
        }

        public override bool OnMouseDown(int button)
        {
            if (new Rectangle(_displayRect.Left, _displayRect.Bottom - 40, _displayRect.Width, 40).Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _input.focus = true;
            }
            else
            {
                _input.focus = false;
            }
            return (_input.OnMouseDown(button) || _output.OnMouseDown(button));

        }
        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            base.HandleEvent(DownStream, eventID, data);
        }

        public Chat(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, bool hasBorder = true, bool center = false)
            : base(parent, spriteBatch, content, displayRect)
        {
            _output = new Statusbox(this, spriteBatch, content, new Rectangle(_displayRect.Left, _displayRect.Top, _displayRect.Width, _displayRect.Height - 22), true, false);
            _input = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left, _displayRect.Bottom - 20, _displayRect.Width, 20), "Say:", "", "Enter text to distribute to other players", -1, true);
        }
    }
}
