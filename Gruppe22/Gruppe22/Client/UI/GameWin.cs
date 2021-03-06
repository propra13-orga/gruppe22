﻿using System;
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
using Lidgren.Network;
using Gruppe22.Backend;
using System.Text.RegularExpressions;

namespace Gruppe22.Client
{
    /// <summary>
    /// Handle all UI related operations of the game
    /// </summary>
    public class GameWin : Game, Backend.IHandleEvent, IKeyHandler
    {
        #region Private Fields

        private Backend.Logic _logic = null;
        /// <summary>
        /// Central Output device
        /// </summary>
        protected GraphicsDeviceManager _graphics = null;

        /// <summary>
        /// Central Sprite drawing algorithm
        /// </summary>
        private SpriteBatch _spriteBatch = null;

        /// <summary>
        /// Unique ID of current player Character
        /// </summary>
        private int _playerID = 0;

        /// <summary>
        /// Current mousewheel position (used to calculate changes)
        /// </summary>
        private int _mouseWheel = 0;

        /// <summary>
        /// Whether the user is currently dragging something
        /// </summary>
        private bool _dragging = false;

        private bool _hideUI = false;
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
        /// Whether the game is paused (for menus etc.)
        /// </summary>
        private Backend.GameStatus _status = Backend.GameStatus.NoRedraw;

        /// <summary>
        /// A list of files to download
        /// </summary>
        private Queue<string> _files2fetch;

        private Song _backMusic;

        /// <summary>
        /// Previous state (to reset after all files are downloaded)
        /// </summary>
        private Backend.GameStatus _prevState;

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
        /// A spritefont used to display information on screen
        /// </summary>
        private SpriteFont _font;

        /// <summary>
        /// The main map used for player 1
        /// </summary>
        private Mainmap _mainmap1 = null;

        /// <summary>
        /// A minimap
        /// </summary>
        private Minimap _minimap1 = null;

        /// <summary>
        /// The statusbox listing all messages / events
        /// </summary>
        private TextOutput _statusbox = null;


        private Orb _health = null;
        private Orb _mana = null;
        private Toolbar _toolbar = null;

        /// <summary>
        /// Change-based handling of events (i.e. keyup/keydown) instead of status based ("Is key pressed?")
        /// </summary>
        private StateToEvent _events = null;

        /// <summary>
        /// Whether we are playing by network (i.e. communicating with a server)
        /// </summary>
        private bool _lan = false;

        //private Texture2D _playerTexture; das machen wir anders
        //private SpriteFont _font; das haben wir schon

        /// <summary>
        /// Random number generator
        /// </summary>
        private Random r = null;

        private GridElement _draggedObject = null;

        /// <summary>
        /// Descriptive text stating which file is currently downloading
        /// </summary>
        private string _downloading = "";

        #endregion


        #region MonoGame default functions (overriden)
        /// <summary>
        /// Set up the (non visible) objects of the game
        /// </summary>
        protected override void Initialize()
        {
            _interfaceElements = new List<UIElement>(); // Initialize the list of UI elements (but not the objects themselves, see LoadContent)
            _logic = new PureLogic(this, null); // Start in Client Mode
            base.Initialize();
        }

        /// <summary>
        /// Setup user interface
        /// </summary>
        private void SetupGame()
        {


            _mainmap1 = new Mainmap(this, _spriteBatch, Content, new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 5, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 125), _logic.map, true);
            _interfaceElements.Add(_mainmap1);

            _toolbar = new Toolbar(this, _spriteBatch, Content, new Rectangle((_graphics.GraphicsDevice.Viewport.Width - 490) / 2, _graphics.GraphicsDevice.Viewport.Height - 140, (_graphics.GraphicsDevice.Viewport.Width - 490) / 2, 34), _logic.map.actors[_playerID]);
            _interfaceElements.Add(_toolbar);
            _minimap1 = new Minimap(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 220, 5, 215, 215), _logic.map);
            _interfaceElements.Add(_minimap1);

            _statusbox = new Chat(this, _spriteBatch, Content, new Rectangle(145, _graphics.GraphicsDevice.Viewport.Height - 100, _graphics.GraphicsDevice.Viewport.Width - 525, 95));
            _interfaceElements.Add(_statusbox);

            _health = new Orb(this, _spriteBatch, Content, new Rectangle(5, _graphics.GraphicsDevice.Viewport.Height - 139, 90, 90), _logic.map.actors[_playerID], false);
            _interfaceElements.Add(_health);

            _mana = new Orb(this, _spriteBatch, Content, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 380, _graphics.GraphicsDevice.Viewport.Height - 139, 90, 90), _logic.map.actors[_playerID], true);
            _interfaceElements.Add(_mana);

            _logic.HandleEvent(true, Events.Initialize);
        }

        /// <summary>
        /// Cache Content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);             // Create a new SpriteBatch, which can be used to draw textures.
            _font = Content.Load<SpriteFont>("font"); // Load the font
            if (!System.IO.File.Exists(".\\Content\\shop.wav"))
            {
                _status = Backend.GameStatus.Loading;
                foreach (string s in System.IO.File.ReadAllLines(".\\content\\filestodownload.txt"))
                {
                    if (!System.IO.File.Exists(".\\Content\\" + s))
                    {
                        _files2fetch.Enqueue(s);
                    }
                }
                _prevState = _status;
                _status = Backend.GameStatus.FetchingData;
                if (_files2fetch.Count > 0)
                    _LoadFile(_files2fetch.Dequeue(), wc_DownloadProgressChanged, wc_DownloadFileCompleted);
            }
            else
            {
                SetupGame();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if ((_status == GameStatus.Running) || (_logic is NetLogic))
                _logic.Update(gameTime);
            if ((_backMusic != null) && ((MediaPlayer.State == MediaState.Paused) || ((MediaPlayer.State == MediaState.Stopped))))
            {
                MediaPlayer.Play(_backMusic);
            }
            if (_status != GameStatus.NoRedraw)
            {
                if ((_logic.map.actors[_playerID].health < 1) && (_status != Backend.GameStatus.GameOver) && (_status != Backend.GameStatus.Paused))
                {
                    _status = Backend.GameStatus.GameOver;
                    _ShowEndGame();
                }
                else
                    if (_status != Backend.GameStatus.GameOver)
                    {
                        foreach (Keys k in _events.keys)
                        {
                            switch (k)
                            {
                                case Keys.T:
                                    _statusbox.focus = true;
                                    break;
                                case Keys.Up:
                                    HandleEvent(true, Backend.Events.MoveActor, _playerID, Backend.Direction.Up, _logic.map.actors[_playerID].moveIndex);
                                    break;
                                case Keys.Left:
                                    HandleEvent(true, Backend.Events.MoveActor, _playerID, Backend.Direction.Left, _logic.map.actors[_playerID].moveIndex);
                                    break;
                                case Keys.Right:
                                    HandleEvent(true, Backend.Events.MoveActor, _playerID, Backend.Direction.Right, _logic.map.actors[_playerID].moveIndex);
                                    break;
                                case Keys.Down:
                                    HandleEvent(true, Backend.Events.MoveActor, _playerID, Backend.Direction.Down, _logic.map.actors[_playerID].moveIndex);
                                    break;
                                case Keys.Space:
                                    HandleEvent(true, Backend.Events.MoveProjectile, null, _logic.map.actors[_playerID].tile.parent, _logic.map.actors[_playerID].direction);
                                    break;

                            }

                        }

                        if ((!_updating) && (_status != Backend.GameStatus.FetchingData))
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

                                    if (_status == Backend.GameStatus.Running || ((_status == Backend.GameStatus.Paused) && (element.ignorePause)))
                                        element.Update(gameTime);
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





                                }

                                if (Mouse.GetState().LeftButton != ButtonState.Pressed)
                                {
                                    _mousepos.X = -1;
                                    _mousepos.Y = -1;

                                    _dragging = false;
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
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_status != Backend.GameStatus.FetchingData)
            {
                if (!_drawing)
                {
                    _drawing = true;
                    if (_status != Backend.GameStatus.NoRedraw)
                    {
                        GraphicsDevice.Clear(_backgroundcolor);

                        for (int i = 0; i < _interfaceElements.Count; ++i)
                        {
                            _interfaceElements[i].Draw(gameTime);
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
                Vector2 position = new Vector2((GraphicsDevice.Viewport.Width - _font.MeasureString(text).X) / 2, (GraphicsDevice.Viewport.Height) / 2 - _font.MeasureString(text).Y);
                _spriteBatch.DrawString(_font, text, position, Color.Gray);
                _spriteBatch.DrawString(_font, text, new Vector2(position.X - 2, position.Y - 2), Color.White);

                position = new Vector2((GraphicsDevice.Viewport.Width - _font.MeasureString(_downloading).X) / 2, (GraphicsDevice.Viewport.Height) / 2);
                _spriteBatch.DrawString(_font, _downloading, position, Color.Gray);
                _spriteBatch.DrawString(_font, _downloading, new Vector2(position.X - 2, position.Y - 2), Color.White);

                _spriteBatch.End();
            }
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
            if (_status != Backend.GameStatus.GameOver)
            {
                for (int i = _interfaceElements.Count - 1; i > -1; --i)
                {
                    if (_interfaceElements[i].OnKeyDown(k)) return true;
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

        public bool OnKeyUp(Keys k)
        {
            return true;
        }

        public bool OnMouseDown(int button)
        {

            if (_status != Backend.GameStatus.GameOver)
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

            if (_status != Backend.GameStatus.GameOver)
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
        public virtual void HandleEvent(bool DownStream, Backend.Events eventID, params object[] data)
        {
            if (DownStream)
            {
                // Frontend to backend (send)
                switch (eventID)
                {
                    case Events.FinishedAnimation:
                        _logic.HandleEvent(true, Events.FinishedAnimation, data);
                        break;
                    case Events.MoveActor:
                        _logic.HandleEvent(true, Events.MoveActor, data);
                        break;
                    case Backend.Events.Chat:
                        if (_logic is PureLogic)
                        {
                            _statusbox.AddLine((string)data[0], Color.Pink);
                        }
                        else
                        {
                            (_logic as NetLogic).SendChat((string)data[0]);
                        }
                        break;

                    case Backend.Events.AddDragItem:
                        _draggedObject = (GridElement)data[0];
                        _toolbar.dragItem = _draggedObject;
                        break;

                    case Backend.Events.Settings:
                        if (_focus is Lobby)
                        {
                            _status = Backend.GameStatus.NoRedraw;
                            NetPlayer tmp = ((Lobby)_focus).network;

                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;

                            if (tmp.connected)
                            {
                                if (_logic is PureLogic)
                                {
                                    _logic = new NetLogic(this, tmp);
                                    ((NetLogic)_logic).RequestMap();
                                }
                                else
                                {
                                    tmp.parent = _logic;
                                    HandleEvent(true, Events.ContinueGame, true);
                                }
                            }
                            else
                            {
                                if (_logic is NetLogic)
                                {
                                    _logic = new PureLogic(this);
                                    _logic.HandleEvent(true, Events.Initialize);
                                    HandleEvent(true, Events.ContinueGame, true);

                                }
                                else HandleEvent(true, Events.ContinueGame, true);

                            }
                        }
                        break;
                    case Events.Initialize:
                        if (_logic is PureLogic)
                        {
                            _logic.HandleEvent(true, Events.Initialize);
                            _logic.HandleEvent(true, Events.ContinueGame);
                        }
                        break;
                    case Events.TileEntered:
                        _logic.HandleEvent(true, Events.TileEntered, data);
                        break;
                    case Backend.Events.ShowMenu:
                        _logic.HandleEvent(true, Events.Pause);
                        if (_focus is CharacterWindow)
                        {
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 13);
                            _status = Backend.GameStatus.Running;
                        }
                        if (_status == Backend.GameStatus.Running)
                        {
                            _ShowMenu();
                        }
                        else HandleEvent(true, Backend.Events.ContinueGame, 0);
                        break;
                    case Backend.Events.ShowCharacter:
                        _logic.HandleEvent(true, Events.Pause);
                        if (_status == Backend.GameStatus.Running) _ShowCharacterWindow((Backend.Actor)data[0], 0);
                        else
                        {
                            if (_focus is CharacterWindow)
                            {
                                if (((CharacterWindow)_focus).page != 0)
                                {
                                    ((CharacterWindow)_focus).page = 0;
                                    _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 10);
                                    return;
                                }
                                HandleEvent(true, Backend.Events.ContinueGame, 0);
                            }
                            else
                            {
                                if (_focus is Window)
                                {
                                    _focus.Dispose();
                                    _interfaceElements.Remove(_focus);
                                    _focus = null;
                                    _status = Backend.GameStatus.Running;
                                }
                                _ShowCharacterWindow((Backend.Actor)data[0], 0);
                                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 10);

                            }
                        }
                        break;
                    case Backend.Events.ShowAbilities:
                        _logic.HandleEvent(true, Events.Pause);

                        if (_status == Backend.GameStatus.Running) _ShowCharacterWindow((Backend.Actor)data[0], 2);
                        else
                        {
                            if (_focus is CharacterWindow)
                            {
                                if (((CharacterWindow)_focus).page != 2)
                                {
                                    ((CharacterWindow)_focus).page = 2;
                                    _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 12);
                                    return;
                                }
                                HandleEvent(true, Backend.Events.ContinueGame, 0);
                            }
                            else
                            {
                                if (_focus is Window)
                                {
                                    _focus.Dispose();
                                    _interfaceElements.Remove(_focus);
                                    _focus = null;
                                    _status = Backend.GameStatus.Running;
                                }
                                _ShowCharacterWindow((Backend.Actor)data[0], 2);
                                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 12);

                            }

                        }
                        break;
                    case Backend.Events.ShowInventory:
                        _logic.HandleEvent(true, Events.Pause);

                        if (_status == Backend.GameStatus.Running) _ShowCharacterWindow((Backend.Actor)data[0], 1);
                        else
                        {
                            if (_focus is CharacterWindow)
                            {
                                if (((CharacterWindow)_focus).page != 1)
                                {
                                    ((CharacterWindow)_focus).page = 1;
                                    _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 11);
                                    return;
                                }
                                _focus.Dispose();
                                _interfaceElements.Remove(_focus);
                                _focus = null;
                                _status = Backend.GameStatus.Running;
                                HandleEvent(true, Backend.Events.ContinueGame, 0);
                            }
                            else
                            {
                                if (_focus is Window)
                                {
                                    _focus.Dispose();
                                    _interfaceElements.Remove(_focus);
                                    _focus = null;
                                    _status = Backend.GameStatus.Running;
                                }
                                _ShowCharacterWindow((Backend.Actor)data[0], 1);
                                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 11);

                            }
                        }
                        break;

                    case Backend.Events.FetchFile:
                        _logic.HandleEvent(true, Events.Pause);

                        if (data.Length > 0)
                        {
                            string file = data[0].ToString();
                            _files2fetch.Enqueue(file);
                            if (_status != Backend.GameStatus.FetchingData)
                            {
                                _prevState = _status;
                                _status = Backend.GameStatus.FetchingData;
                                _LoadFile(file, wc_DownloadProgressChanged, wc_DownloadFileCompleted);
                            }
                        }
                        break;

                    case Backend.Events.LoadFromCheckPoint:
                        _status = Backend.GameStatus.NoRedraw;
                        _logic.HandleEvent(true, Events.LoadFromCheckPoint, _playerID);
                        break;



                    case Backend.Events.Network:
                        if (_logic is NetLogic)
                        {
                            _logic.HandleEvent(true, Events.Pause);
                            _ShowLANWindow(((NetLogic)_logic).network);
                        }
                        else
                        {
                            _ShowLANWindow();
                        }
                        /*  //_secondPlayer = true;
                          _lan = true;
                          foreach (UIElement element in _interfaceElements)
                          {
                              element.HandleEvent(true, Backend.Events.ToggleButton, Backend.Events.Player2, true);
                              element.HandleEvent(true, Backend.Events.ToggleButton, Backend.Events.Player1, false);
                              element.HandleEvent(true, Backend.Events.ToggleButton, Backend.Events.Local, false);
                              element.HandleEvent(true, Backend.Events.ToggleButton, Backend.Events.LAN, true);
                          }
                          // _mainmap2.enabled = false;
                          _mainmap1.Resize(new Rectangle(5, 5, _graphics.GraphicsDevice.Viewport.Width - 230, ((_graphics.GraphicsDevice.Viewport.Height - 20)) - 115));*/
                        break;


                    case Backend.Events.ContinueGame:

                        if ((_status != Backend.GameStatus.NoRedraw) || (data.Count() > 0))
                        {
                            _PlayMusic();
                            _logic.HandleEvent(true, Events.ContinueGame);
                            if (_logic is PureLogic)
                            {
                                for (int i = 0; i < _logic.map.actors.Count; ++i)
                                {
                                    if (_logic.map.actors[i] is Player)
                                    {
                                        _playerID = i;
                                        break;
                                    }
                                }
                            }
                            if (_focus is Window)
                            {
                                _focus.Dispose();
                                _interfaceElements.Remove(_focus);
                            }
                            _toolbar.HandleEvent(true, Backend.Events.ContinueGame, -1);
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                            {
                                _mainmap1.noMove = true;
                            }
                            _mainmap1.playerID = _playerID;
                            _minimap1.playerID = _playerID;
                            _logic.map.actors[_playerID].online = true;
                            _mainmap1.resetActors();
                            //_mainmap2.resetActors();
                            _mana.actor = _logic.map.actors[_playerID];
                            _health.actor = _logic.map.actors[_playerID];
                            _toolbar.actor = _logic.map.actors[_playerID];

                            _status = GameStatus.Running;
                            _focus = null;

                        }
                        break;

                    case Backend.Events.About:
                        _logic.HandleEvent(true, Events.Pause);
                        if (_focus is Window)
                        {
                            _focus.Dispose();
                            _interfaceElements.Remove(_focus);
                            _focus = null;
                        }
                        _ShowAbout();
                        break;

                    case Backend.Events.EndGame:
                        _logic.HandleEvent(true, Events.EndGame);
                        Exit();
                        break;

                    case Backend.Events.NewMap:
                        _status = Backend.GameStatus.NoRedraw;
                        _logic.HandleEvent(true, Backend.Events.NewMap);
                        break;

                    case Backend.Events.ResetGame:
                        _status = Backend.GameStatus.NoRedraw;
                        _logic.HandleEvent(true, Events.ResetGame);
                        break;

                    case Backend.Events.ButtonPressed:
                        switch ((Backend.Buttons)data[0])
                        {
                            case Backend.Buttons.Save:
                                _ShowFileDialog(true);
                                break;
                            case Backend.Buttons.Restore:
                                _ShowFileDialog(false);
                                break;
                        }
                        break;

                    case Backend.Events.ActivateAbility:
                        _logic.HandleEvent(true, Events.ActivateAbility, data);
                        break;

                    case Backend.Events.SaveLoad:
                        if (_logic is PureLogic)
                        {
                            PureLogic tmp = _logic as PureLogic;
                            if ((bool)data[0])
                                tmp.Save((string)data[1]);
                            else
                                tmp.Restore((string)data[1]);
                        }
                        HandleEvent(true, Backend.Events.ContinueGame);
                        break;

                    case Backend.Events.ChangeMap:
                        _status = Backend.GameStatus.NoRedraw;
                        _playerID = (int)data[0];
                        _logic.map.actors[_playerID].online = true;
                        _mainmap1.playerID = _playerID;
                        _minimap1.playerID = _playerID;
                        _mainmap1.map = _logic.map;
                        _minimap1.map = _logic.map;
                        _health.actor = _logic.map.actors[_playerID];
                        _mana.actor = _logic.map.actors[_playerID];
                        _toolbar.actor = _logic.map.actors[_playerID];
                        _mainmap1.resetActors();
                        if (_logic.map.actors[_playerID].tile != null)
                            _minimap1.MoveCamera(_logic.map.actors[_playerID].tile.coords);
                        HandleEvent(false, Events.ShowMessage, "You entered " + _logic.map.name + ".");
                        _PlayMusic();
                        _status = Backend.GameStatus.Paused;
                        _logic.HandleEvent(false, Events.ContinueGame);
                        break;
                }
            }
            else
            {
                // Backend to Frontend (received)
                switch (eventID)
                {
                    case Backend.Events.AddPlayer:
                        _mainmap1.resetActors();
                        break;
                    case Backend.Events.RemovePlayer:
                        _mainmap1.resetActors();
                        break;
                    case Backend.Events.Disconnect:
                        HandleEvent(true, Events.Network);
                        break;
                    case Backend.Events.Attack:
                        _mainmap1.HandleEvent(true, Events.AnimateActor, data[0], Backend.Activity.Attack);
                        break;
                    case Backend.Events.ActorText:
                        if (data.Length > 3)
                            _mainmap1.floatNumber((Coords)data[1], (string)data[2], (Color)data[3]);
                        else
                            _mainmap1.floatNumber((Coords)data[1], (string)data[2], Color.White);

                        // defender, _map.actors[defender].tile.coords, "Evade")
                        break;
                    case Backend.Events.DamageActor:
                        // , defender, _map.actors[defender].tile.coords, _map.actors[defender].health, damage);
                        _mainmap1.HandleEvent(true, Events.AnimateActor, data[0], Backend.Activity.Hit);
                        if ((int)data[0] == _playerID)
                        {
                            _PlaySoundEffect(SoundFX.Damage);
                        }
                        _mainmap1.floatNumber((Coords)data[1], ((int)data[3]).ToString(), ((int)data[0] == _playerID) ? Color.Red : Color.White);
                        break;
                    case Backend.Events.KillActor:
                        if ((int)data[0] < _logic.map.actors.Count)
                        {
                            _mainmap1.HandleEvent(true, Events.AnimateActor, data[0], Backend.Activity.Die);
                            if ((int)data[0] == _playerID)
                            {
                                _PlaySoundEffect(SoundFX.Damage);
                            }
                            _mainmap1.floatNumber((Coords)data[1], ((int)data[3]).ToString(), ((int)data[0] == _playerID) ? Color.Red : Color.White);
                            HandleEvent(false, Events.ShowMessage, _logic.map.actors[(int)data[0]].name + " was killed.");
                        }
                        break;
                    case Backend.Events.ChangeStats:
                        break;
                    case Backend.Events.FireProjectile:
                        break;
                    case Backend.Events.PlaySound:
                        _PlaySoundEffect((Backend.SoundFX)data[0]);
                        break;
                    case Backend.Events.ActivateAbility:
                        /*   if ((int)data[0] == _playerID)
                           {
                               if ((int)data[1] < 0)
                               {
                                   int item = (int)data[1] + 1;

                               }
                               else
                               {
                                   if ((int)data[1] > 0)
                                   {
                                       int ability = (int)data[1] - 1;
                                   }
                               }
                           }*/
                        break;
                    case Backend.Events.Dialog:
                        //from, to, message, new Backend.DialogLine[] { new Backend.DialogLine("Goodbye", -1) }
                        _ShowTextBox((string)data[2]);
                        HandleEvent(false, Events.ShowMessage, ((Actor)data[1]).name + "->" + ((Actor)data[0]).name + ":" + (string)data[2]);
                        break;
                    case Backend.Events.Shop:
                        _ShowShopWindow((Actor)data[0], (Actor)data[1]);
                        HandleEvent(false, Events.ShowMessage, ((Actor)data[0]).name + " and " + ((Actor)data[1]).name + " traded items.");
                        break;
                    case Events.SetItemTiles:
                        break;
                    case Events.Checkpoint:
                        break;
                    case Events.GameOver:
                        _ShowEndGame("You escaped from the Dungeon!", "Game won");
                        break;
                    case Backend.Events.FinishedAnimation:

                        break;
                    case Backend.Events.AnimateActor:
                        _mainmap1.HandleEvent(true, Events.AnimateActor, data);
                        break;
                    case Backend.Events.ShowMessage:
                        _AddMessage(data[0].ToString(), data.Length > 1 ? data[1] : null);
                        break;

                    case Events.MoveActor:
                        _mainmap1.HandleEvent(true, Events.MoveActor, data);
                        break;

                    case Backend.Events.ChangeMap:
                        _logic.ChangeMap((string)data[0], (Coords)data[1]);
                        break;
                }
            }
        }
        #endregion

        #region Private Helper Methods




        /// <summary>
        /// Restart background music
        /// </summary>
        private void _PlayMusic()
        {
            MediaPlayer.IsRepeating = false;
            Song _tmp = Content.Load<Song>(_logic.map.music); // Todo: *.mp3

            if (_tmp != _backMusic)
            {
                MediaPlayer.Stop();
                _backMusic = _tmp;
                MediaPlayer.Play(_backMusic);
                MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            }
            else MediaPlayer.Resume();
            MediaPlayer.IsRepeating = false;

            MediaPlayer.Volume = (float)0.3;
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if ((MediaPlayer.State == MediaState.Paused) || ((MediaPlayer.State == MediaState.Stopped)))
            {
                MediaPlayer.Play(_backMusic);
            }
        }


        /// <summary>
        /// Play a sound effect
        /// </summary>
        /// <param name="index"></param>
        private void _PlaySoundEffect(Backend.SoundFX index)
        {
            SoundEffect tmp = null;
            switch (index)
            {
                case SoundFX.ChangeMap:
                    tmp = Content.Load<SoundEffect>("changemap1.wav");
                    break;
                case SoundFX.Checkpoint:
                    tmp = Content.Load<SoundEffect>("checkpoint1.wav");

                    break;
                case SoundFX.Damage:
                    tmp = Content.Load<SoundEffect>("trapdamage1.wav");

                    break;
                case SoundFX.Pickup:
                    tmp = Content.Load<SoundEffect>("pickup1.wav");
                    break;
                case SoundFX.Trap:
                    tmp = Content.Load<SoundEffect>("trap1.wav");
                    break;

            }
            if (tmp != null)
            {
                SoundEffectInstance effect = tmp.CreateInstance();
                effect.Play();
            }
        }


        /// <summary>
        /// Display a dialog with Ok button
        /// </summary>
        /// <param name="message">Message to display</param>
        private void _ShowTextBox(string message)
        {
            _status = Backend.GameStatus.Paused;
            Client.Window _messagebox = new Client.Window(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 100, 600, 200));
            Client.Statusbox stat = new Client.Statusbox(_messagebox, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 300 + 10, (int)(GraphicsDevice.Viewport.Height / 2.0f) - 70, 590, 110), false, true);
            stat.AddLine(message);
            _messagebox.AddChild(stat);
            _messagebox.AddChild(new Client.Button(_messagebox, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width) / 2.0f) - 65, (int)(GraphicsDevice.Viewport.Height / 2.0f) + 30, 130, 40), "Goodbye!", (int)Backend.Buttons.Close));
            _interfaceElements.Add(_messagebox);
            _messagebox.ChangeFocus();
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }




        /// <summary>
        /// Add text to status box
        /// </summary>
        /// <param name="s"></param>
        private void _AddMessage(string s, object color = null)
        {
            _statusbox.AddLine(s, color);
        }

        /// <summary>
        /// Indicate damage  done to player
        /// </summary>
        private void _RemoveHealth()
        {
            _backgroundcolor.G = 0;
            _backgroundcolor.R = 200;
            // Play sound
        }

        /// <summary>
        /// Indicate health regained by potions
        /// </summary>
        private void _AddHealth()
        {
            _backgroundcolor.R = 0;
            _backgroundcolor.G = 200;
            // Play sound
        }
        #endregion


        #region Default subwindows



        private void _screenshot()
        {
            RenderTarget2D renderTarget = new RenderTarget2D(
_graphics.GraphicsDevice,
_graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
_graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
            GameStatus temp = _status;
            _status = GameStatus.NoRedraw;
            _graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            _mainmap1.Draw(new GameTime());
            _graphics.GraphicsDevice.SetRenderTarget(null);
            _status = temp;

            byte[] pixelData = new byte[renderTarget.Width * renderTarget.Height * 4];

            renderTarget.GetData(pixelData);
            for (int i = 0; i < pixelData.Length - 3; i += 4)
            {
                byte tmp = pixelData[i];
                pixelData[i] = pixelData[i + 2];
                pixelData[i + 2] = tmp;
            }
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(renderTarget.Width, renderTarget.Height, renderTarget.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(pixelData, 0));
            int _width = renderTarget.Width - 5;
            int _height = renderTarget.Height - 145;
            int _left = 5;
            int _top = 5;
            if (_width > _height)
            {
                _left += (_width - _height) / 2;
                _width = _height;
            }
            else
            {
                _top += (_height - _left) / 2;
                _height = _width;
            }
            System.Drawing.Bitmap cropped = bitmap.Clone(new System.Drawing.Rectangle(_left, _top, _width, _height), bitmap.PixelFormat);

            bitmap.Dispose();
            bitmap = new System.Drawing.Bitmap(cropped, new System.Drawing.Size(200, 200));

            //Unlock the pixels
            //bmp.UnlockBits(bmpData);
            bitmap.Save("save\\screen.png", System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
            cropped.Dispose();


        }

        public static Rectangle center(Rectangle ViewPort, int width = 200, int height = 400, int top = -1)
        {
            return new Rectangle(ViewPort.Left + (ViewPort.Width - width) / 2, ViewPort.Top + ((top != -1) ? top : ((ViewPort.Height - height) / 2)), width, height);
        }


        public static Rectangle inner(Rectangle outer, int padding = 10)
        {
            return new Rectangle(outer.Left + 10, outer.Top + 10, outer.Width - 20, outer.Height - 20);
        }


        /// <summary>
        /// Display Main Menu
        /// </summary>
        private void _ShowMenu()
        {
            _status = Backend.GameStatus.Paused;
            Rectangle rect = center(GraphicsDevice.Viewport.Bounds, 300, 300);
            Window _mainMenu = new Window(this, _spriteBatch, Content, rect);
            rect = inner(rect);
            if (!(_logic is NetLogic))
            {
                _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 32, 0), "Continue", (int)Backend.Buttons.Close));
                _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 32, 50), "Restart", (int)Backend.Buttons.Restart));

                _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 32, 90), "New Maps", (int)Backend.Buttons.NewMap));

                _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle(rect.Left, rect.Top + 130, 135, 32), "Save", (int)Backend.Buttons.Save));
                bool hasSave = false;
                foreach (string dir in Directory.GetDirectories(".\\save"))
                {
                    string tmp = dir.ToLower().Trim();
                    if (tmp.Substring(tmp.LastIndexOf('\\') + 1) != "auto")
                    {
                        hasSave = true;
                        break;
                    }
                }
                if (hasSave)
                    _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle(rect.Right - 135, rect.Top + 130, 135, 32), "Restore", (int)Backend.Buttons.Restore));
                /*
                            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 30, 140, 60), "Local", (int)Buttons.Local, !_lan));
                            */
            }
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 146, 170), "Multiplayer " + ((_logic is NetLogic) ? "(online)" : "(offline)"), (int)Backend.Buttons.LAN, _lan));
            /*
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 40, 300, 60), "Settings", (int)Buttons.Settings));
*/

            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 186, 210), "Credits", (int)Backend.Buttons.Credits));
            _mainMenu.AddChild(new Button(_mainMenu, _spriteBatch, Content, center(rect, 280, 226, 250), "Quit", (int)Backend.Buttons.Quit));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));



            _interfaceElements.Add(_mainMenu);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        private void _ShowCharacterWindow(Backend.Actor actor, uint page = 0)
        {
            if (!(_focus is CharacterWindow))
            {
                _status = Backend.GameStatus.Paused;
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

        private void _ShowLANWindow(NetPlayer network = null)
        {
            if (_focus is Window)
            {
                _focus.Dispose();
                _interfaceElements.Remove(_focus);
                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 13);
                _status = Backend.GameStatus.Running;
            }
            _status = Backend.GameStatus.Paused;
            Lobby _lobby = new Lobby(this, _spriteBatch, Content, new Rectangle((GraphicsDevice.Viewport.Width - 340) / 2, (GraphicsDevice.Viewport.Height - 200) / 2, 680, 400), network);
            _interfaceElements.Add(_lobby);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        private void _ShowShopWindow(Backend.Actor actor1, Backend.Actor actor2)
        {
            if (!(_focus is CharacterWindow))
            {
                _status = Backend.GameStatus.Paused;
                Shop c = new Shop(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 250) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) - 200, 500, 300), actor1, actor2);
                _interfaceElements.Add(c);
                _focus = _interfaceElements[_interfaceElements.Count - 1];
            }
        }

        /// <summary>
        /// A text displayed if the player died
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        private void _ShowEndGame(string message = "You have failed in your mission. Better luck next time.", string title = "Game over!")
        {
            if (_focus is Window)
            {
                _focus.Dispose();
                _interfaceElements.Remove(_focus);
                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 13);
                _status = Backend.GameStatus.Running;
            }
            _status = Backend.GameStatus.GameOver;
            //            _logic.map.Save("savedroom" + _logic.map.id + ".xml");
            Rectangle rect = center(GraphicsDevice.Viewport.Bounds, 500, 150);
            Window _gameOver = new Window(this, _spriteBatch, Content, rect);
            Statusbox stat;
            rect = inner(rect);
            if (_logic is PureLogic)
            {
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Left, rect.Bottom - 40, 80, 32), "New Maps", (int)Backend.Buttons.NewMap));

                if (_logic.map.actors[_playerID].lives > 0)
                {
                    _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Left + 90, rect.Bottom - 40, 150, 32), "Respawn (" + _logic.map.actors[_playerID].lives.ToString() + " left)", (int)Backend.Buttons.Load));
                }
                bool hasSave = false;
                foreach (string dir in Directory.GetDirectories(".\\save"))
                {
                    string tmp = dir.ToLower().Trim();
                    if (tmp.Substring(tmp.LastIndexOf('\\') + 1) != "auto")
                    {
                        hasSave = true;
                        break;
                    }
                }
                if (hasSave)
                    _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Right - 215, rect.Bottom - 40, 65, 32), "Restore", (int)Backend.Buttons.Restore));
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Right - 140, rect.Bottom - 40, 65, 32), "Restart", (int)Backend.Buttons.Restart));
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Right - 65, rect.Bottom - 40, 65, 32), "Quit", (int)Backend.Buttons.Quit));
            }
            else
            {
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Left, rect.Bottom - 40, 80, 32), "Resurrect", (int)Backend.Buttons.Load));
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Right - 170, rect.Bottom - 40, 80, 32), "Disconnect", (int)Backend.Buttons.Connect));
                _gameOver.AddChild(new Button(_gameOver, _spriteBatch, Content, new Rectangle(rect.Right - 80, rect.Bottom - 40, 80, 32), "Quit", (int)Backend.Buttons.Quit));
            }
            _gameOver.AddChild(stat = new Statusbox(_gameOver, _spriteBatch, Content, center(rect, 450, 200, 0), false, true));
            stat.AddLine(title + "\n \n" + message);

            _interfaceElements.Add(_gameOver);
            _focus = _interfaceElements[_interfaceElements.Count - 1];
        }

        /// <summary>
        /// Setup and display a window containing credits
        /// </summary>
        private void _ShowAbout()
        {
            if (_focus is Window)
            {
                _focus.Dispose();
                _interfaceElements.Remove(_focus);
                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 13);
                _status = Backend.GameStatus.Running;
            }
            _status = Backend.GameStatus.Paused;
            Rectangle rect = center(GraphicsDevice.Viewport.Bounds, 600, 300);
            Window _about = new Window(this, _spriteBatch, Content, rect);
            rect = inner(rect);
            _about.AddChild(new Button(_about, _spriteBatch, Content, new Rectangle(rect.Left + 45, rect.Bottom - 42, rect.Width - 90, 32), "Ok", (int)Backend.Buttons.Close));

            //  _mainMenu.AddChild(new ProgressBar(this, _spriteBatch, Content, new Rectangle((int)((GraphicsDevice.Viewport.Width - 160) / 2.0f), (int)(GraphicsDevice.Viewport.Height / 2.0f) + 80, 300, 30), ProgressStyle.Block,100,2));
            Statusbox stat;
            _about.AddChild(stat = new Statusbox(_about, _spriteBatch, Content, new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - 50), false, true));
            stat.AddLine("Dungeon Crawler 2013\n\n Developed by Group 22\n\n" +
                "*********************************\n\n" +
                "Music: Video Dungeon Crawl by Kevin MacLeod is licensed under a CC Attribution 3.0\n\n" +
                "http://incompetech.com/music/royalty-free/index.html?collection=029\n\n" +
                "Graphics: Tile Graphics by Reiner Prokein\n\n" +
                "http://www.reinerstilesets.de/de/lizenz/ ");
            _interfaceElements.Add(_about);
            _focus = _interfaceElements[_interfaceElements.Count - 1];

        }


        /// <summary>
        /// Setup and display a window used for saving / loading files
        /// </summary>
        private void _ShowFileDialog(bool save)
        {
            if (_focus is Window)
            {
                _focus.Dispose();
                _interfaceElements.Remove(_focus);
                _toolbar.HandleEvent(true, Backend.Events.ContinueGame, 13);
                _status = Backend.GameStatus.Paused;
            }
            _screenshot();
            FileDialog _fileDialog = new FileDialog(this, _spriteBatch, Content, center(GraphicsDevice.Viewport.Bounds, 600, 390), save);
            _interfaceElements.Add(_fileDialog);
            _focus = _interfaceElements[_interfaceElements.Count - 1];

        }
        #endregion

        #region Download
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
                _downloading = _files2fetch.Count.ToString() + ": " + _filename + "()";
                await wc.DownloadFileTaskAsync("http://casim.hhu.de/Crawler/" + _filename, ".\\Content\\" + _filename);
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
            _downloading = _downloading.Substring(0, _downloading.LastIndexOf("(")) + "(" + e.BytesReceived.ToString("n0") + "/" + e.TotalBytesToReceive.ToString("n0") + "=" + e.ProgressPercentage.ToString() + "%)";
            //  throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when a download is complete
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        public async void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_files2fetch.Count == 0)
            {
                _status = _prevState;
                if (_status == Backend.GameStatus.Loading)
                {
                    _status = Backend.GameStatus.FetchingData;
                    await CreateXMLFiles.CreateXML(_mainmap1, new Camera(Vector2.Zero), Content);
                    _downloading = "Creating XML files...";
                    SetupGame();
                }
            }
            else
            {
                string file = "";
                do
                { file = _files2fetch.Dequeue(); }
                while ((System.IO.File.Exists(".\\Content\\" + file)) && (_files2fetch.Count > 0));
                if ((file != "") && (!System.IO.File.Exists(".\\Content\\" + file)))
                {
                    _LoadFile(file, wc_DownloadProgressChanged, wc_DownloadFileCompleted);
                }

            }
        }
        #endregion

        #region Constructor
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
        #endregion

    }
}
