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
    class TextInput : UIElement
    {
        private string _label = "";
        private string _text = "";
        private string _toolTip = "";

        public string text
        {
            get
            {
                return _text;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public TextInput(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, string label, string text, string toolTip)
            : base(parent, spriteBatch, content, displayRect)
        {
            _label = label;
            _text = text;
            _toolTip = toolTip;
        }
    }
}
