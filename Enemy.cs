using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Uno2
{
    public class Enemy
    {
        public Vector2 _position;
        public Vector2 _velocity;

        public SimpleAnimator _enemyAnim;
        public SpriteEffects _spriteEffects;

        public Rectangle _hitbox;
        public Texture2D _texture;

        public float _speed;
        public float _gravity = 800f;

        public int _health;
        public bool _isDead = false;
        public bool _isOnGround = false;

        private float _detectionRange = 200f;
        private string currentAnimation = "Idle";
        private bool _movingRight = true;

        private Player _playerRef;

        private float _damageCooldown = 1f; // 1 second cooldown between damaging the player
        private float _damageTimer = 0f;

        public int _damage = 1;

        public Enemy(Texture2D texture, Vector2 startPosition, int health, float speed, Player player, Dictionary<string, List<Rectangle>> animations, double walkSpeed = 0.3)
        {
            _texture = texture;
            _position = startPosition;
            _health = health;
            _speed = speed;
            _playerRef = player;

            _enemyAnim = new SimpleAnimator(texture);

            // Add animations from the dictionary
            foreach (var animation in animations)
            {
                _enemyAnim.AddAnimation(animation.Key, animation.Value, walkSpeed);
            }

            _enemyAnim.Play("Walk");
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (_isDead) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _damageTimer += deltaTime;

            _velocity.X = 0;

            float distanceToPlayer = Vector2.Distance(_position, _playerRef._position);

            // Chase if close
            if (distanceToPlayer < _detectionRange)
            {
                if (_playerRef._position.X < _position.X)
                {
                    _velocity.X = -_speed;
                    _spriteEffects = SpriteEffects.None;
                }
                else
                {
                    _velocity.X = _speed;
                    _spriteEffects = SpriteEffects.FlipHorizontally;
                }

                if (currentAnimation != "Walk")
                {
                    _enemyAnim.Play("Walk");
                    currentAnimation = "Walk";
                }
            }
            else
            {
                // Patrol when player not nearby
                if (_movingRight)
                {
                    _velocity.X = _speed * 0.5f;
                    _spriteEffects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    _velocity.X = -_speed * 0.5f;
                    _spriteEffects = SpriteEffects.None;
                }

                if (currentAnimation != "Walk")
                {
                    _enemyAnim.Play("Walk");
                    currentAnimation = "Walk";
                }
            }
            // Gravity handling
            if (!_isOnGround)
            {
                _velocity.Y += _gravity * deltaTime;
            }
            // Apply movement
            _position.X += _velocity.X * deltaTime;
            _position.Y += _velocity.Y * deltaTime;

            _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);

            _isOnGround = false;

            // Tile collision logic
            foreach (var tile in Game1._tiles)
            {
                if (!tile._solid) continue;

                if (_hitbox.Intersects(tile._hitbox))
                {
                    Rectangle intersect = Rectangle.Intersect(_hitbox, tile._hitbox);

                    if (intersect.Width < intersect.Height)
                    {
                        if (_velocity.X > 0)
                        {
                            _position.X = tile._hitbox.Left - _hitbox.Width;
                            _movingRight = false;
                        }
                        else if (_velocity.X < 0)
                        {
                            _position.X = tile._hitbox.Right;
                            _movingRight = true;
                        }

                        _velocity.X = 0;
                    }
                    else
                    {
                        if (_velocity.Y > 0)
                        {
                            _position.Y = tile._hitbox.Top - _hitbox.Height;
                            _velocity.Y = 0f;
                            _isOnGround = true;
                        }
                        else if (_velocity.Y < 0)
                        {
                            _position.Y = tile._hitbox.Bottom;
                            _velocity.Y = 0;
                        }
                    }
                    _hitbox = new Rectangle((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), 64, 64);
                }
            }

            // Player collision logic
            if (_playerRef._hitbox.Intersects(this._hitbox))
            {
                Rectangle intersection = Rectangle.Intersect(_playerRef._hitbox, this._hitbox);

                // Player jumps on top of the enemy
                if (intersection.Height < intersection.Width && _playerRef._velocity.Y > 0)
                {
                    TakeDamage(1);
                    _playerRef._velocity.Y = _playerRef._jumpForce * 0.5f; // Bounce
                }
                // Enemy damages player only if hit from left or right
                else if (intersection.Width < intersection.Height)
                {
                    if (_damageTimer >= _damageCooldown)
                    {
                        _playerRef.TakeDamage(_damage);
                        _damageTimer = 0f;
                    }
                }
            }

            _enemyAnim.Update(gameTime);

            if (_health <= 0)
            {
                _isDead = true;
            }
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isDead) return;

            _enemyAnim.Draw(spriteBatch, _position, _spriteEffects, Color.White);
        }
    }
}