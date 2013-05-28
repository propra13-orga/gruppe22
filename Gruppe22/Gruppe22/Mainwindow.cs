﻿#region Using Statements
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
    public enum Events
    {
        ContinueGame = 0,
        EndGame,
        ToggleButton,
        HideNotification,
        MoveActor,
        ChangeMap,
        NewMap,
        ResetGame,
        About,
        AnimateActor,
        FinishedAnimation,
        ShowMessage,
        Player1,
        Player2,
        Network,
        Settings,
        Local,
        LAN,
        FetchFile
    }

    public enum GameStatus
    {
        Running,
        NoRedraw,
        Paused,
        FetchingData
    }

    /// <summary>
    /// The main class (disposing events, handling game logic, reacting to user input)
    /// </summary>
    public class MainWindow : Game, IHandleEvent, IKeyHandler
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
        /// Whether the user is currently dragging something
        /// </summary>
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
        private Queue<string> _files2fetch;
        private GameStatus _prevState;
        /// <summary>
        /// True if update cycle is in progress (to prevent simultaneous changes)
        /// </summary>
        private bool _updating = false;
        /// <summary>
        /// True if currently drawing (to prevent superfluous redraws)
        /// </summary>
        private bool _drawing = false;
        /// <summary>
        /// Current background color (used to indicate healing or damage)
        /// </summary>
        private Color _backgroundcolor;
        private SpriteFont _font;
        private Mainmap _mainmap1 = null;
        private Mainmap _mainmap2 = null;
        private Minimap _minimap1 = null;
        private Statusbox _statusbox = null;
        private Inventory _inventory = null;
        /// <summary>
        /// A reference to the object displaying current player statistics
        /// </summary>
        private SimpleStats _playerStats = null;
        /// <summary>
        /// A reference to the object displaying current enemy statistics
        /// </summary>
        private SimpleStats _enemyStats = null;

        /// <summary>
        /// Change-based handling of events (i.e. keyup/keydown) instead of status based ("Is key pressed?")
        /// </summary>
        private StateToEvent _events = null;

        private bool _secondPlayer = false;
        private bool _lan = false;
        #endregion


        #region Implementation of IKeyHandler-Interface
        public void OnKeyDown(Keys k)
        {
            switch (k)
            {
                case Keys.Escape:
                    if (_status == GameStatus.Running) ShowMenu();
                    else HandleEvent(null, Events.ContinueGame, 0);
                    break;
            }

            HandleMovementKeys(k);
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
            HandleMovementKeys(k);
            foreach (UIElement element in _interfaceElements)
            {
                element.OnKeyHeld(k);
            }
        }
        #endregion
        #region Protected Methods (overrides)
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            if (!System.IO.File.Exists("room1.xml"))
            {
                GenerateMaps();
            }
            _map1 = new Map(this, "room1.xml", null);
            _interfaceElements = new List<UIElement>();
            base.Initialize();
        }

        /// <summary>
        /// A method to generate three rooms and save them to xml files
        /// </summary>
        public void GenerateMaps()
        {
            List<Exit> exits = new List<Exit>();
            Random r = new Random();
            Generator tempMap = null;

            for (int i = 1; i < 4; i++) //3 Level a 3 Räume
            {
                int minX = 7;
                int minY = 8;
                if (exits.Count > 0) minX += exits[0].from.x;
                if (exits.Count > 0) minY += exits[0].from.y;
                tempMap = new Generator(this, minX + r.Next(8), minY + r.Next(8), true, null, i, 3, exits, r);
                tempMap.Save("room" + i.ToString() + ".xml");
                exits = Map.ExitToEntry(i + 1, tempMap.exits);
                tempMap.Dispose();
            }
        }

        /// <summary>
        /// Cache Content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("font");
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
            // _backMusic = Content.Load<Song>("Video Dungeon Crawl.wav"); // Todo: *.mp3
            // _font = Content.Load<SpriteFont>("Font");
            // MediaPlayer.Volume = (float)0.3;
            // MediaPlayer.Play(_backMusic);            
        }

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

        /// <summary>
        /// Unload all managed content which has not been disposed of elsewhere
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Methode to delete old saved rooms
        /// </summary>
        public void DeleteSavedRooms()
        {
            if (File.Exists("savedroom1.xml"))
            {
                try
                {
                    File.Delete("savedroom1.xml");
                    File.Delete("savedroom2.xml");
                    File.Delete("savedroom3.xml");
                }
                catch
                {
                    Exit();
                }
            }
        }

        /// <summary>
        /// Handle events from UIElements and/or backend objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventID"></param>
        /// <param name="data"></param>
        public void HandleEvent(UIElement sender, Events eventID, params object[] data)
        {

            switch (eventID)
            {
                case Events.Player1:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(null, Events.ToggleButton, Events.Player2, false);
                        element.HandleEvent(null, Events.ToggleButton, Events.Player1, true);
                        element.HandleEvent(null, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(null, Events.ToggleButton, Events.LAN, false);
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
                        element.HandleEvent(null, Events.ToggleButton, Events.Player2, true);
                        element.HandleEvent(null, Events.ToggleButton, Events.Player1, false);
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
                        element.HandleEvent(null, Events.ToggleButton, Events.Player2, true);
                        element.HandleEvent(null, Events.ToggleButton, Events.Player1, false);
                        element.HandleEvent(null, Events.ToggleButton, Events.Local, false);
                        element.HandleEvent(null, Events.ToggleButton, Events.LAN, true);
                    }
                    _mainmap2.enabled = false;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));
                    break;
                case Events.Local:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(null, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(null, Events.ToggleButton, Events.LAN, false);
                    }
                    _lan = false;
                    _mainmap2.enabled = true;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20) / 2) - 60));
                    break;
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
                    DeleteSavedRooms();
                    Exit();
                    break;

                case Events.ChangeMap:
                    _status = GameStatus.NoRedraw;
                    _map1.Save("savedroom" + _map1.currRoomNbr + ".xml");//+Nummer
                    if (File.Exists("saved" + (string)data[0]))
                        _map1.Load("saved" + (string)data[0], (Coords)data[1]);
                    else
                        _map1.Load((string)data[0], (Coords)data[1]);
                    _mainmap1.resetActors();
                    AddMessage("You entered room number " + data[0].ToString().Substring(4, 1) + ".");
                    _status = GameStatus.Running;
                    break;

                case Events.NewMap:
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    GenerateMaps();
                    DeleteSavedRooms();
                    _map1.Load("room1.xml", null);
                    _mainmap1.resetActors();
                    _status = GameStatus.Paused;
                    HandleEvent(null, Events.ContinueGame);
                    break;

                case Events.ResetGame:
                    DeleteSavedRooms();
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    _map1.Load("room1.xml", null);
                    _mainmap1.resetActors();
                    _status = GameStatus.Paused;
                    HandleEvent(null, Events.ContinueGame);
                    break;

                case Events.ShowMessage:
                    AddMessage(data[0].ToString());
                    break;

                case Events.FinishedAnimation:
                    int FinishedID = (int)data[0];
                    Activity FinishedActivity = (Activity)data[1];
                    if (FinishedActivity == Activity.Die)
                    {
                        if (_map1.actors[FinishedID] is Enemy)
                        {
                            ((ActorTile)_map1.actors[FinishedID].tile).enabled = false;
                            AddMessage(_map1.actors[FinishedID].name + " is dead.");
                        }
                        else
                        {
                            AddMessage("<red>You are dead.");
                            RemoveHealth();
                            ShowEndGame();
                        }
                    }
                    break;

                case Events.MoveActor:
                    int id = (int)data[0];
                    if (!_mainmap1.IsMoving(id))
                    {
                        Direction dir = (Direction)data[1];
                        _mainmap1.ChangeDir(id, dir);
                        Coords target = _map1.DirectionTile(_map1.actors[id].tile.coords, dir);
                        if ((id == 0) && (_map1[target.x, target.y].hasTeleport))
                        {
                            HandleEvent(null, Events.ChangeMap, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextRoom, ((TeleportTile)_map1[target.x, target.y].overlay[0]).nextPlayerPos);
                        }


                        if ((_map1[target.x, target.y].hasEnemy) || (_map1[target.x, target.y].hasPlayer))
                        {
                            if ((_map1.firstActorID(target.x, target.y) != id) && (!_map1[target.x, target.y].firstActor.IsDead()))
                            {
                                // Display enemy statistics
                                if (_map1[target.x, target.y].firstActor is Player)
                                {
                                    _enemyStats.actor = _map1.actors[id]; // Enemy attacked
                                }
                                else
                                {
                                    if (id == 0)
                                    {
                                        _enemyStats.actor = _map1[target.x, target.y].firstActor; // Player attacked enemy
                                    }
                                }
                                // Aktuelle Figur attackiert
                                // Spieler verletzt
                                // oder tot
                                _map1[target.x, target.y].firstActor.SetDamage(_map1.actors[id]);
                                if (_map1[target.x, target.y].firstActor is Player) RemoveHealth();
                                if (_map1[target.x, target.y].firstActor.IsDead())
                                {
                                    _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Die, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage((_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " killed " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name) + " doing " + _map1.actors[id].damage.ToString() + " points of damage.");

                                }
                                else
                                {
                                    _mainmap1.HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Hit, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + (_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " attacked " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + "The attack caused " + (_map1[target.x, target.y].firstActor.armour - _map1.actors[id].damage).ToString() + " points of damage (" + _map1.actors[id].damage.ToString() + " attack strength - " + _map1[target.x, target.y].firstActor.armour + " defense)");
                                }
                                _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Attack, false, dir, true);
                            }
                        }
                        else
                        {
                            if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                            {
                                _map1.MoveActor(_map1.actors[id], dir);
                                if (_map1.actors[id] is Player)
                                    _minimap1.MoveCamera(_map1.actors[id].tile.coords);
                                _mainmap1.HandleEvent(null, Events.MoveActor, id, _map1.actors[id].tile.coords);

                                if (_map1[target.x, target.y].hasTreasure)
                                {
                                    while (_map1[target.x, target.y].hasTreasure)
                                    {
                                        _map1._actors[id].inventory.Add(_map1[target.x, target.y].firstItem.item);
                                        AddMessage((_map1.actors[id] is Player ? "You found " : _map1.actors[id].name + " found ") + _map1[target.x, target.y].firstItem.item.name + " .");
                                        _map1[target.x, target.y].firstItem.item.EquipItem(_map1._actors[id]);
                                        _map1[target.x, target.y].Remove(_map1[target.x, target.y].firstItem);
                                    }
                                }
                                else
                                {
                                    if (_map1[target.x, target.y].hasTrap)
                                    {
                                        _map1.actors[id].SetDamage(_map1[target.x, target.y].trapDamage);
                                        if (_map1.actors[id].IsDead())
                                        {
                                            _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Die, true, dir, true);
                                            AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + " was") + " killed by a trap  doing " + (_map1[target.x, target.y].trapDamage - _map1.actors[id].armour).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armour + " protection)");

                                        }
                                        else
                                        {
                                            _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Hit, true, dir, true);
                                            AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + "  was") + " hit for " + (_map1[target.x, target.y].trapDamage - _map1.actors[id].armour).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armour + " protection)");
                                        }
                                        if (_map1._actors[id] is Player) RemoveHealth();

                                    }
                                    else
                                    {
                                        if ((_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget) && (id == 0))
                                        {
                                            _mainmap1.HandleEvent(null, Events.AnimateActor, id, Activity.Talk, true);
                                            ShowEndGame("You have successfully found the hidden treasure. Can you do it again?", "Congratulations!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

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
            if ((!_updating) && (_status != GameStatus.FetchingData))
            {
                {
                    _updating = true;

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
            catch (Exception e)
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

        public void LoadSaveGame(string filename = "autosave")
        {
            throw new NotImplementedException("TODO!");
        }

        /// <summary>
        /// Save the running Game to continue later.
        /// </summary>
        /// <param name="filename"></param>
        public void SaveGame(string filename = "autosave")
        {
            if (!Directory.Exists(Environment.CurrentDirectory + @"\SaveGames"))
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\SaveGames");
            //TODO: xml file with a list of all savegames? or any dir IS a savegame
            string filedir = Environment.CurrentDirectory + @"\SaveGames\" + filename;
            if (!Directory.Exists(filedir))
                Directory.CreateDirectory(filedir);
            //Save the Gamedata
            //TODO: Dynamic levels and rooms (List of rooms, levels)
            if (File.Exists("room1.xml")) File.Copy("room1.xml", filedir + @"\room1.xml", true);
            if (File.Exists("room2.xml")) File.Copy("room2.xml", filedir + @"\room2.xml", true);
            if (File.Exists("room3.xml")) File.Copy("room3.xml", filedir + @"\room3.xml", true);
            if (File.Exists("savedroom1.xml")) File.Copy("savedroom1.xml", filedir + @"\savedroom1.xml", true);
            if (File.Exists("savedroom2.xml")) File.Copy("savedroom2.xml", filedir + @"\savedroom2.xml", true);
            if (File.Exists("savedroom3.xml")) File.Copy("savedroom3.xml", filedir + @"\savedroom3.xml", true);
            //create savegame.xml with necessary information
            XmlWriter xmlw = XmlWriter.Create(filedir + @"\savegame.xml");
            //TODO: write the current room and other values which are required to load a savegame
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("SaveGame");
            xmlw.WriteAttributeString("id", filename);
            xmlw.WriteStartElement("Player1");
            xmlw.WriteAttributeString("info", "TODO!");
            xmlw.WriteEndElement();
            xmlw.WriteEndElement();
            xmlw.WriteEndDocument();
            xmlw.Close();
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
            _backgroundcolor = Color.Black;
            _events = new StateToEvent(this);
            //TODO: delete
            //SaveGame();
            //Exit();
            _graphics = new GraphicsDeviceManager(this);
             _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
             _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 40;
            _files2fetch = new Queue<String>();
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


