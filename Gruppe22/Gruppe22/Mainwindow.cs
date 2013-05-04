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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
#endregion


namespace Gruppe22
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainWindow : Game
    {
        #region Private Fields
        /// <summary>
        /// Central Output device
        /// </summary>
        private GraphicsDeviceManager _graphics = null;

        /// <summary>
        /// Central Sprite drawing algorithm
        /// </summary>
        private SpriteBatch _spriteBatch = null;

        /// <summary>
        /// Current mousewheel position (used to calculate changes)
        /// </summary>
        private int _mouseWheel = 0;

        /// <summary>
        /// Current position of mouse on screen
        /// </summary>
        private Vector2 _mousepos = Vector2.Zero;

        /// <summary>
        /// A list of all elements displayed on screen
        /// </summary>
        private List<UIElement> _interfaceElements = null;

        /// <summary>
        /// The focussed element (to which input is passed)
        /// </summary>
        private UIElement _focus = null;

        /// <summary>
        /// Internal storage for Player 1
        /// </summary>
        private Map _map1 = null;

        /// <summary>
        /// Internal storage for Player 2
        /// </summary>
        private Map _map2 = null;

        #endregion

        #region Protected Methods (overrides)
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            string str4 =
                "######.####.###\n" +
                "###.##.####.###\n" +
                "###.........###\n" +
                "###.###.###.###\n" +
                "###.###.###.###\n" +
                "#.............#\n" +
                "#....##.#######\n" +
                "#.#####.#######\n",
             str =
              "###############\n" +
              "#....#........#\n" +
              "#....#........#\n" +
              "#..############\n" +
              "#....#..#.....#\n" +
              "#.####..#######\n" +
              "#....#..#.....#\n" +
              "######..#######\n",
            str1 =
              "###############\n" +
              "#.....#.......#\n" +
              "#.....#.......#\n" +
              "###.........###\n" +
              "#.............#\n" +
              "#.....#.......#\n" +
              "#.....#.......#\n" +
              "###############\n";
            _map1 = new Map(15, 8);
            _map1.FromString(str);
            _interfaceElements = new List<UIElement>();

            base.Initialize();
        }

        /// <summary>
        /// Cache Content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _interfaceElements.Add(new Minimap(_spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 210, 20, 200, 200), _map1));
                        _interfaceElements.Add(new Mainmap(_spriteBatch, Content, new Rectangle(0, 20, _graphics.GraphicsDevice.Viewport.Width - 220, _graphics.GraphicsDevice.Viewport.Height - 140), _map1));
                        _interfaceElements.Add(new Statusbox(_spriteBatch, Content, new Rectangle(40, _graphics.GraphicsDevice.Viewport.Height - 120, _graphics.GraphicsDevice.Viewport.Width - 20, 100)));


            // _backMusic = Content.Load<Song>("Video Dungeon Crawl.wav"); // Todo: *.mp3
            // _font = Content.Load<SpriteFont>("Font");
            // MediaPlayer.Volume = (float)0.3;
            // MediaPlayer.Play(_backMusic);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            foreach (UIElement element in _interfaceElements)
            {
                if (element.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    _focus = element;
                }
                element.Update(gameTime);
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                _graphics.ToggleFullScreen();

            if (_focus != null)
            {
                if (Mouse.GetState().ScrollWheelValue != _mouseWheel)
                {
                    _focus.ScrollWheel(_mouseWheel - Mouse.GetState().ScrollWheelValue);
                    _mouseWheel = Mouse.GetState().ScrollWheelValue;
                }


                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (_mousepos.X != -1)
                    {
                        _focus.MoveContent(new Vector2(Mouse.GetState().X - _mousepos.X, Mouse.GetState().Y - _mousepos.Y));
                    }
                    _mousepos.X = Mouse.GetState().X;
                    _mousepos.Y = Mouse.GetState().Y;
                }
                else
                {
                    _mousepos.X = -1;
                    _mousepos.Y = -1;
                }
                _focus.HandleKey();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            foreach (UIElement element in _interfaceElements)
            {
                if (element.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    _focus = element;
                }
                element.Draw(gameTime);
            }
            base.Draw(gameTime);
        }



        /// <summary>
        /// Download a file from the internet and place it in the local documents directory
        /// </summary>
        /// <param name="_filename">The name of the file to download</param>
        private static async void _LoadFile(string _filename)
        {
            var wc = new WebClient();

            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += wc_DownloadFileCompleted;
            await wc.DownloadFileTaskAsync("http://casim.hhu.de/Crawler/" + _filename, _filename);
        }

        /// <summary>
        /// Display download progress in the UI
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when a download is complete
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        public static void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Level data (or download it if it is not available)
        /// </summary>
        /// <returns></returns>
        public bool LoadLevel()
        {
            string filename = "";
            if (!System.IO.File.Exists(filename))
            { // Ressource not available locally: Fetch it from the web
                _LoadFile(filename);
            }
            return true;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow(): base()
        {
            Content.RootDirectory = "Content";
            Window.Title = "Dungeon Crawler 2013";

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 40;
            _graphics.IsFullScreen = false;
            Window.AllowUserResizing = true;
            Type type = typeof(OpenTKGameWindow);

            // Move window to top left corner of the screen
            System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(this.Window);
            window.X = 0;
            window.Y = 0;
        }
        #endregion
    }
}
