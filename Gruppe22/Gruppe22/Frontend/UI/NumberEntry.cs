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
    class NumberEntry:UIElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public NumberEntry(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
        }

    }
}
