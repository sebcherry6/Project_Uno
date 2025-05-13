using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Project_Uno2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Camera _camera;
        public Player _player;

        public string _screen = "menu";
        private string _currentLoadedRoom = "";

        public static ContentManager _publicContent;

        public static List<Tile> _tiles = new List<Tile>();
        public List<Enemy> _enemies = new List<Enemy>();
        public List<Card> _cards = new();
        public List<Door> _doors = new();
        public List<Acid> _acids = new();

        SimpleAnimator _playButtonAnimator, _exitButtonAnimator;
        bool _wasHovering = false;

        public Texture2D _playerTxr, _playerTxrDefault, _playerTxrEvolved1,
            _playerTxrEvolved2, _playButtonTexture, _bgTexture,
            _enemyTextureRed, _enemyTextureBlue, _enemyTextureGreen,
            _cardTexture, _doorTexture, _acidTexture, _heartTexture,
            _playButtonTxr, _exitButtonTxr;

        public Rectangle _playButtonRect, _cursorRect, _exitButtonRect;

        public KeyboardState _keyState;
        public KeyboardState _oldKeyState;

        public MouseState _mouseState;
        public MouseState _oldMouseState;

        public Vector2 _playerSpawnPos;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _publicContent = Content;

            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            _camera = new Camera(_graphics.GraphicsDevice.Viewport);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerTxrDefault = Content.Load<Texture2D>("Player");
            _enemyTextureRed = Content.Load<Texture2D>("RedEnemy");
            _enemyTextureBlue = Content.Load<Texture2D>("BlueEnemy");
            _enemyTextureGreen = Content.Load<Texture2D>("GreenEnemy");
            _cardTexture = Content.Load<Texture2D>("CardTxr");
            _doorTexture = Content.Load<Texture2D>("DoorTxr");
            _acidTexture = Content.Load<Texture2D>("Acid");
            _heartTexture = Content.Load<Texture2D>("Heart");
            _bgTexture = Content.Load<Texture2D>("Background");
            _playButtonTxr = Content.Load<Texture2D>("playbutton");
            _exitButtonTxr = Content.Load<Texture2D>("exitbutton");

            _exitButtonAnimator = new SimpleAnimator(_exitButtonTxr);
            _exitButtonRect = new Rectangle(372, 580, 256, 128);
            _exitButtonAnimator.AddAnimation("Idle", new List<Rectangle> { new Rectangle(0, 0, 256, 128) }, 0.2, true);
            _exitButtonAnimator.AddAnimation("Hover", new List<Rectangle> {
                                                      new Rectangle(256, 0, 256, 128),
                                                      new Rectangle(512, 0, 256, 128),
                                                      new Rectangle(768, 0, 256, 128),
                                                      new Rectangle(1024, 0, 256, 128),
                                                      new Rectangle(1280, 0, 256, 128),
                                                      new Rectangle(1536, 0, 256, 128)
                                                                         }, 0.2, true);

            _playButtonAnimator = new SimpleAnimator(_playButtonTxr);
            _playButtonRect = new Rectangle(372, 436, 256, 128);
            _playButtonAnimator.AddAnimation("Idle", new List<Rectangle> { new Rectangle(0, 0, 256, 128) }, 0.2, true);
            _playButtonAnimator.AddAnimation("Hover", new List<Rectangle> {
                                                       new Rectangle(256, 0, 256, 128),
                                                       new Rectangle(512, 0, 256, 128),
                                                       new Rectangle(768, 0, 256, 128),
                                                       new Rectangle(1024, 0, 256, 128),
                                                       new Rectangle(1280, 0, 256, 128),
                                                       new Rectangle(1536, 0, 256, 128)
                                                                        }, 0.2, true);

            _player = new Player(this, _playerTxrDefault, Vector2.Zero);
            LoadLevel(Content.Load<Texture2D>("Rooms/Room1"));
            _player._position = _playerSpawnPos;
        }

        protected override void Update(GameTime gameTime)
        {
            _oldKeyState = _keyState;
            _keyState = Keyboard.GetState();
            _mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyState.IsKeyDown(Keys.Escape))
                Exit();

            // --- Menu screen logic ---
            if (_screen == "menu")
            {
                _cursorRect = new Rectangle(_mouseState.X, _mouseState.Y, 1, 1);
                ButtonsEvents();
                _playButtonAnimator.Update(gameTime);
                _exitButtonAnimator.Update(gameTime);
            }

            // --- Game screen logic ---
            if (_screen == "game")
            {
                UpdateRoom(gameTime);
            }
            else if (_screen.StartsWith("room"))
            {
                if (_currentLoadedRoom != _screen)
                {
                    string roomName = _screen;
                    LoadLevel(Content.Load<Texture2D>($"Rooms/{roomName}"));
                    _playerTxr = _playerTxrDefault;
                    _player._position = _playerSpawnPos;
                    _currentLoadedRoom = _screen; // prevent re-loading every frame
                }

                UpdateRoom(gameTime);
            }
;

            _oldKeyState = _keyState;
            _oldMouseState = _mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            
            _spriteBatch.Begin(transformMatrix: _camera._transform);

            if (_screen == "menu")
            {
                _playButtonAnimator.Draw(_spriteBatch, new Vector2(_playButtonRect.X, _playButtonRect.Y), SpriteEffects.None, Color.White);
                _exitButtonAnimator.Draw(_spriteBatch, new Vector2(_exitButtonRect.X, _exitButtonRect.Y), SpriteEffects.None, Color.White);
            }

            if (_screen == "game" || _screen.StartsWith("room"))
            {
                DrawRoom();
            }

            if (_screen == "gameOver")
            {

            }

            _spriteBatch.End();

            // --- HUD drawing ---
            _spriteBatch.Begin(); // No camera transform 

            if (_screen == "game" || _screen.StartsWith("room"))
            {
                DrawHearts(gameTime);
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadLevel(Texture2D room)
        {
            _enemies.Clear();
            _cards.Clear();
            _doors.Clear();
            _acids.Clear();
            _tiles.Clear();

            List<Color> pixels = new List<Color>();
            Color[] textureData = new Color[room.Width * room.Height];

            room.GetData(textureData);

            for (int i = 0; i < textureData.Length; i++)
            {
                pixels.Add(textureData[i]);
            }

            for (int y = 0; y < room.Height; y++)
            {
                for (int x = 0; x < room.Width; x++)
                {
                    Color pixelColor = pixels[y * room.Width + x];

                    // Tiles
                    if (pixelColor == Color.Black)
                    {
                        _tiles.Add(new Tile(new Vector2(x * 32, y * 32)));
                    }
                    else if (pixelColor == Color.Lime)
                    {
                        _acids.Add(new Acid(_acidTexture, new Vector2(x * 32, y * 32)));
                    }
                    else if (pixelColor == Color.DarkSalmon)
                    { 
                    _playerSpawnPos = new Vector2(x * 32, y * 32);
                    }

                    // Enemies
                    else if (pixelColor == Color.Green)
                    {
                        var slimeAnimations = new Dictionary<string, List<Rectangle>>()
                        {
                            { "Walk", new List<Rectangle>
                            { new Rectangle(0, 0, 64, 64), new Rectangle(64, 0, 64, 64), new Rectangle(128, 0, 64, 64), new Rectangle(192, 0, 64, 64),
                            new Rectangle(256, 0, 64, 64), new Rectangle(320, 0, 64, 64), new Rectangle(384, 0, 64, 64), new Rectangle(448, 0, 64, 64),
                            new Rectangle(512, 0, 64, 64), new Rectangle(576, 0, 64, 64), new Rectangle(640, 0, 64, 64), new Rectangle(704, 0, 64, 64),
                            new Rectangle(768, 0, 64, 64), new Rectangle(832, 0, 64, 64), new Rectangle(896, 0, 64, 64)} }
                        };
                        Enemy slime = new Enemy(_enemyTextureGreen, new Vector2(x * 32, y * 32), 3, 40, _player, slimeAnimations, 0.1);
                        _enemies.Add(slime);
                    }
                    else if (pixelColor == Color.Red)
                    {
                        var wormAnimations = new Dictionary<string, List<Rectangle>>()
                        {
                            { "Walk", new List<Rectangle>
                            { new Rectangle(0, 0, 64, 64), new Rectangle(64, 0, 64, 64), new Rectangle(128, 0, 64, 64), new Rectangle(192, 0, 64, 64) } }
                        };
                        Enemy worm = new Enemy(_enemyTextureRed, new Vector2(x * 32, y * 32), 3, 20, _player, wormAnimations, 0.2);
                        _enemies.Add(worm);
                    }
                    else if (pixelColor == Color.Blue)
                    {
                        var eyeAnimations = new Dictionary<string, List<Rectangle>>()
                        {
                            { "Walk", new List<Rectangle>
                            { new Rectangle(0, 0, 64, 64), new Rectangle(64, 0, 64, 64), new Rectangle(128, 0, 64, 64), new Rectangle(192, 0, 64, 64),
                            new Rectangle(256, 0, 64, 64), new Rectangle(320, 0, 64, 64), new Rectangle(384, 0, 64, 64), new Rectangle(448, 0, 64, 64) } }
                        };
                        Enemy eye = new Enemy(_enemyTextureBlue, new Vector2(x * 32, y * 32), 3, 30, _player, eyeAnimations, 0.2);
                        _enemies.Add(eye);
                    }

                    // Door
                    else if (pixelColor == Color.DarkGreen)
                    {
                        _doors.Add(new Door(_doorTexture, new Vector2(x * 32, y * 32), Color.Green, "room2"));
                    }
                    else if (pixelColor == Color.DarkBlue)
                    {
                        _doors.Add(new Door(_doorTexture, new Vector2(x * 32, y * 32), Color.Blue, "room3"));
                    }
                    else if (pixelColor == Color.Yellow)
                    {
                        _doors.Add(new Door(_doorTexture, new Vector2(x * 32, y * 32), Color.Yellow, "room4"));
                    }
                    else if (pixelColor == Color.Maroon)
                    {
                        _doors.Add(new Door(_doorTexture, new Vector2(x * 32, y * 32), Color.Red, "room5"));
                    }

                    // Collectible Card
                    else if (pixelColor == Color.DarkOliveGreen)
                    {
                        _cards.Add(new Card(_cardTexture, new Vector2(x * 32, y * 32), Color.Green));
                    }
                    else if (pixelColor == Color.LightBlue)
                    {
                        _cards.Add(new Card(_cardTexture, new Vector2(x * 32, y * 32), Color.Blue));
                    }
                    else if (pixelColor == Color.LightYellow)
                    {
                        _cards.Add(new Card(_cardTexture, new Vector2(x * 32, y * 32), Color.Yellow));
                    }
                    else if (pixelColor == Color.OrangeRed)
                    {
                        _cards.Add(new Card(_cardTexture, new Vector2(x * 32, y * 32), Color.Red));
                    }

                }
            }
        }
        private string GetCurrentAnimName(SimpleAnimator animator)
        {
            var field = typeof(SimpleAnimator).GetMethod("GetCurrentAnimationName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (string)field.Invoke(animator, null);
        }
        private void ButtonsEvents()
        {
            bool isHoveringPlay = _playButtonRect.Intersects(_cursorRect);
            bool isHoveringExit = _exitButtonRect.Intersects(_cursorRect);

            // --- Play Button ---
            if (isHoveringPlay)
            {
                if (_playButtonAnimator._isFinished || GetCurrentAnimName(_playButtonAnimator) == "Idle")
                    _playButtonAnimator.Play("Hover");

                if (_mouseState.LeftButton == ButtonState.Pressed)
                    _screen = "game";
            }
            else if (_playButtonAnimator._isFinished)
            {
                _playButtonAnimator.Play("Idle");
            }

            // --- Exit Button ---
            if (isHoveringExit)
            {
                if (_exitButtonAnimator._isFinished || GetCurrentAnimName(_exitButtonAnimator) == "Idle")
                    _exitButtonAnimator.Play("Hover");

                if (_mouseState.LeftButton == ButtonState.Pressed)
                    Exit();
            }
            else if (_exitButtonAnimator._isFinished)
            {
                _exitButtonAnimator.Play("Idle");
            }
            if (!isHoveringPlay)
            {
                _playButtonAnimator.Play("Idle");
            }
            if (!isHoveringExit)
            {
                _exitButtonAnimator.Play("Idle");
            }
        }
        private void DrawHearts(GameTime gameTime)
        {
            int spacing = 10;
            int startX = 20;
            int startY = 20;
            int heartSize = 32;

            for (int i = 0; i < _player._health; i++)
            {
                float time = (float)gameTime.TotalGameTime.TotalSeconds;
                float lerpAmount = (float)(Math.Sin(time * 2f) * 0.5f + 0.5f);

                Color pulseColor = Color.Lerp(Color.Yellow, Color.Red, lerpAmount);

                _spriteBatch.Draw(
                    _heartTexture,
                    new Rectangle(startX + i * (heartSize + spacing), startY, heartSize, heartSize),
                    pulseColor
                );
            }
        }
        private void UpdateRoom(GameTime gameTime)
        {
            _player.Update(gameTime);
            _camera.Follow(_player._position);
            _camera.Update();

            foreach (var acid in _acids)
                acid.Update(gameTime, _player);

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime, _player);
                if (_enemies[i]._isDead)
                    _enemies.RemoveAt(i);
            }

            for (int i = _doors.Count - 1; i >= 0; i--)
            {
                _doors[i].Update(this, gameTime, _player, _cards, ref _screen);
            }

            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                _cards[i].Update(_player);
            }

            if (_player._health <= 0)
            {
                _screen = "gameOver";
            }
        }
        private void DrawRoom()
        {
            int screenWidth = 4000;
            int screenHeight = 4000;
            int textureWidth = _bgTexture.Width;
            int textureHeight = _bgTexture.Height;

            for (int y = 0; y < screenHeight; y += textureHeight)
            {
                for (int x = 0; x < screenWidth; x += textureWidth)
                {
                    _spriteBatch.Draw(_bgTexture, new Vector2(x, y), Color.White);
                }
            }

            foreach (var tile in _tiles)
                tile.Draw(_spriteBatch, Content);
            foreach (var card in _cards)
                card.Draw(_spriteBatch);
            foreach (var door in _doors)
                door.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            foreach (var enemy in _enemies)
                enemy.Draw(_spriteBatch);
            foreach (var acid in _acids)
                acid.Draw(_spriteBatch);
        }
    }
    
}
