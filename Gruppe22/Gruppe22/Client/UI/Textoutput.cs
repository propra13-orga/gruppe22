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
    /// <summary>
    /// Class used for output - parent to statusbox (output only) and chat (input + output)
    /// </summary>
    public class TextOutput : UIElement, Backend.IHandleEvent
    {
        /// <summary>
        /// Add a line of text to output (requires implementation in children
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public virtual void AddLine(string text, object color = null)
        {
            // Nothing happens here
        }

        public TextOutput(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
            // Nothing happens here ;-)
        }

    }
}
