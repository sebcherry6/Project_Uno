using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_Uno2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Camera _camera;
        public Player _player;

        public static ContentManager _publicContent;

        public static List<Tile> _tiles = new List<Tile>();

        public Texture2D _playerTxr;

        public static KeyboardState _keyState;
        public static KeyboardState _oldkeyState;

        public static MouseState _mouseState;
        public static MouseState _oldMouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _publicContent = Content;

            _player = new Player();

            _graphics.PreferredBackBufferWidth = 768;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();
            _camera = new Camera(_graphics.GraphicsDevice.Viewport);

            _playerTxr = Content.Load<Texture2D>("Player");

            LoadLevel(Content.Load<Texture2D>("Rooms/room1"));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _keyState = Keyboard.GetState();
            _camera._position = _player._position * 4;
            _camera.Update();
            _player.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _camera.ApplyProjectionMatrix(_graphics);
            
            _spriteBatch.Begin(transformMatrix: _camera._transform);

            _spriteBatch.Draw(Content.Load<Texture2D>("image"), Vector2.Zero, Color.White);
            _player.Draw(_spriteBatch, _playerTxr);

            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].Draw(_spriteBatch, Content);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void LoadLevel(Texture2D room)
        {
            List<Color> pixels = new List<Color>();
            Color[] textureData = new Color[room.Width * room.Height];

            room.GetData(textureData);

            for (int i = 0; i < textureData.Length; i++)
            {
                pixels.Add(textureData[i]);
            }

            for (int y = 1; y < room.Height; y++)
            {
                for (int x = 1; x < room.Width; x++)
                {
                    if (pixels[y * room.Width + x].A == 255 && pixels[y * room.Width + x].R == 0 && pixels[y * room.Width + x].G == 0 && pixels[y * room.Width + x].B == 0)
                    {
                        _tiles.Add(new Tile(new Vector2(x * 32, y * 32)));
                    }
                }
            }
        }
    }
}
