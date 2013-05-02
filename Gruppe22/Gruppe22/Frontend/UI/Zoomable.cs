using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
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

        public void MoveContent(Vector2 difference)
        {
            Move(difference);
            base.MoveContent(difference);
        }


        public new void ScrollWheel(int Difference)
        {
            Zoom += Difference / 10;
            base.ScrollWheel(Difference);
        }

        public new void HandleKey()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                Zoom += (float)0.1;

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                Zoom -= (float)0.1;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                Move(new Vector2(-1, 0));

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                Move(new Vector2(1, 0));

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                Move(new Vector2(0, -1));

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Move(new Vector2(0, 1));
            base.HandleKey();
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
    }
}
