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
    class ModifyWindow : Window
    {
        List<NumberEntry> _properties;
        Backend.Item _item;
        TextInput _name;
        /// <summary>
        /// Constructor
        /// </summary>
        public ModifyWindow(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Backend.Item item)
            : base(parent, spriteBatch, content, displayRect)
        {
            _item = item;
            _name = new TextInput(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 5, _displayRect.Width - 15, 25), "Name:", _item.name, "This is the name used for the item.", 20, true);
            _children.Add(_name);
            _properties = new List<NumberEntry>();
            int i = 0;
            foreach (Backend.ItemEffect effect in _item.effects)
            {
                NumberEntry element = new NumberEntry(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + 5, _displayRect.Top + 35 + i * 30, (_displayRect.Width - 10) / 2 - 10, 25), effect.property.ToString(), effect.effect, "Click to remove ability", 2, false);
                ++i;
                _children.Add(element);
            }
            _children.Add(new Button(this, _spriteBatch, _content, new Rectangle(_displayRect.Left + (_displayRect.Width - 100) / 2, _displayRect.Top + _displayRect.Height - 45, 100, 30), "Ok", (int)Backend.Buttons.Close, false));
            _focusID = _children.Count - 1;
            ChangeFocus();
        }
    }
}
