#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
#endregion


namespace Gruppe22
{
    /// <summary>
    /// The main class (disposing events, handling game logic, reacting to user input)
    /// </summary>
    public class MainWindow : Backend.GameLogic
    {



        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
            : base()
        {
            Content.RootDirectory = "Content";
            Window.Title = "Dungeon Crawler 2013";

            // Setup output: A little lower than fullscreen to save room for task bar
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.SynchronizeWithVerticalRetrace = true;
            // Move window to top left corner of the screen
            Type type = typeof(OpenTKGameWindow);
            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(this.Window);
            window.X = 0;
            window.Y = 0;
        }
        #endregion

    }
}


