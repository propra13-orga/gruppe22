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
        /// Output device
        /// </summary>
        GraphicsDeviceManager _graphics = null;
        /// <summary>
        /// Main Sprite drawing algorithm
        /// </summary>
        SpriteBatch _spriteBatch = null;
        /// <summary>
        /// Font to display information
        /// </summary>
        SpriteFont _font = null;

        /// <summary>
        /// Images to use on the minimap
        /// </summary>
        Texture2D _miniIcons = null;
        int counter = 0;
        Texture2D _floor = null;
        int _mouseWheel = 0;

        Texture2D _wall1 = null;
        Texture2D _wall2 = null;
        UIElement _currentObject = null;

        Texture2D _player1 = null;
        Texture2D _player2 = null;

        Effect _desaturateEffect = null;
        /// <summary>
        /// Textarea to display status information
        /// </summary>
        Statusbox _statusBox = null;


        /// <summary>
        /// Minimap for Player 1
        /// </summary>
        Minimap _miniMap1 = null;
        /// <summary>
        /// Main map for Player 1
        /// </summary>
        Mainmap _mainMap1 = null;
        /// <summary>
        /// Internal storage for Player 1
        /// </summary>
        Map _map1 = null;

        /// <summary>
        /// Background Music
        /// </summary>
        Song _backMusic;

        Vector2 _mousepos;
        #endregion

        #region Protected Methods (overrides)
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backMusic = Content.Load<Song>("Video Dungeon Crawl.wav"); // Todo: *.mp3
            _font = Content.Load<SpriteFont>("Font");
            MediaPlayer.Volume = (float)0.3;
            MediaPlayer.Play(_backMusic);

            _wall1 = Content.Load<Texture2D>("wall1");
            _wall2 = Content.Load<Texture2D>("wall2");
            _floor = Content.Load<Texture2D>("floor");
            _player1 = Content.Load<Texture2D>("player1");
            _player2 = Content.Load<Texture2D>("player2");
            _player2 = Content.Load<Texture2D>("player2");
            _miniIcons = Content.Load<Texture2D>("Minimap");
            //_desaturateEffect = Content.Load<Effect>("normalmap");
            _map1 = new Map(15, 8);
            string str4 =
                "######.####.###\n" +
                "###.##.####.###\n" +
                "###.........###\n" +
                "###.###.###.###\n" +
                "###.###.###.###\n" +
                "#.............#\n" +
                "#....##.#######\n" +
                "#.#####.#######\n";
            string str =
              "###############\n" +
              "#....#........#\n" +
              "#....#........#\n" +
              "#..############\n" +
              "#....#..#.....#\n" +
              "#.####..#######\n" +
              "#....#..#.....#\n" +
              "######..#######\n";
            string str1 =
              "###############\n" +
              "#.....#.......#\n" +
              "#.....#.......#\n" +
              "###.........###\n" +
              "#.............#\n" +
              "#.....#.......#\n" +
              "#.....#.......#\n" +
              "###############\n";


            // _drawWall(Direction.LeftClose, 4, 3, false);

            _map1.FromString(str);

            _miniMap1 = new Minimap(_graphics, _spriteBatch, new Rectangle(_graphics.GraphicsDevice.Viewport.Width - 210, 20, 200, 200), _miniIcons, _map1);
            _mainMap1 = new Mainmap(_graphics, _spriteBatch, new Rectangle(0, 20, _graphics.GraphicsDevice.Viewport.Width - 220, _graphics.GraphicsDevice.Viewport.Height - 140), _floor, _wall1, _wall2, _desaturateEffect, _map1, _player1, _player2);
            _statusBox = new Statusbox(_graphics, _spriteBatch, new Rectangle(40, _graphics.GraphicsDevice.Viewport.Height - 120, _graphics.GraphicsDevice.Viewport.Width - 20, 100), _font);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            _backMusic.Dispose();
            _wall1.Dispose();
            _wall2.Dispose();

            _floor.Dispose();
            _player1.Dispose();
            _player2.Dispose();

            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _currentObject = _mainMap1;
            /*

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (_miniMap1.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    _currentObject = _miniMap1;
                }
                else
                    if (_mainMap1.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                    {
                        _currentObject = _mainMap1;
                    }
                    else
                        if (_statusBox.IsHit(Mouse.GetState().X, Mouse.GetState().Y))
                        {
                            _currentObject = _statusBox;
                        }
                        else
                        { _currentObject = null; }
            }*/



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                _graphics.ToggleFullScreen();

            if (_currentObject != null)
            {
                if (Mouse.GetState().ScrollWheelValue != _mouseWheel)
                {
                    _mainMap1.ScrollWheel(_mouseWheel - Mouse.GetState().ScrollWheelValue);
                    _mouseWheel = Mouse.GetState().ScrollWheelValue;
                }


                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (_mousepos.X != -1)
                    {
                        _mainMap1.MoveContent(new Vector2(Mouse.GetState().X - _mousepos.X, Mouse.GetState().Y - _mousepos.Y));
                    }
                    _mousepos.X = Mouse.GetState().X;
                    _mousepos.Y = Mouse.GetState().Y;
                }
                else
                {
                    _mousepos.X = -1;
                    _mousepos.Y = -1;
                }
                _mainMap1.HandleKey();
            }
            counter += 1;
            // TODO: Add your update logic here
            if (counter % 10 == 1)
                _mainMap1.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (_mainMap1 != null) _mainMap1.Draw();

            if (_miniMap1 != null) _miniMap1.Draw(gameTime);
            if (_statusBox != null) _statusBox.Draw(gameTime);

            base.Draw(gameTime);
        }
        #endregion

        public MainWindow()
            : base()
        {
            Content.RootDirectory = "Content";
            Window.Title = "Dungeon Crawler 2013";

            _graphics = new GraphicsDeviceManager(this);
<<<<<<< HEAD
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.IsFullScreen = true;
=======
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 400;
            _graphics.IsFullScreen = false;
>>>>>>> 9f9f0678d300463cd1d13c7b022a238f2e864248
            _graphics.ApplyChanges();
            //_graphics.IsFullScreen = true;

        }

    }
}
