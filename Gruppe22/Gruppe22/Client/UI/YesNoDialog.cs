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
    class YesNoDialog : Window
    {
        private Statusbox _question;
        private Button _yes;
        private Button _no;

        public YesNoDialog(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string question = "Quit?", string yes = "Yes", string no = "No")
            : base(parent, spriteBatch, content, displayRect)
        {
            AddChild(_yes = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Right - 85, _displayRect.Bottom - 35, 85, 30), yes, (int)Backend.Buttons.Yes));
            AddChild(_no = new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Bottom - 35, 85, 30), no, (int)Backend.Buttons.No));
            AddChild(_question = new Statusbox(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 10, _displayRect.Top + 10, _displayRect.Width - 20, _displayRect.Height - 60), false, true));
            _question.AddLine(question);
            ChangeFocus();
        }
    }
}
