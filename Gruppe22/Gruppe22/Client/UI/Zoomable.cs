using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22.Client
{
    public class Zoomable : UIElement
    {
        #region Private Fields
        /// <summary>
        /// The transformation-matrix used for zooming and panning the map
        /// </summary>
        protected Camera _camera;
        #endregion

        #region Public Fields
        /// <summary>
        /// Current zoom level
        /// </summary>
        public float Zoom
        {
            get
            {
                return _camera.zoom;
            }
            set
            {
                if ((value > 0.4) && (value < 4.0))
                    _camera.zoom = value;
            }
        }

        /// <summary>
        /// Current position of the map
        /// </summary>
        public Vector2 Pos
        {
            get
            {
                return _camera.position;
            }
            set
            {
                _camera.position = value;
            }
        }
        #endregion

        #region Public Methods

        public override void MoveContent(Vector2 difference, int _lastCheck = 0)
        {
            Move(difference);
            base.MoveContent(difference);
        }


        public override void ScrollWheel(int Difference)
        {
            Zoom -= Difference;
            base.ScrollWheel(Difference);
        }

        public override bool OnKeyDown(Keys k)
        {
            return DoZoom(k);
        }

        public override bool OnKeyHeld(Keys k)
        {
            return DoZoom(k);
        }
        public bool DoZoom(Keys k)
        {
            switch (k)
            {
                case Keys.PageUp:
                    Zoom += (float)0.1;
                    return true;

                case Keys.PageDown:
                    Zoom -= (float)0.1;
                    return true;

                case Keys.Right:
                    Move(new Vector2(-1, 0));
                    return true;

                case Keys.Left:
                    Move(new Vector2(1, 0));
                    return true;

                case Keys.Down:
                    Move(new Vector2(0, -1));
                    return true;

                case Keys.Up:
                    Move(new Vector2(0, 1));
                    return true;

            }
            return false;
        }
        /// <summary>
        /// Move the camera by a specified number of pixels (pass through to camera)
        /// </summary>
        /// <param name="target"></param>
        public void Move(Vector2 target)
        {
            _camera.Move(target);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="content"></param>
        /// <param name="displayRect"></param>
        public Zoomable(Backend.IHandleEvent parent, SpriteBatch spriteBatch, ContentManager content, Rectangle displayRect)
            : base(parent, spriteBatch, content, displayRect)
        {
            _camera = new Camera(new Vector2(displayRect.Width / 2 + displayRect.Left, displayRect.Height / 2 + displayRect.Top));
            _camera.position = new Vector2(-displayRect.Left, -displayRect.Top);
            //_camera.zoom = 0.4f;
        }
        #endregion
    }
}
