using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_Uno2;
using System;
using System.Collections.Generic;

public class Door
{
    public Rectangle _hitbox;
    public Vector2 _position;
    public Color _color;
    public string _destinationLevel;
    public bool _isOpen = false;
    private SimpleAnimator _animator;
    private string _currentAnim = "Closed";


    public Door(Texture2D texture, Vector2 position, Color color, string destinationLevel)
    {
        _position = position;
        _color = color;
        _destinationLevel = destinationLevel;
        _hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 64);
        _animator = new SimpleAnimator(texture);
        List<Rectangle> openFrames = new List<Rectangle>();

        for (int i = 0; i < 5; i++)
        {
            openFrames.Add(new Rectangle(i * 32, 0, 32, 64));
        }

        _animator.AddAnimation("Opening", openFrames, 0.2f, false); // Non-looping
        _animator.AddAnimation("Closed", new List<Rectangle> { new Rectangle(0, 0, 32, 64) }, 1f, true);
    }

    public void Update(Game1 gameroot, GameTime gameTime, Player player, List<Card> cards, ref string currentScreen)
    {
        _animator.Update(gameTime);

        if (!_isOpen)
        {
            foreach (var card in cards)
            {
                if (card._collected && card._color == _color)
                {
                    if (_currentAnim != "Opening")
                    {
                        _animator.Play("Opening");
                        _currentAnim = "Opening";
                    }

                    if (gameroot._keyState.IsKeyDown(Keys.E) && gameroot._oldKeyState.IsKeyUp(Keys.E))
                    {
                        _isOpen = true;
                        currentScreen = _destinationLevel;
                    }

                    break;
                }
            }
        }

        // Mark as open after animation is done
        if (_currentAnim == "Opening" && _animator._isFinished)
        {
            _currentAnim = "Open"; // Just a flag; no animation needed
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_currentAnim == "Closed")
        {
            _animator.Play("Closed");
        }

        _animator.Draw(spriteBatch, new Vector2((int)_position.X - 16, (int)_position.Y - 48), SpriteEffects.None, _color);
    }
}