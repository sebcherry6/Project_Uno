using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Project_Uno2
{
    public class Player
    {
        public Vector2 _position;
        public Vector2 _velocity;

        public Game1 _gameroot;

        public SimpleAnimator _playerAnim;

        public SpriteEffects _spriteEffects;

        public Rectangle _hitbox;

        public Texture2D _texture;

        public float _speed = 200f;
        public float _jumpForce = -500f;
        public float _gravity = 800f;
        public float _maxYVelocity = 1f;

        public int _health = 5;

        public float _acidCooldownTimer = 0f;

        public bool _isFlashing = false;
        public float _flashTimer = 0f;
        public float _flashDuration = 1.5f;

        public bool _isInvincible = false;
        public float _invincibleTimer = 0f;
        public float _invincibleDuration = 3f;

        private float _blinkTimer = 0f;
        private float _blinkInterval = 0.1f;
        private bool _isVisible = true;

        public bool _isOnGround = false;
        public bool _isJumping = false;

        private string currentAnimation = "Idle"; // To track the current animation

        public Player(Game1 gameroot, Texture2D texture, Vector2 startPosition)
        {
            _gameroot = gameroot;
            _playerAnim = new SimpleAnimator(texture);
            _velocity = Vector2.Zero;
            _texture = texture;
            _position = startPosition;

            List<Rectangle> idleFrames = new();
            List<Rectangle> walkFrames = new();

            for (int i = 0; i < 2; i++)
            {
                idleFrames.Add(new Rectangle(i * 64, 0, 64, 64));
            }
            for (int i = 0; i < 4; i++)
            {
                walkFrames.Add(new Rectangle(i * 64, 65, 64, 64));
            }

            _playerAnim.AddAnimation("Idle", idleFrames, 0.2);
            _playerAnim.AddAnimation("Walk", walkFrames, 0.2);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // MOVEMENT
            _velocity.X = 0;

            // Check for horizontal movement (Left or Right)
            if (_gameroot._keyState.IsKeyDown(Keys.A))
            {
                _velocity.X = -_speed;
                _spriteEffects = SpriteEffects.FlipHorizontally; // Flip sprite for moving left
            }
            else if (_gameroot._keyState.IsKeyDown(Keys.D))
            {
                _velocity.X = _speed;
                _spriteEffects = SpriteEffects.None; // No flip for moving right
            }

            // Handle walking animation
            if (_velocity.X != 0 && _isOnGround)
            {
                if (currentAnimation != "Walk")
                {
                    _playerAnim.Play("Walk");
                    currentAnimation = "Walk"; // Update the current animation to "Walk"
                }
            }
            else if (_velocity.X == 0 && _isOnGround)
            {
                if (currentAnimation != "Idle")
                {
                    _playerAnim.Play("Idle");
                    currentAnimation = "Idle"; // Update the current animation to "Idle"
                }
            }

            // GRAVITY
            if (!_isOnGround)
            {
                _velocity.Y += _gravity * deltaTime;

                // Play "Jump" animation if player is in the air
                if (currentAnimation != "Jump")
                {
                    _playerAnim.Play("Jump");
                    currentAnimation = "Jump"; // Update to "Jump" animation when falling or jumping
                }
            }
            else
            {
                // Reset _isJumping when player is on the ground (so they can jump again)
                _isJumping = false;

                // Player is on the ground, so idle or walk depending on input
                if (_gameroot._keyState.IsKeyDown(Keys.Space) && !_isJumping)
                {
                    _velocity.Y = _jumpForce;
                    _isJumping = true; // Set _isJumping to true when jumping

                    if (currentAnimation != "Jump")
                    {
                        _playerAnim.Play("Jump");
                        currentAnimation = "Jump"; // Update animation to "Jump" when jumping
                    }
                }
                else
                {
                    // If player is not moving horizontally and they are on the ground, play "Idle"
                    if (_velocity.X == 0 && currentAnimation != "Idle")
                    {
                        _playerAnim.Play("Idle");
                        currentAnimation = "Idle"; // Reset to idle if not moving
                    }
                }
            }

            // Apply movement based on velocity
            // Separate horizontal and vertical movement
            float deltaX = _velocity.X * deltaTime;
            float deltaY = _velocity.Y * deltaTime;

            // --- HORIZONTAL MOVEMENT + COLLISION ---
            _position.X += deltaX;
            _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);

            foreach (var tile in Game1._tiles)
            {
                if (!tile._solid) continue;

                if (_hitbox.Intersects(tile._hitbox))
                {
                    Rectangle intersect = Rectangle.Intersect(_hitbox, tile._hitbox);
                    if (deltaX > 0) // moving right
                    {
                        _position.X = tile._hitbox.Left - _hitbox.Width;
                    }
                    else if (deltaX < 0) // moving left
                    {
                        _position.X = tile._hitbox.Right;
                    }

                    _velocity.X = 0;
                    _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);
                }
            }

            // --- VERTICAL MOVEMENT + COLLISION ---
            _position.Y += deltaY;
            _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);

            _isOnGround = false;
            foreach (var tile in Game1._tiles)
            {
                if (!tile._solid) continue;

                if (_hitbox.Intersects(tile._hitbox))
                {
                    Rectangle intersect = Rectangle.Intersect(_hitbox, tile._hitbox);
                    if (deltaY > 0) // falling
                    {
                        _position.Y = tile._hitbox.Top - _hitbox.Height;
                        _isOnGround = true;
                        _isJumping = false;
                    }
                    else if (deltaY < 0) // jumping up
                    {
                        _position.Y = tile._hitbox.Bottom;
                    }

                    _velocity.Y = 0;
                    _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);
                }
            }



            // Handle flashing
            if (_isFlashing)
            {
                _flashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_flashTimer >= _flashDuration)
                {
                    _isFlashing = false;
                }
            }

            // Handle invincibility
            if (_isInvincible)
            {
                _invincibleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_invincibleTimer >= _invincibleDuration)
                {
                    _isInvincible = false;
                }
            }

            if (_isInvincible)
            {
                _invincibleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                _blinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_blinkTimer >= _blinkInterval)
                {
                    _isVisible = !_isVisible; // Flip visible/invisible
                    _blinkTimer = 0f;
                }

                if (_invincibleTimer >= _invincibleDuration)
                {
                    _isInvincible = false;
                    _isVisible = true; // Make sure player is visible again after invincibility ends
                }
            }
            if (_acidCooldownTimer > 0f)
            {
                _acidCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Update the animation
            _playerAnim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible) return; // Don't draw if invisible (during blink)

            Color color = Color.White;

            if (_isFlashing)
            {
                color = Color.Red;
            }

            _playerAnim.Draw(spriteBatch, _position, _spriteEffects, color);
        }
        public void TakeDamage(int amount)
        {
            if (_isInvincible) return; // Don't take damage if invincible

            _health -= amount;

            _isFlashing = true;
            _flashTimer = 0f;

            _isInvincible = true;
            _invincibleTimer = 0f;
        }

    }
}
