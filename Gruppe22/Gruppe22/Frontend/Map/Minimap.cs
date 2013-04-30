using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gruppe22
{
    public class Minimap
    {
        #region Private Fields
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Texture2D _mapIcon;
        Rectangle _paintRegion;
        float _zoom = (float)1.0;
        Rectangle _mapRegion;
        Map _map;
        #endregion

        #region Public Methods
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected  void LoadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected  void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected  void Draw(GameTime gameTime)
        {
            
        }
        #endregion
    }
}
