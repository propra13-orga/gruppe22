using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    public class UIElement
    {
        /// <summary>
        /// The area used for the element
        /// </summary>
        protected Rectangle _displayRect;


        public bool IsHit(int x, int y)
        {
            return _displayRect.Contains(x, y);
        }

        public void Click(int Button)
        {

        }

        public void MoveContent(Vector2 difference)
        {

        }


        public void ScrollWheel(int Difference)
        {

        }

        public void HandleKey()
        {

        }
    }
}
