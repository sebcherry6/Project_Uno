using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project_Uno2
{
    public class SimpleAnimator
    {
        private Texture2D _texture;
        private Dictionary<string, List<Rectangle>> _animations;
        private Dictionary<string, bool> _isLooping;
        private List<Rectangle> _currentAnimation;
        private double _frameTime;
        private double _timer;
        public bool _isFinished { get; private set; } = false;
        private int _frameIndex;



        public Rectangle _currentFrame { get; private set; }

        public SimpleAnimator(Texture2D texture)
        {
            _texture = texture;
            _animations = new Dictionary<string, List<Rectangle>>();
            _frameTime = 0.1;
            _timer = 0;
            _frameIndex = 0;
            _isLooping = new Dictionary<string, bool>();
        }

        public void AddAnimation(string name, List<Rectangle> frames, double frameDuration, bool loop = true)
        {
            _animations[name] = frames;
            _isLooping[name] = loop;

            // Set default animation
            if (_currentAnimation == null)
            {
                _currentAnimation = frames;
                _frameTime = frameDuration;
                _currentFrame = _currentAnimation[0];
                _isFinished = false;
            }
        }

        public void Play(string name)
        {
            if (!_animations.ContainsKey(name)) return;

            if (_currentAnimation != _animations[name])
            {
                _currentAnimation = _animations[name];
                _frameIndex = 0;
                _timer = 0;
                _currentFrame = _currentAnimation[0];
                _isFinished = false;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_currentAnimation == null || _isFinished) return;

            _timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _frameTime)
            {
                _frameIndex++;
                if (_frameIndex >= _currentAnimation.Count)
                {
                    if (_isLooping.TryGetValue(GetCurrentAnimationName(), out bool loop) && loop)
                    {
                        _frameIndex = 0;
                    }
                    else
                    {
                        _frameIndex = _currentAnimation.Count - 1; // Stay on last frame
                        _isFinished = true;
                    }
                }

                _currentFrame = _currentAnimation[_frameIndex];
                _timer = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
        {
            spriteBatch.Draw(_texture, position, _currentFrame, color, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
        private string GetCurrentAnimationName()
        {
            foreach (var kvp in _animations)
            {
                if (kvp.Value == _currentAnimation)
                    return kvp.Key;
            }
            return null;
        }
    }
}