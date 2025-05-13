using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_Uno2;

public class Card
{
    public Rectangle _hitbox; 
    public Color _color; 
    public Texture2D _texture;
    public Vector2 _position;
    public bool _collected = false;

    public Card(Texture2D texture, Vector2 position, Color color)
    {
        _texture = texture;
        _position = position;
        _color = color;
        _hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32);
    }

    public void Update(Player player)
    {
        if (_hitbox.Intersects(player._hitbox))
        {
            _collected = true; 
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_collected)
        {
            spriteBatch.Draw(_texture, new Vector2(_position.X - 16, _position.Y - 16), _color);
        }
    }
}