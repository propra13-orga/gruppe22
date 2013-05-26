using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruppe22
{
    class Inventory:Grid
    {
        #region Private Fields
        Actor _actor = null;
        #endregion

        #region Public Fields
        #endregion

        #region Constructor        
        public Inventory(IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect,Actor actor=null)
            : base(parent, spriteBatch, content, displayRect)
        {
            _actor = actor;
        }
        #endregion
    }
}
