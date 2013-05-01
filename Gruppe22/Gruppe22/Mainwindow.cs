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
        GraphicsDeviceManager _graphics;
        /// <summary>
        /// Main Sprite drawing algorithm
        /// </summary>
        SpriteBatch _spriteBatch;
        /// <summary>
        /// Font to display information
        /// </summary>
        SpriteFont _font;

        /// <summary>
        /// Images to use on the minimap
        /// </summary>
        Texture2D _miniIcons;

        Texture2D _floor;

        Texture2D _wall1;
        Texture2D _wall2;
        Texture2D _wall3;


        Texture2D _player1;
        Texture2D _player2;
        /// <summary>
        /// Textarea to display status information
        /// </summary>
        Statusbox _statusBox;


        /// <summary>
        /// Minimap for Player 1
        /// </summary>
        Minimap _miniMap1;
        /// <summary>
        /// Main map for Player 1
        /// </summary>
        Mainmap _mainMap1;
        /// <summary>
        /// Internal storage for Player 1
        /// </summary>
        Map _map1;

        /// <summary>
        /// Background Music
        /// </summary>
        Song _backMusic;
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
            _wall3 = Content.Load<Texture2D>("wall3");
            _floor = Content.Load<Texture2D>("floor");
            _player1 = Content.Load<Texture2D>("player1");
            _player2 = Content.Load<Texture2D>("player2");
            _player2 = Content.Load<Texture2D>("player2");
            _miniIcons = Content.Load<Texture2D>("Minimap");
            _map1 = new Map(10, 10);

            _miniMap1 = new Minimap(_graphics, _spriteBatch, new Rectangle(_graphics.PreferredBackBufferWidth - 210, 5, 200, 100), _miniIcons, _map1);
            _mainMap1 = new Mainmap(_graphics, _spriteBatch, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth - 220, _graphics.PreferredBackBufferHeight - 140), _floor, _wall1, _wall2, _wall3, _map1);
            _statusBox = new Statusbox(_graphics, _spriteBatch, new Rectangle(40, _graphics.PreferredBackBufferHeight - 120, _graphics.PreferredBackBufferWidth - 20, 100), _font);

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
            _wall3.Dispose();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                _graphics.ToggleFullScreen();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _statusBox.Draw(gameTime);
            _miniMap1.Draw(gameTime);
            _mainMap1.Draw();
            base.Draw(gameTime);
        }
        #endregion

        public MainWindow()
            : base()
        {
            Content.RootDirectory = "Content";
            Window.Title = "Dungeon Crawler 2013";

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferWidth = 1024;
            //_graphics.IsFullScreen = true;

        }

    }
}
