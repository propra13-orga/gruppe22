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
        MoveActor,
        ChangeMap,
        NewMap,
        ResetGame,
        About,
        AnimateActor,
        FinishedAnimation,
        ShowMessage
    }

    public enum GameStatus
    {
        Running,
        NoRedraw,
        Paused
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
        /// Whether the game is paused (for menus etc.)
        /// </summary>
        private GameStatus _status = GameStatus.Running;

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
        #endregion


        #region Implementation of IKeyHandler-Interface
        public void OnKeyDown(Keys k)
        {
        }

        public void OnKeyUp(Keys k)
        {
        }

        public void OnMouseDown(int button)
        {
        }

        public void OnMouseUp(int button)
        {
        }

        public void OnMouseHeld(int button)
        {
        }

        public void OnKeyHeld(Keys k)
        {
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
            tempMap = new Generator(this, 7 + r.Next(8), 8 + r.Next(8), true, null, 1, 3, null, r);
            tempMap.Save("room1.xml");
            exits = Map.ExitToEntry(2, tempMap.exits);
            tempMap.Dispose();
            tempMap = new Generator(this, r.Next(8) + 4 + exits[0].from.x, r.Next(8) + 4 + exits[0].from.y, true, null, 2, 3, exits, r);
            tempMap.Save("room2.xml");
            exits = Map.ExitToEntry(3, tempMap.exits);
            tempMap.Dispose();
            tempMap = new Generator(this, r.Next(10) + 8 + exits[0].from.x, r.Next(10) + 8 + exits[0].from.y, true, null, 3, 3, exits, r);
            tempMap.Save("room3.xml");
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
            _interfaceElements.Add(new Inventory(this, _spriteBatch, Content, new Rectangle(20, _graphics.GraphicsDevice.Viewport.Height - 120, _graphics.GraphicsDevice.Viewport.Width - 260, 100)),_map1.actors[0]);
            (_interfaceElements[2] as Statusbox).AddLine("Welcome to this highly innovative Dungeon Crawler!\nYou can scroll in this status window.\nUse A-S-D-W to move your character.\n Use Arrow keys (or drag mouse) to scroll map or minimap\n Press ESC to display Game Menu.");
            _playerStats = new SimpleStats(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 230, 240, 210, 50), _map1._actors[0]);
            _enemyStats = new SimpleStats(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 230, 240, 210, 50), null);
            _interfaceElements.Add(_playerStats);
            _interfaceElements.Add(_enemyStats);            
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

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 180, 300, 60), "Continue", Events.ContinueGame));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 300, 60), "Restart", Events.ResetGame));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 20, 300, 60), "New Maps", Events.NewMap));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 120, 300, 60), "Credits", Events.About));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 220, 300, 60), "Quit", Events.EndGame));

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
            Window _gameOver = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Statusbox stat = new Statusbox(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 110), false, true);
            stat.AddLine(title + "\n \n" + message);
            _gameOver.AddChild(stat);
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "New Maps", Events.NewMap));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 180, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Restart", Events.ResetGame));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 300) / 2.0f) + 600 - 170, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Quit", Events.EndGame));

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
                    ((Mainmap)_interfaceElements[1]).resetActors();
                    AddMessage("You entered room number " + data[0].ToString().Substring(4, 1) + ".");
                    _status = GameStatus.Running;
                    break;

                case Events.NewMap:
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    GenerateMaps();
                    DeleteSavedRooms();
                    _map1.Load("room1.xml", null);
                    ((Mainmap)_interfaceElements[1]).resetActors();
                    _status = GameStatus.Paused;
                    HandleEvent(null, Events.ContinueGame);
                    break;

                case Events.ResetGame:
                    DeleteSavedRooms();
                    _status = GameStatus.NoRedraw;
                    _map1.Dispose();
                    _map1.Load("room1.xml", null);
                    ((Mainmap)_interfaceElements[1]).resetActors();
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
                    if (!((Mainmap)_interfaceElements[1]).IsMoving(id))
                    {
                        Direction dir = (Direction)data[1];
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
                                    ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Die, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage((_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " killed " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name) + " doing " + _map1.actors[id].damage.ToString() + " points of damage.");

                                }
                                else
                                {
                                    ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, _map1.firstActorID(target.x, target.y), Activity.Hit, false, Map.WhichWayIs(_map1.actors[id].tile.coords, target));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + (_map1.actors[id] is Player ? "<green>You" : _map1.actors[id].name) + " attacked " + (_map1.actors[_map1.firstActorID(target.x, target.y)] is Player ? "you" : _map1.actors[_map1.firstActorID(target.x, target.y)].name));
                                    AddMessage(((_map1.actors[_map1.firstActorID(target.x, target.y)] is Player) ? "<red>" : "") + "The attack caused " + (_map1[target.x, target.y].firstActor.armour - _map1.actors[id].damage).ToString() + " points of damage (" + _map1.actors[id].damage.ToString() + " attack strength - " + _map1[target.x, target.y].firstActor.armour + " defense)");
                                }
                                ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, id, Activity.Attack, false, dir, true);
                            }
                        }
                        else
                        {
                            if (_map1.CanMove(_map1.actors[id].tile.coords, dir))
                            {
                                _map1.MoveActor(_map1.actors[id], dir);
                                if (_map1.actors[id] is Player)
                                    ((Minimap)_interfaceElements[0]).MoveCamera(_map1.actors[id].tile.coords);
                                ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.MoveActor, id, _map1.actors[id].tile.coords);

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
                                            ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, id, Activity.Die, true, dir, true);
                                            AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + " was") + " killed by a trap  doing " + (_map1.actors[id].armour - _map1[target.x, target.y].trapDamage).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armour + "protection)");

                                        }
                                        else
                                        {
                                            ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, id, Activity.Hit, true, dir, true);
                                            AddMessage((_map1.actors[id] is Player ? "You were" : _map1.actors[id].name + "  was") + " hit for " + _map1[target.x, target.y].trapDamage.ToString() + " points of damage (" + (_map1.actors[id].armour - _map1[target.x, target.y].trapDamage).ToString() + " points of damage (" + _map1[target.x, target.y].trapDamage.ToString() + " - " + _map1.actors[id].armour + "protection)");
                                        }
                                        if (_map1._actors[id] is Player) RemoveHealth();

                                    }
                                    else
                                    {
                                        if ((_map1[_map1.actors[id].tile.coords.x, _map1.actors[id].tile.coords.y].hasTarget) && (id == 0))
                                        {
                                            ((Mainmap)_interfaceElements[1]).HandleEvent(null, Events.AnimateActor, id, Activity.Talk, true);
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
            foreach (UIElement element in _interfaceElements)
            {
                if (element is Statusbox)
                {
                    ((Statusbox)element).AddLine(s);

                    return;
                }
            }
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
            if (!_updating)
            {
                if (_backgroundcolor.R > 0) // Remove Red Tint
                {
                    _backgroundcolor.R -= 1;
                };
                if (_backgroundcolor.G > 0) // Remove Green Tint
                {
                    _backgroundcolor.G -= 1;
                };
                _updating = true;
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


                    if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
                    {
                        
                            if (_status == GameStatus.Running) ShowMenu();
                            else HandleEvent(null, Events.ContinueGame, 0);                        
                    }
                }

                base.Update(gameTime);
                _updating = false;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!_drawing)
            {
                _drawing = true;
                BeginDraw();
                if (_status != GameStatus.NoRedraw)
                {
                    GraphicsDevice.Clear(_backgroundcolor);

                    foreach (UIElement element in _interfaceElements)
                    {
                        element.Draw(gameTime);
                    }
                    base.Draw(gameTime);
                }
                EndDraw();
                _drawing = false;
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
            _backgroundcolor = Color.Black;
            _graphics = new GraphicsDeviceManager(this);
            // _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 200;
            // _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 40;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.PreferredBackBufferWidth = 768;

            _graphics.IsFullScreen = true;
            Window.AllowUserResizing = false;
            //  Type type = typeof(OpenTKGameWindow);

            // _graphics.SynchronizeWithVerticalRetrace = false;            

            // Move window to top left corner of the screen
            // System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //   OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(this.Window);
            // window.X = 0;
            // window.Y = 0;
        }
        #endregion

    }
}
