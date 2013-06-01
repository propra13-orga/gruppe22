using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gruppe22
{
    /// <summary>
    /// Handle all UI related operations of the game
    /// </summary>
    public class GameWin : Game, IHandleEvent, IKeyHandler
    {
        #region Private Fields
        /// <summary>
        /// Central Output device
        /// </summary>
        protected GraphicsDeviceManager _graphics = null;

        /// <summary>
        /// Central Sprite drawing algorithm
        /// </summary>
        protected SpriteBatch _spriteBatch = null;

        /// <summary>
        /// Current mousewheel position (used to calculate changes)
        /// </summary>
        protected int _mouseWheel = 0;

        /// <summary>
        /// Whether the user is currently dragging something
        /// </summary>
        protected bool _dragging = false;

        /// <summary>
        /// Current position of mouse on screen
        /// </summary>
        protected Vector2 _mousepos = Vector2.Zero;

        /// <summary>
        /// A list of all elements displayed on screen
        /// </summary>
        protected List<UIElement> _interfaceElements = null;

        /// <summary>
        /// The focussed element (to which input is passed)
        /// </summary>
        protected UIElement _focus = null;

        /// <summary>
        /// Internal storage for Player 1
        /// </summary>
        protected Map _map1 = null;

        /// <summary>
        /// Internal storage for Player 2
        /// </summary>
        protected Map _map2 = null;

        /// <summary>
        /// Whether the game is paused (for menus etc.)
        /// </summary>
        protected GameStatus _status = GameStatus.Running;

        /// <summary>
        /// A list of files to download
        /// </summary>
        protected Queue<string> _files2fetch;

        /// <summary>
        /// Previous state (to reset after all files are downloaded)
        /// </summary>
        protected GameStatus _prevState;

        /// <summary>
        /// True if update cycle is in progress (to prevent simultaneous changes)
        /// </summary>
        protected bool _updating = false;

        /// <summary>
        /// True if currently drawing (to prevent superfluous redraws)
        /// </summary>
        protected bool _drawing = false;

        /// <summary>
        /// Current background color (used to indicate healing or damage)
        /// </summary>
        protected Color _backgroundcolor;

        /// <summary>
        /// A spritefont used to display information on screen
        /// </summary>
        protected SpriteFont _font;

        /// <summary>
        /// The main map used for player 1
        /// </summary>
        protected Mainmap _mainmap1 = null;

        /// <summary>
        /// The main map used for player 2
        /// </summary>
        protected Mainmap _mainmap2 = null;

        /// <summary>
        /// A minimap
        /// </summary>
        protected Minimap _minimap1 = null;

        /// <summary>
        /// The statusbox listing all messages / events
        /// </summary>
        protected Statusbox _statusbox = null;

        /// <summary>
        /// The player's inventory
        /// </summary>
        protected Inventory _inventory = null;


        /// <summary>
        /// A reference to the object displaying current player statistics
        /// </summary>
        protected SimpleStats _playerStats = null;

        /// <summary>
        /// A reference to the object displaying current enemy statistics
        /// </summary>
        protected SimpleStats _enemyStats = null;

        /// <summary>
        /// Change-based handling of events (i.e. keyup/keydown) instead of status based ("Is key pressed?")
        /// </summary>
        protected StateToEvent _events = null;

        /// <summary>
        /// Whether a second player is participating in the game
        /// </summary>
        private bool _secondPlayer = false;

        /// <summary>
        /// Whether we are playing by network (i.e. communicating with a server)
        /// </summary>
        private bool _lan = false;

        /// <summary>
        /// Random number generator
        /// </summary>
        protected Random r = null;
        #endregion


        #region MonoGame default functions (overriden)
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            _interfaceElements = new List<UIElement>(); // Initialize the list of UI elements (but not the objects themselves, see LoadContent)
            base.Initialize();
        }


        /// <summary>
        /// Cache Content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);             // Create a new SpriteBatch, which can be used to draw textures.
            _font = Content.Load<SpriteFont>("font"); // Load the font

            // Setup user interface elements
            _minimap1 = new Minimap(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 220, 5, 215, 215), _map1);
            _interfaceElements.Add(_minimap1);
            _mainmap1 = new Mainmap(this, _spriteBatch, Content, new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115), _map1, true);
            _interfaceElements.Add(_mainmap1);
            _mainmap2 = new Mainmap(this, _spriteBatch, Content, new Rectangle(5, ((_graphics.GraphicsDevice.Viewport.Height) / 2) - 60, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20) / 2) - 60), _map1, _secondPlayer && !_lan);
            _interfaceElements.Add(_mainmap2);
            _statusbox = new Statusbox(this, _spriteBatch, Content, new Rectangle(5, _graphics.GraphicsDevice.Viewport.Height - 125, _graphics.GraphicsDevice.Viewport.Width - 230, 120));
            _interfaceElements.Add(_statusbox);
            _inventory = new Inventory(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 220, _graphics.GraphicsDevice.Viewport.Height - 125, 215, 120), _map1.actors[0]);
            _interfaceElements.Add(_inventory);
            _statusbox.AddLine("Welcome to this highly innovative Dungeon Crawler!\nYou can scroll in this status window.\nUse A-S-D-W to move your character.\n Use Arrow keys (or drag mouse) to scroll map or minimap\n Press ESC to display Game Menu.");
            _playerStats = new SimpleStats(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 215, 230, 210, 100), _map1.actors[0]);
            _enemyStats = new SimpleStats(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 215, 340, 210, 100), null);
            _interfaceElements.Add(_playerStats);
            _interfaceElements.Add(_enemyStats);
            _inventory.Update();
            _playerStats.Update(null);
            if (_map1.actors[0].health < 1)
            {
                ShowEndGame();
            }
            // _backMusic = Content.Load<Song>("Video Dungeon Crawl.wav"); // Todo: *.mp3
            // _font = Content.Load<SpriteFont>("Font");
            // MediaPlayer.Volume = (float)0.3;
            // MediaPlayer.Play(_backMusic);            
        }


        /// <summary>
        /// Unload all managed content which has not been disposed of elsewhere
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }
        #endregion

        #region Implementation of IKeyHandler-Interface
        public void OnKeyDown(Keys k)
        {
            switch (k)
            {
                case Keys.Escape:
                    if (_status == GameStatus.Running) ShowMenu();
                    else HandleEvent(true, Events.ContinueGame, 0);
                    break;
            }

            foreach (UIElement element in _interfaceElements)
            {
                element.OnKeyDown(k);
            }
        }

        private void HandleMovementKeys(Keys k)
        {
            switch (k)
            {
                case Keys.W:
                    _mainmap1.MovePlayer(Direction.Up);
                    break;
                case Keys.A:
                    _mainmap1.MovePlayer(Direction.Left);
                    break;
                case Keys.D:
                    _mainmap1.MovePlayer(Direction.Right);
                    break;
                case Keys.S:
                    _mainmap1.MovePlayer(Direction.Down);
                    break;
                case Keys.Q:
                    _mainmap1.MovePlayer(Direction.UpLeft);
                    break;
                case Keys.E:
                    _mainmap1.MovePlayer(Direction.UpRight);
                    break;
                case Keys.Y:
                    _mainmap1.MovePlayer(Direction.DownLeft);
                    break;
                case Keys.C:
                    _mainmap1.MovePlayer(Direction.DownRight);
                    break;
            }
        }

        public void OnKeyUp(Keys k)
        {
        }

        public void OnMouseDown(int button)
        {
            for (int i = 0; i < _interfaceElements.Count; ++i)
            {
                _interfaceElements[i].OnMouseDown(button);
            }
        }

        public void OnMouseUp(int button)
        {
            foreach (UIElement element in _interfaceElements)
            {
                element.OnMouseUp(button);
            }
        }

        public void OnMouseHeld(int button)
        {
            foreach (UIElement element in _interfaceElements)
            {
                element.OnMouseHeld(button);
            }
        }

        public void OnKeyHeld(Keys k)
        {
            foreach (UIElement element in _interfaceElements)
            {
                element.OnKeyHeld(k);
            }
        }
        #endregion

        #region Event handling
        /// <summary>
        /// Handle events from UIElements and/or backend objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public virtual void HandleEvent(bool DownStream, Events eventID, params object[] data)
        {
            switch (eventID)
            {
                case Events.Player1:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Player2, false);
                        element.HandleEvent(true, Events.ToggleButton, Events.Player1, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.LAN, false);
                    }
                    _secondPlayer = false;
                    _mainmap2.enabled = false;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));
                    _lan = false;
                    break;

                case Events.FetchFile:
                    if (data.Length > 0)
                    {
                        string file = data[0].ToString();
                        _files2fetch.Enqueue(file);
                        if (_status != GameStatus.FetchingData)
                        {
                            _prevState = _status;
                            _status = GameStatus.FetchingData;
                            _LoadFile(file, wc_DownloadProgressChanged, wc_DownloadFileCompleted);
                        }
                    }
                    break;

                case Events.Player2:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Player2, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.Player1, false);
                    }
                    _secondPlayer = true;
                    if (!_lan)
                    {
                        _mainmap2.enabled = true;
                        _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20) / 2) - 60));
                    }
                    else
                    {
                        _mainmap2.enabled = false;
                        _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));
                    }

                    break;

                case Events.LAN:
                    _secondPlayer = true;
                    _lan = true;
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Player2, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.Player1, false);
                        element.HandleEvent(true, Events.ToggleButton, Events.Local, false);
                        element.HandleEvent(true, Events.ToggleButton, Events.LAN, true);
                    }
                    _mainmap2.enabled = false;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));
                    break;

                case Events.Local:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.LAN, false);
                    }
                    _lan = false;
                    _mainmap2.enabled = true;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20) / 2) - 60));
                    break;

                case Events.ContinueGame:
                    if (_status != GameStatus.NoRedraw)
                    {
                        if (_focus != null)
                        {
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                        }
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
                    _map1.Save("savedroom" + _map1.currRoomNbr + ".xml");//+Nummer
                    Exit();
                    break;

                case Events.NewMap:
                    _status = GameStatus.NoRedraw;
                    GenerateMaps();
                    HandleEvent(true, Events.ResetGame);
                    break;

                case Events.ResetGame:
                    _status = GameStatus.NoRedraw;
                    DeleteSavedRooms();
                    _map1.Dispose();
                    _map1.Load("room1.xml", null);
                    _mainmap1.resetActors();
                    _mainmap2.resetActors();
                    _inventory.actor = _map1.actors[0];
                    _playerStats.actor = _map1.actors[0];
                    _enemyStats.actor = null;
                    _inventory.Update();
                    _status = GameStatus.Paused;
                    HandleEvent(true, Events.ContinueGame);
                    break;

                case Events.ShowMessage:
                    AddMessage(data[0].ToString());
                    break;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Methode to delete old saved rooms
        /// </summary>
        public void DeleteSavedRooms()
        {
            if (File.Exists("GameData")) File.Delete("GameData");
            while (Directory.GetFiles(".", "savedroom*.xml").Length > 0)
            {
                File.Delete(Directory.GetFiles(".", "savedroom*.xml")[0]);
            }
        }

        /// <summary>
        /// A method to generate three rooms and save them to xml files
        /// </summary>
        public void GenerateMaps()
        {
            List<Exit> exits = new List<Exit>();
            Generator tempMap = null;

            for (int i = 1; i < 4; i++) //3 Level a 3 Räume
            {
                int minX = 7;
                int minY = 8;
                if (exits.Count > 0) minX += exits[0].from.x;
                if (exits.Count > 0) minY += exits[0].from.y;
                tempMap = new Generator(Content, this, minX + r.Next(8), minY + r.Next(8), true, null, i, 3, exits, r);
                tempMap.Save("room" + i.ToString() + ".xml");
                exits = Map.ExitToEntry(i + 1, tempMap.exits);
                tempMap.Dispose();
            }
        }
        #endregion


        #region Default subwindows
        /// <summary>
        /// Display Main Menu
        /// </summary>
        public void ShowMenu()
        {
            _status = GameStatus.Paused;
            Window _mainMenu = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 180) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 250, 320, 500));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 240, 300, 60), "Continue", Events.ContinueGame));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 170, 140, 60), "Restart", Events.ResetGame));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 170, 140, 60), "New Maps", Events.NewMap));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 140, 60), "1 Player", Events.Player1, !_secondPlayer));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 140, 60), "2 Players", Events.Player2, _secondPlayer));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 30, 140, 60), "Local", Events.Local, !_lan));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 30, 140, 60), "LAN", Events.LAN, _lan));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 40, 300, 60), "Settings", Events.Settings));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 110, 300, 60), "Credits", Events.About));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 180, 300, 60), "Quit", Events.EndGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));

            _interfaceElements.Add(_mainMenu);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        /// <summary>
        /// A text displayed if the player died
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public void ShowEndGame(string message = "You have failed in your mission. Better luck next time.", string title = "Game over!")
        {
            _status = GameStatus.Paused;
            Window _gameOver = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Statusbox stat = new Statusbox(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 110), false, true);
            stat.AddLine(title + "\n \n" + message);
            _gameOver.AddChild(stat);
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "New Maps", Events.NewMap));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 180, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Restart", Events.ResetGame));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 600 - 170, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Quit", Events.EndGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));

            _interfaceElements.Add(_gameOver);
            _focus = _interfaceElements[_interfaceElements.Count - 1];

        }

        /// <summary>
        /// Setup and display a window containing credits
        /// </summary>
        public void ShowAbout()
        {
            _status = GameStatus.Paused;
            Window _about = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 200, 600, 400));

            _about.AddChild(new Button(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 80, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 140, 160, 40), "Ok", Events.ContinueGame));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));
            Statusbox stat = new Statusbox(_about, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 180, 600, 380), false, true);
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
        #endregion



        /// <summary>
        /// Add text to status box
        /// </summary>
        /// <param name="s"></param>
        public void AddMessage(string s)
        {
            _statusbox.AddLine(s);
        }

        /// <summary>
        /// Indicate damage  done to player
        /// </summary>
        public void RemoveHealth()
        {
            _backgroundcolor.G = 0;
            _backgroundcolor.R = 200;
            // Play sound
        }

        /// <summary>
        /// Indicate health regained by potions
        /// </summary>
        public void AddHealth()
        {
            _backgroundcolor.R = 0;
            _backgroundcolor.G = 200;
            // Play sound
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            foreach (Keys k in _events.keys) { HandleMovementKeys(k); }

            if ((!_updating) && (_status != GameStatus.FetchingData))
            {
                {
                    // _updating = true;

                    _events.Update(gameTime);
                    if (_backgroundcolor.R > 0) // Remove Red Tint
                    {
                        _backgroundcolor.R -= 1;
                    };
                    if (_backgroundcolor.G > 0) // Remove Green Tint
                    {
                        _backgroundcolor.G -= 1;
                    };
                    for (int i = 0; i < _interfaceElements.Count; ++i)
                    {
                        UIElement element = _interfaceElements[i];
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

                    if (_status == GameStatus.Running)
                    {
                        _map1.Update(gameTime);
                    }

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
                                _focus.MoveContent(new Vector2(Mouse.GetState().X - _mousepos.X, Mouse.GetState().Y - _mousepos.Y));
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





                    }

                    _updating = false;
                }
            }
            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_status != GameStatus.FetchingData)
            {
                if (!_drawing)
                {
                    _drawing = true;
                    if (_status != GameStatus.NoRedraw)
                    {
                        GraphicsDevice.Clear(_backgroundcolor);

                        foreach (UIElement element in _interfaceElements)
                        {
                            element.Draw(gameTime);
                        }
                        base.Draw(gameTime);
                    }
                    _drawing = false;
                }
            }
            else
            {
                GraphicsDevice.Clear(Color.DarkBlue);
                _spriteBatch.Begin();

                string text = "Downloading additional content...";
                Vector2 position = new Vector2((GraphicsDevice.Viewport.Width - _font.MeasureString(text).X) / 2, (GraphicsDevice.Viewport.Height - _font.MeasureString(text).Y) / 2);
                _spriteBatch.DrawString(_font, text, position, Color.Gray);
                _spriteBatch.DrawString(_font, text, new Vector2(position.X - 2, position.Y - 2), Color.White);
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Download a file from the internet and place it in the local documents directory
        /// </summary>
        /// <param name="_filename">The name of the file to download</param>
        private async Task _LoadFile(string _filename, DownloadProgressChangedEventHandler progress, AsyncCompletedEventHandler finished)
        {
            var wc = new WebClient();

            wc.DownloadProgressChanged += progress;
            wc.DownloadFileCompleted += finished;
            try
            {
                await wc.DownloadFileTaskAsync("http://casim.hhu.de/Crawler/" + _filename, _filename);
            }
            catch
            {
                finished(null, null);
            }
        }

        /// <summary>
        /// Display download progress in the UI
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        public void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when a download is complete
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        public void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_files2fetch.Count == 0)
            {
                _status = _prevState;
            }
            else
            {
                string file = "";
                do
                { file = _files2fetch.Dequeue(); }
                while ((System.IO.File.Exists(file)) && (_files2fetch.Count > 0));
                if ((file != "") && (!System.IO.File.Exists(file)))
                {
                    _LoadFile(file, wc_DownloadProgressChanged, wc_DownloadFileCompleted);
                }

            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GameWin()
            : base()
        {

            _backgroundcolor = Color.Black;
            _events = new StateToEvent(this);
            _files2fetch = new Queue<String>();
            r = new Random();

        }
    }
}
