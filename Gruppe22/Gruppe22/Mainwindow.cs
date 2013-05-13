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
#endregion


namespace Gruppe22
{
    public enum Events
    {
        ContinueGame = 0,
        EndGame,
        HideNotification,
        LoadMap,
        MoveActor,
        ChangeMap,
        NewMap,
        ResetGame,
        About
    }

    public enum GameStatus
    {
        Running,
        NoRedraw,
        Paused
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainWindow : Game, IHandleEvent
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


        bool _dragging = false;
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

        /// <summary>
        /// Whether the game is paused (for menus etc.)
        /// </summary>
        private GameStatus _status = GameStatus.Running;
        private Keys _lastKey = Keys.A;
        private int _lastCheck = 0;
        #endregion

        #region Protected Methods (overrides)
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            if (!System.IO.File.Exists("map1.xml"))
            {
                GenerateMaps();
            }
            _map1 = new Map("map1.xml", null); // TEST!!!
            _interfaceElements = new List<UIElement>();
            base.Initialize();
        }

        public void GenerateMaps()
        {
            Random r = new Random();
            Map tempMap = null;
            tempMap = new Map(r.Next(4) + 6, r.Next(4) + 6, true);
            tempMap.Save("map1.xml");
            tempMap.Dispose();
            tempMap = new Map(r.Next(5) + 3, r.Next(2) + 3, true);
            tempMap.Save("map2.xml");
            tempMap.Dispose();
            tempMap = new Map(r.Next(10) + 6, r.Next(10) + 6, true);
            tempMap.Save("map3.xml");
            tempMap.Dispose();
        }

        /// <summary>
        /// Cache Content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _interfaceElements.Add(new Minimap(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 230, 20, 210, 200), _map1));
            _interfaceElements.Add(new Mainmap(this, _spriteBatch, Content, new Rectangle(20, 20, _graphics.GraphicsDevice.Viewport.Width - 260, _graphics.GraphicsDevice.Viewport.Height - 160), _map1));
            _interfaceElements.Add(new Statusbox(this, _spriteBatch, Content, new Rectangle(20, _graphics.GraphicsDevice.Viewport.Height - 120, _graphics.GraphicsDevice.Viewport.Width - 260, 100)));
            (_interfaceElements[2] as Statusbox).AddLine("Welcome to this highly innovative Dungeon Crawler!\nYou can scroll in this status window.\nUse A-S-D-W to move your character.\n Use Arrow keys (or drag mouse) to scroll map or minimap\n Press ESC to display Game Menu.");


            // _backMusic = Content.Load<Song>("Video Dungeon Crawl.wav"); // Todo: *.mp3
            // _font = Content.Load<SpriteFont>("Font");
            // MediaPlayer.Volume = (float)0.3;
            // MediaPlayer.Play(_backMusic);
            ShowMenu();
        }

        /// <summary>
        /// Display Main Menu
        /// </summary>
        public void ShowMenu()
        {
            _status = GameStatus.Paused;
            Window _mainMenu = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 220) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 200, 350, 500));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 180, 300, 60), "Spiel fortsetzen", Events.ContinueGame));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 300, 60), "Spiel neu starten", Events.ResetGame));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 20, 300, 60), "Karte neu generieren", Events.NewMap));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 120, 300, 60), "Rechtliches", Events.About));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 220, 300, 60), "Spiel beenden", Events.EndGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));

            _interfaceElements.Add(_mainMenu);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        public void ShowEndGame(string message = "You have failed in your mission. Better luck next time.", string title = "Game over!")
        {
            _status = GameStatus.Paused;
            Window _gameOver = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Statusbox stat = new Statusbox(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 90), false, true);
            stat.AddLine(title + "\n \n" + message);
            _gameOver.AddChild(stat);
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Neue Karte", Events.NewMap));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 180, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Neu starten", Events.ResetGame));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 600 - 170, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Beenden", Events.EndGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));

            _interfaceElements.Add(_gameOver);
            _focus = _interfaceElements[_interfaceElements.Count - 1];

        }


        public void ShowAbout()
        {
            _status = GameStatus.Paused;
            Window _about = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 200, 600, 500));

            _about.AddChild(new Button(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 80) / 2.0f) + 120, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 240, 160, 40), "Ok", Events.ContinueGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));
            Statusbox stat = new Statusbox(_about, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 180, 600, 380), false, true);
            stat.AddLine("Dungeon Crawler 2013\n\n Developed by Group 22\n\n" +
                "*********************************\n\n" +
                "Music: Video Dungeon Crawl by Kevin MacLeod is licensed under a CC Attribution 3.0\n\n" +
                "http://incompetech.com/music/royalty-free/index.html?collection=029\n\n" +
                "Graphics: Tile Graphics by Reiner Prokein\n\n" +
                "http://www.reinerstilesets.de/de/lizenz/ ");
            _about.AddChild(stat);
            _interfaceElements.Add(_about);
            _focus = _interfaceElements[_interfaceElements.Count - 1];

        }


        /// <summary>
        /// Unload all managed content which has not been disposed of elsewhere
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {

            switch (eventID)
            {
                case Events.ContinueGame:
                    if (_status != GameStatus.NoRedraw)
                    {
                        _focus.Dispose();
                        _interfaceElements.Remove(_focus);
                        _focus = null;
                        _status = GameStatus.Running;
                    }
                    break;
                case Events.About:
                    _focus.Dispose();
                    _interfaceElements.Remove(_focus);
                    _focus = null;
                    ShowAbout();
                    break;
                case Events.EndGame:
                    Exit();
                    break;
                case Events.ChangeMap:
                    //TODO: Lade neue Karte
                    _status = GameStatus.NoRedraw;
                    _map1.Load((string)data[0], (Coords)data[1]);
                    _status = GameStatus.Running;
                    break;
                case Events.NewMap:
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    GenerateMaps();
                    _map1.Load("map1.xml", null);
                    _status = GameStatus.Paused;
                    HandleEvent(null, Events.ContinueGame);
                    break;
                case Events.ResetGame:
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    _map1.Load("map1.xml", null);
                    _status = GameStatus.Paused;
                    HandleEvent(null, Events.ContinueGame);
                    break;
                case Events.MoveActor:
                    int id = (int)data[0];
                    if (!((Mainmap)_interfaceElements[1]).IsMoving(id))
                    {
                        Direction dir = (Direction)data[1];
                        if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                        {
                            _map1.MoveActor(_map1.actors[id], dir);
                            ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.MoveActor, id, _map1.actors[id].tile.coords);
                        }
                        if (_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasEnemy || _map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTrap)
                        {
                            ShowEndGame();
                        }
                        if (_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget)
                        {
                            ShowEndGame("You have successfully found the hidden treasure. Can you do it again?", "Congratulations!");
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /*   ((ProgressBar)
                   ((Window)_interfaceElements[_interfaceElements.Count - 1]).children[((Window)_interfaceElements[_interfaceElements.Count - 1]).children.Count - 1]).value += 1;*/
            try
            {

                foreach (UIElement element in _interfaceElements)
                {

                    if (!_dragging)
                    {
                        if (element.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            if ((_focus == null) || (!_focus.holdFocus))
                            {
                                _focus = element;
                            }
                        }
                    }

                    if (_status == GameStatus.Running || ((_status == GameStatus.Paused) && (element.ignorePause)))
                        element.Update(gameTime);
                }




                //            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                //                _graphics.ToggleFullScreen();

                if (_focus != null)
                {
                    if (Mouse.GetState().ScrollWheelValue != _mouseWheel)
                    {

                        int Difference = _mouseWheel - Mouse.GetState().ScrollWheelValue;
                        _mouseWheel = Mouse.GetState().ScrollWheelValue;
                        _focus.ScrollWheel(Difference / Math.Abs(Difference));
                    }


                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (_mousepos.X != -1)
                        {
                            _dragging = true;
                            _focus.MoveContent(new Vector2(Mouse.GetState().X - _mousepos.X, Mouse.GetState().Y - _mousepos.Y), Math.Abs(gameTime.TotalGameTime.Milliseconds - _lastCheck));
                        }
                        _mousepos.X = Mouse.GetState().X;
                        _mousepos.Y = Mouse.GetState().Y;
                    }
                    else
                    {
                        _mousepos.X = -1;
                        _mousepos.Y = -1;
                        _dragging = false;
                    }

                    if (Math.Abs(gameTime.TotalGameTime.Milliseconds - _lastCheck) > 100)
                    {
                        _lastCheck = gameTime.TotalGameTime.Milliseconds;
                    }

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        _focus.MouseClick((int)_mousepos.X, (int)_mousepos.Y, Math.Abs(gameTime.TotalGameTime.Milliseconds - _lastCheck));
                    }

                    _focus.HandleKey(Math.Abs(gameTime.TotalGameTime.Milliseconds - _lastCheck));


                    if (_lastCheck > 90)
                    {
                        if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
                        {
                            if (_lastKey != Keys.Escape)
                            {
                                _lastKey = Keys.Escape;
                                if (_status == GameStatus.Running) ShowMenu();
                                else HandleEvent(null, Events.ContinueGame, 0);
                            }
                        }
                        else
                        {
                            _lastKey = Keys.None;
                        }
                    }
                }
            }
            catch { }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_status != GameStatus.NoRedraw)
            {
                GraphicsDevice.Clear(Color.Black);

                foreach (UIElement element in _interfaceElements)
                {
                    element.Draw(gameTime);
                }
                base.Draw(gameTime);
            }
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
        public MainWindow()
            : base()
        {
            Content.RootDirectory = "Content";
            Window.Title = "Dungeon Crawler 2013";

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 40;
            //_graphics.PreferredBackBufferHeight = 640;
            //_graphics.PreferredBackBufferWidth = 480;

            _graphics.IsFullScreen = false;
            Window.AllowUserResizing = false;
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
