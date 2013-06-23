using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public override void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            base.HandleEvent(DownStream, eventID, data);
        }

        public Chat(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, bool hasBorder = true, bool center = false)
            : base(parent, spriteBatch, content, displayRect)
        {
            _output = new Statusbox(this, spriteBatch, content, new Rectangle(_displayRect.Left, _displayRect.Top, _displayRect.Width, _displayRect.Height - 40), true, false);
            _input = new TextInput(this, spriteBatch, content, new Rectangle(_displayRect.Left, _displayRect.Bottom - 40, _displayRect.Width, 40), "Say:", "", "Enter text to distribute to other players", -1, true);
        }
    }
}
