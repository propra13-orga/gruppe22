using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class Camera
    {
        #region Private Fields

        private float _zoom = (float)1.0;
        private Matrix _transform = Matrix.Identity;
        private Vector2 _position = Vector2.Zero;
        private Vector2 _center = Vector2.Zero;
        #endregion

        #region public Fields
        /// <summary>
        /// Get or set the camera's zoom level
        /// </summary>
        public float zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;

                if (_zoom < 0.1) _zoom = (float)0.1;
            }
        }

        /// <summary>
        /// Get or set the camera's position
        /// </summary>
        public Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Get a transformation matrix moving a camera to the specified position and zoom level
        /// </summary>
        /// <param name="_graphics">The device to use for calculating the matrix</param>
        /// <returns></returns>
        public Matrix matrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(_position, 0)) * Matrix.CreateScale(_zoom) * Matrix.CreateTranslation(new Vector3(_center, 0));

            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Move the Camera by a specified amount vertically and horizontally
        /// </summary>
        /// <param name="amount">The number of pixels to move the camera</param>
        public void Move(Vector2 amount)
        {
            _position += amount;
        }


        public void ResetCenter(Vector2 center)
        {
            _center = center;
        }

        #endregion

        public Camera(Vector2 center)
        {
            _center = center;
        }
    }
}
