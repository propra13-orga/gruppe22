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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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
        /// Count deads of player to load checkpoint or show gameover
        /// </summary>
        protected int _deadcounter = -1;

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
        /// A minimap
        /// </summary>
        protected Minimap _minimap1 = null;

        /// <summary>
        /// The statusbox listing all messages / events
        /// </summary>
        protected Statusbox _statusbox = null;


        protected Orb _health = null;
        protected Orb _mana = null;
        protected Toolbar _toolbar = null;

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

        private GridElement _draggedObject = null;
        /// <summary>
        /// List of all sounds using in the Game
        /// </summary>
        protected List<SoundEffect> soundEffects = null;
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
            _mainmap1 = new Mainmap(this, _spriteBatch, Content, new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 125), _map1, true);
            _interfaceElements.Add(_mainmap1);
            _statusbox = new Statusbox(this, _spriteBatch, Content, new Rectangle(145, _graphics.GraphicsDevice.Viewport.Height - 100, _graphics.GraphicsDevice.Viewport.Width - 525, 95));
            _interfaceElements.Add(_statusbox);
            _statusbox.AddLine("Welcome to this highly innovative Dungeon Crawler!\nYou can scroll in this status window.\nUse A-S-D-W to move your character.\n Use Arrow keys (or drag mouse) to scroll map or minimap\n Press ESC to display Game Menu.");

            _toolbar = new Toolbar(this, _spriteBatch, Content, new Rectangle((_graphics.GraphicsDevice.Viewport.Width - 490) / 2, _graphics.GraphicsDevice.Viewport.Height - 140, (_graphics.GraphicsDevice.Viewport.Width - 490) / 2, 34), _map1.actors[0]);
            _interfaceElements.Add(_toolbar);

            _health = new Orb(this, _spriteBatch, Content, new Rectangle(5, _graphics.GraphicsDevice.Viewport.Height - 139, 90, 90), _map1.actors[0], false);
            _interfaceElements.Add(_health);

            _mana = new Orb(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 380, _graphics.GraphicsDevice.Viewport.Height - 139, 90, 90), _map1.actors[0], true);
            _interfaceElements.Add(_mana);

            if (_map1.actors[0].health < 1)
            {
                ShowEndGame();
            }

            //Load Sound
            soundEffects = new List<SoundEffect>();
            SoundEffect tmp = Content.Load<SoundEffect>("changemap1.wav");
            soundEffects.Add(tmp);
            tmp = Content.Load<SoundEffect>("checkpoint1.wav");
            soundEffects.Add(tmp);
            tmp = Content.Load<SoundEffect>("pickup1.wav");
            soundEffects.Add(tmp);
            tmp = Content.Load<SoundEffect>("trap1.wav");
            soundEffects.Add(tmp);
            tmp = Content.Load<SoundEffect>("trapdamage1.wav");
            soundEffects.Add(tmp);
            // ShowCharacterWindow(_map1.actors[0]);
            // ShowShopWindow(_map1.actors[0], _map1.actors[1]);
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
        public bool OnKeyDown(Keys k)
        {
            if (_status != GameStatus.GameOver)
            {
                for (int i = 0; i < _interfaceElements.Count; ++i)
                {
                    _interfaceElements[i].OnKeyDown(k);
                }
                return true;
            }
            else
            {
                if (_focus != null)
                    _focus.OnKeyDown(k);
                return true;
            }
        }

        private void HandleMovementKeys(Keys k)
        {
            switch (k)
            {
                case Keys.Up:
                    _mainmap1.MovePlayer(Direction.Up);
                    break;
                case Keys.Left:
                    _mainmap1.MovePlayer(Direction.Left);
                    break;
                case Keys.Right:
                    _mainmap1.MovePlayer(Direction.Right);
                    break;
                case Keys.Down:
                    _mainmap1.MovePlayer(Direction.Down);
                    break;
                case Keys.Space:
                    _mainmap1.FireProjectile();
                    break;

            }
        }

        public bool OnKeyUp(Keys k)
        {
            return true;
        }

        public bool OnMouseDown(int button)
        {

            if (_status != GameStatus.GameOver)
            {
                for (int i = 0; i < _interfaceElements.Count; ++i)
                {
                    if (_interfaceElements[i].OnMouseDown(button)) return true;
                }
                return true;
            }
            else
            {

                if (_focus != null)
                    _focus.OnMouseDown(button);
                return true;
            }
        }

        public bool OnMouseUp(int button)
        {

            if (_status != GameStatus.GameOver)
            {
                for (int i = 0; i < _interfaceElements.Count; ++i)
                {
                    if (_interfaceElements[i].OnMouseUp(button))
                    {
                        _draggedObject = null;
                        return true;
                    }
                }
                _draggedObject = null;
                return true;
            }
            else
            {
                _draggedObject = null;
                if (_focus != null)
                    _focus.OnMouseUp(button);
                return true;
            }
        }

        public bool OnMouseHeld(int button)
        {
            return true;
        }

        public bool OnKeyHeld(Keys k)
        {
            return true;
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
                case Events.AddDragItem:
                    _draggedObject = (GridElement)data[0];
                    break;
                case Events.ShowMenu:
                    if (_focus is CharacterWindow)
                    {
                        _focus.Dispose();
                        _interfaceElements.Remove(_focus);
                        _toolbar.HandleEvent(true, Events.ContinueGame, 13);
                        _status = GameStatus.Running;
                    }
                    if (_status == GameStatus.Running) ShowMenu();
                    else HandleEvent(true, Events.ContinueGame, 0);
                    break;
                case Events.ShowCharacter:
                    if (_status == GameStatus.Running) ShowCharacterWindow((Actor)data[0], 0);
                    else
                    {
                        if (_focus is CharacterWindow)
                        {
                            if (((CharacterWindow)_focus).page != 0)
                            {
                                ((CharacterWindow)_focus).page = 0;
                                _toolbar.HandleEvent(true, Events.ContinueGame, 10);
                                return;
                            }
                            HandleEvent(true, Events.ContinueGame, 0);
                        }
                        else
                        {
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;
                            _status = GameStatus.Running;
                            ShowCharacterWindow((Actor)data[0], 0);
                            _toolbar.HandleEvent(true, Events.ContinueGame, 10);
                        }
                    }
                    break;
                case Events.ShowAbilities: if (_status == GameStatus.Running) ShowCharacterWindow((Actor)data[0], 2);
                    else
                    {
                        if (_focus is CharacterWindow)
                        {
                            if (((CharacterWindow)_focus).page != 2)
                            {
                                ((CharacterWindow)_focus).page = 2;
                                _toolbar.HandleEvent(true, Events.ContinueGame, 12);
                                return;
                            }
                            HandleEvent(true, Events.ContinueGame, 0);
                        }
                        else
                        {
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;
                            _toolbar.HandleEvent(true, Events.ContinueGame, 12);
                            _status = GameStatus.Running;
                            ShowCharacterWindow((Actor)data[0], 2);
                        }
                    }
                    break;
                case Events.ShowInventory:
                    if (_status == GameStatus.Running) ShowCharacterWindow((Actor)data[0], 1);
                    else
                    {
                        if (_focus is CharacterWindow)
                        {
                            if (((CharacterWindow)_focus).page != 1)
                            {
                                ((CharacterWindow)_focus).page = 1;
                                _toolbar.HandleEvent(true, Events.ContinueGame, 11);
                                return;
                            }
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;
                            _status = GameStatus.Running;
                            HandleEvent(true, Events.ContinueGame, 0);
                        }
                        else
                        {
                            _toolbar.HandleEvent(true, Events.ContinueGame, 11);
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;
                            _status = GameStatus.Running;
                            ShowCharacterWindow((Actor)data[0], 1);
                        }
                    }
                    break;
                case Events.Player1:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Player2, false);
                        element.HandleEvent(true, Events.ToggleButton, Events.Player1, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.LAN, false);
                    }
                    _secondPlayer = false;
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
                        // _mainmap2.enabled = true;
                        _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20) / 2) - 60));
                    }
                    else
                    {
                        //   _mainmap2.enabled = false;
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
                    // _mainmap2.enabled = false;
                    _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));
                    break;

                case Events.Local:
                    foreach (UIElement element in _interfaceElements)
                    {
                        element.HandleEvent(true, Events.ToggleButton, Events.Local, true);
                        element.HandleEvent(true, Events.ToggleButton, Events.LAN, false);
                    }
                    _lan = false;
                    //    _mainmap2.enabled = true;
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
                        _toolbar.HandleEvent(true, Events.ContinueGame, -1);
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            _mainmap1.noMove = true;
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
                    _map1.Save("savedroom" + _map1.id + ".xml");
                    Exit();
                    break;

                case Events.NewMap:
                    _status = GameStatus.NoRedraw;
                    _deadcounter = -1;
                    (this as GameLogic).GenerateMaps();
                    HandleEvent(true, Events.ResetGame);
                    break;

                case Events.ResetGame:
                    _status = GameStatus.NoRedraw;
                    _deadcounter = -1;
                    DeleteSavedRooms();
                    _map1.Dispose();
                    _map1.Load("room1.xml", null);
                    _mainmap1.resetActors();
                    //  _mainmap2.resetActors();
                    _mana.actor = _map1.actors[0];
                    _health.actor = _map1.actors[0];
                    _toolbar.actor = _map1.actors[0];
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

        #endregion


        #region Default subwindows
        /// <summary>
        /// Display Main Menu
        /// </summary>
        public void ShowMenu()
        {
            _status = GameStatus.Paused;
            Window _mainMenu = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 180) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 250, 320, 500));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 240, 300, 60), "Continue", (int)Buttons.Close));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 170, 140, 60), "Restart", (int)Buttons.Restart));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 170, 140, 60), "New Maps", (int)Buttons.NewMap));
            /*
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 140, 60), "1 Player", (int)Buttons.SinglePlayer, !_secondPlayer));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 140, 60), "2 Players", (int)Buttons.TwoPlayers, _secondPlayer));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 30, 140, 60), "Local", (int)Buttons.Local, !_lan));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f) + 160, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 30, 140, 60), "LAN", (int)Buttons.LAN, _lan));

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 40, 300, 60), "Settings", (int)Buttons.Settings));
*/

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 110, 300, 60), "Credits", (int)Buttons.Credits));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 180, 300, 60), "Quit", (int)Buttons.Quit));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));



            _interfaceElements.Add(_mainMenu);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        public void ShowCharacterWindow(Actor actor, uint page = 0)
        {
            if (!(_focus is CharacterWindow))
            {
                _status = GameStatus.Paused;
                CharacterWindow c = new CharacterWindow(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 250) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 240, 500, 480), actor);
                _interfaceElements.Add(c);
                c.page = page;
                _focus = _interfaceElements[_interfaceElements.Count - 1];
            }
            else
            {
                (_focus as CharacterWindow).page = page;
            }
        }


        public void ShowShopWindow(Actor actor1, Actor actor2)
        {
            if (!(_focus is CharacterWindow))
            {
                _status = GameStatus.Paused;
                Shop c = new Shop(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 250) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 240, 500, 480), actor1, actor2);
                _interfaceElements.Add(c);
                _focus = _interfaceElements[_interfaceElements.Count - 1];
            }
        }

        /// <summary>
        /// A text displayed if the player died
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public void ShowEndGame(string message = "You have failed in your mission. Better luck next time.", string title = "Game over!")
        {
            _status = GameStatus.GameOver;
            _map1.Save("savedroom" + _map1.id + ".xml");
            Window _gameOver = new Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Statusbox stat = new Statusbox(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 110), false, true);
            stat.AddLine(title + "\n \n" + message);
            _gameOver.AddChild(stat);
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 130, 40), "New Maps", (int)Buttons.Reset));

            if (_deadcounter > 0)
            {
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 170, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 160, 40), "Restore (" + _deadcounter.ToString() + " left)", (int)Buttons.Load));
            }
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 600 - 190, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 100, 40), "Restart", (int)Buttons.Restart));
            _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 600 - 80, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 70, 40), "Quit", (int)Buttons.Quit));
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

            _about.AddChild(new Button(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 80, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 140, 160, 40), "Ok", (int)Buttons.Close));

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
            if (_status != GameStatus.GameOver)
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
            else
            {
                _events.Update(gameTime);

                if (_focus != null)
                {
                    _focus.Update(gameTime);
                }
            }

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
                if (_draggedObject != null)
                {
                    Point mousePos = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    _spriteBatch.Draw(_draggedObject.icon.texture, new Rectangle(mousePos.X, mousePos.Y, _draggedObject.icon.clipRect.Width, _draggedObject.icon.clipRect.Height), _draggedObject.icon.clipRect, Color.White);
                    _spriteBatch.End();
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
