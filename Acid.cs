using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Project_Uno2
{
    public class Acid
    {
        private Vector2 _position;
        private Rectangle _hitbox;
        private SimpleAnimator _animator;
        private int _damage;
        private float _damageCooldown = 3f; // seconds between damage
        private float _cooldownTimer = 0f;

        public Acid(Texture2D texture, Vector2 position)
        {
            _position = position;
            _damage = 1;
            _hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32);

            _animator = new SimpleAnimator(texture);
            _animator.AddAnimation("Flow", new List<Rectangle>
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32)
            }, 0.2);
            _animator.Play("Flow");
        }

        public void Update(GameTime gameTime, Player player)
        {
            _animator.Update(gameTime);

            // Only deal damage if player's cooldown has expired
            if (_hitbox.Intersects(player._hitbox) && player._acidCooldownTimer <= 0f)
            {
                player._health -= _damage;
                player._acidCooldownTimer = _damageCooldown; // 3 seconds to escape
                player._isFlashing = true;
                player._flashTimer = 0f;

                player._isInvincible = true;
                player._invincibleTimer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch, new Vector2(_position.X - 16, _position.Y - 16), SpriteEffects.None, Color.White);
        }
    }
}