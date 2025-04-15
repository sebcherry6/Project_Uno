using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Project_Uno2
{
    public class Player
    {
        public Vector2 _position;
        public Vector2 _velocity;

        public Rectangle _hitbox;

        public Texture2D _texture;

        public float _speed = 200f;
        public float _jumpForce = -600f;
        public float _gravity = 700f;
        public float _maxYVelocity = 1f;

        public bool _isOnGround = false;
        public bool _isJumping = false;

        public Player()
        {
            _position = Vector2.Zero;
            _velocity = Vector2.Zero;
            _texture = Game1._publicContent.Load<Texture2D>("Player");
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            //MOVEMENT

            //HORIZONTAL
            _velocity.X = 0;
            if (Game1._keyState.IsKeyDown(Keys.A))
            {
                _velocity.X = -_speed;
            }
            if (Game1._keyState.IsKeyDown(Keys.D))
            {
                _velocity.X = _speed;
            }

            //GRAVITY (when not on a the ground)
            if (!_isOnGround)
            {
                _velocity.Y += _gravity * deltaTime;        
            }
            else
            {
                if (Game1._keyState.IsKeyDown(Keys.Space))
                {
                    _velocity.Y = _jumpForce;
                    _isJumping = true;
                }
            }

            //Update the players position based on velocity
            _position.X += (int)(_velocity.X * deltaTime);
            _position.Y += (int)(_velocity.Y * deltaTime);

            _hitbox = new Rectangle((int)_position.X - (_texture.Width / 2),
                                    (int)_position.Y - (_texture.Height / 2),
                                    _texture.Width, _texture.Height);

            _isOnGround = false;

            for (int i = 0; i < Game1._tiles.Count; i++)
            {
                if (_hitbox.Intersects(Game1._tiles[i]._hitbox) && Game1._tiles[i]._solid)
                {
                    Rectangle intersect = Rectangle.Intersect(_hitbox, Game1._tiles[i]._hitbox);

                    if (intersect.Width < intersect.Height)
                    {
                        if (Game1._tiles[i]._hitbox.Intersects(new Rectangle((int)_position.X - 16, (int)_position.Y - 16, 1, 32)))
                        {
                            _position.X = Game1._tiles[i]._hitbox.Right + 16;
                        }

                        if (Game1._tiles[i]._hitbox.Intersects(new Rectangle((int)_position.X + 16, (int)_position.Y - 16, 1, 32)))
                        {
                            _position.X = Game1._tiles[i]._hitbox.Left - 16;
                        }
                    }
                    else
                    {
                        if (Game1._tiles[i]._hitbox.Intersects(new Rectangle((int)_position.X - 16, (int)_position.Y - 16, 32, 1)))
                        {
                            _position.Y = Game1._tiles[i]._hitbox.Bottom + 16;

                        }

                        if (Game1._tiles[i]._hitbox.Intersects(new Rectangle((int)_position.X - 16, (int)_position.Y + 16, 32, 1)))
                        {
                            _position.Y = Game1._tiles[i]._hitbox.Top - 16;
                            _velocity.Y = 0;
                            _isOnGround = true;
                        }
                    }
                }
            }

        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(_texture, new Vector2(_position.X - 16, _position.Y - 16), Color.White);
        }
    }
}
