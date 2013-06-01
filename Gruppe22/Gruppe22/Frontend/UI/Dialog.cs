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
    class Dialog : Window
    {
        private Actor _actor = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public Dialog(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect, Actor actor)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
    }
}
